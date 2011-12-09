// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Reflection;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Linq;

using Wmhelp.XPath2.AST;
using Wmhelp.XPath2.Proxy;

namespace Wmhelp.XPath2
{
    public class XPath2Expression
    {
        private string expr;
        private AbstractNode exprTree;
        private NameBinder.ReferenceLink[] variables;
        private object[] variableValues;

        private XPath2Expression(String expr, AbstractNode exprTree, 
            NameBinder.ReferenceLink[] vars, object[] values)
        {
            this.expr = expr;
            this.exprTree = exprTree;
            this.variables = vars;
            this.variableValues = values;
        }

        public XPath2Expression Clone()
        {
            return new XPath2Expression(expr, exprTree, variables, variableValues);
        }

        public static XPath2Expression Compile(string xpath)
        {
            return Compile(xpath, null, null);
        }

        public static XPath2Expression Compile(string xpath, IXmlNamespaceResolver resolver)
        {
            return Compile(xpath, resolver, null);
        }

        public static XPath2Expression Compile(string xpath, object arg)
        {
            return Compile(xpath, null, arg);
        }     

        public static object Evaluate(string xpath2, object arg)
        {
            return Evaluate(xpath2, null, arg);
        }

        public static object Evaluate(string xpath2, IXmlNamespaceResolver nsResolver)
        {
            return Evaluate(xpath2, nsResolver, null);
        }

        public static object Evaluate(string xpath2, IXmlNamespaceResolver nsResolver, object arg)
        {
            return XPath2Expression.Compile(xpath2, nsResolver, arg).Evaluate();
        }

        public static IEnumerable<T> Select<T>(string xpath, object arg)
            where T : XObject
        {
            return Select<T>(xpath, null, arg);
        }

        public static IEnumerable<T> Select<T>(string xpath, IXmlNamespaceResolver resolver, object arg)
            where T : XObject
        {
            XPath2NodeIterator iter = XPath2NodeIterator.Create(Compile(xpath, resolver, arg).Evaluate());
            foreach (XPathItem item in iter)
                if (item.IsNode)
                {
                    XPathNavigator curr = (XPathNavigator)item;
                    XObject o = (XObject)curr.UnderlyingObject;
                    if (!(o is T))
                        throw new InvalidOperationException(String.Format("Unexpected evalution {0}", o.GetType()));
                    yield return (T)o;
                }
                else
                    throw new InvalidOperationException(String.Format("Unexpected evalution {0}", item.TypedValue.GetType()));
        }

        public static IEnumerable<Object> SelectValues(string xpath, object arg)
        {
            return SelectValues(xpath, null, arg);
        }

        public static IEnumerable<Object> SelectValues(string xpath, IXmlNamespaceResolver resolver, object arg)
        {
            XPath2NodeIterator iter = XPath2NodeIterator.Create(Compile(xpath, resolver, arg).Evaluate());
            while (iter.MoveNext())
                yield return iter.Current.GetTypedValue();
        }

        public static XPath2Expression Compile(string xpath, IXmlNamespaceResolver resolver, object arg)
        {
            if (xpath == null)
                throw new ArgumentNullException("xpath");
            if (xpath == "")
                throw new XPath2Exception("Empty xpath expression");
            NameBinder.ReferenceLink[] variables = null;
            object[] variableValues = null;
            XPath2Context context = new XPath2Context(resolver);
            if (arg != null)
            {
                Type type = arg.GetType();
                if (type.IsArray)
                    throw new ArgumentException("arg");
                PropertyInfo[] props = type.GetProperties();
                variables = new NameBinder.ReferenceLink[props.Length];
                variableValues = new object[props.Length];
                for (int k = 0; k < props.Length; k++)
                {
                    PropertyInfo property = props[k];
                    variables[k] = context.NameBinder.PushVar(new XmlQualifiedName(property.Name));
                    variableValues[k] = property.GetValue(arg, null);
                }
            }
            Tokenizer tokenizer = new Tokenizer(xpath);
            YYParser parser = new YYParser(context);
            AbstractNode exprTree = (AbstractNode)parser.yyparseSafe(tokenizer);
            exprTree.Bind();
            return new XPath2Expression(xpath, exprTree, variables, variableValues);
        }

        public object Evaluate()
        {
            return Evaluate(null);
        }

        private IEnumerable<XPathItem> CreateIterator(IEnumerable<XNode> en)
        {
            foreach (XNode node in en)
                yield return node.CreateNavigator();
        }

        private IEnumerable<XPathItem> CreateIterator(IEnumerable<Object> en)
        {
            foreach (object item in en)
                yield return new XPath2Item(item);
        }

        private object PrepareValue(object value)
        {
            if (value == null)
                return Undefined.Value;
            XmlNode xmlNode = value as XmlNode;
            if (xmlNode != null)
                return xmlNode.CreateNavigator();
            XNode xnode = value as XNode;
            if (xnode != null)
                return xnode.CreateNavigator();
            IEnumerable<XNode> en = value as IEnumerable<XNode>;
            if (en != null)
                return new NodeIterator(CreateIterator(en));
            IEnumerable<Object> eno = value as IEnumerable<Object>;
            if (eno != null)
                return new NodeIterator(CreateIterator(eno));
            return value;
        }

        public object Evaluate(IContextProvider provider)
        {
            object[] dataPool = new object[exprTree.Context.NameBinder.Length];
            if (variables != null)
            {
                for (int k = 0; k < variables.Length; k++)
                    variables[k].Set(dataPool, PrepareValue(variableValues[k]));
            }
            object res = exprTree.Execute(provider, dataPool);
            ValueProxy proxy = res as ValueProxy;
            if (proxy != null)
                return proxy.Value;
            return res;
        }

        public String Expression { get { return expr; } }

        public XPathResultType ReturnType { get { throw new NotImplementedException(); } }
    }
}
