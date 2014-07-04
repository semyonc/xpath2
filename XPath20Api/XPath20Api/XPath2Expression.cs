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
        private object[] dataPool;
        private XPath2ResultType? returnType;

        private XPath2Expression(String expr, AbstractNode exprTree, NameBinder.ReferenceLink[] variables, object[] variableValues)
        {
            this.expr = expr;
            this.exprTree = exprTree;
            dataPool = new object[exprTree.Context.NameBinder.Length];
            if (variables != null)
            {
                for (int k = 0; k < variables.Length; k++)
                    variables[k].Set(dataPool, PrepareValue(variableValues[k]));
            }
        }

        private XPath2Expression(String expr, AbstractNode exprTree, object[] dataPool)
        {
            this.expr = expr;
            this.exprTree = exprTree;
            this.dataPool = dataPool;
        }

        public XPath2Expression Clone()
        {
            return new XPath2Expression(expr, exprTree, dataPool);
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
            XPath2NodeIterator iter = value as XPath2NodeIterator;
            if (iter != null)
                return iter;
            IEnumerable<XNode> en = value as IEnumerable<XNode>;
            if (en != null)
                return new NodeIterator(CreateIterator(en));
            IEnumerable<Object> eno = value as IEnumerable<Object>;
            if (eno != null)
                return new NodeIterator(CreateIterator(eno));
            return value;
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

        public static object Evaluate(string xpath2, IDictionary<XmlQualifiedName, object> param)
        {
            return Evalute(xpath2, null, param);
        }

        public static object Evaluate(string xpath2, IXmlNamespaceResolver nsResolver)
        {
            return Evaluate(xpath2, nsResolver, null);
        }

        public static object Evaluate(string xpath2, IXmlNamespaceResolver nsResolver, object arg)
        {
            return XPath2Expression.Compile(xpath2, nsResolver, arg).Evaluate();
        }

        public static object Evalute(string xpath2, IXmlNamespaceResolver nsResolver, IDictionary<XmlQualifiedName, object> param)
        {
            return XPath2Expression.Compile(xpath2, nsResolver, param).Evaluate();
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

        public static IEnumerable<Object> SelectValues(string xpath, IDictionary<XmlQualifiedName,object> param)
        {
            return SelectValues(xpath, null, param);
        }

        public static IEnumerable<Object> SelectValues(string xpath, IXmlNamespaceResolver resolver, IDictionary<XmlQualifiedName, object> param)
        {
            XPath2NodeIterator iter = XPath2NodeIterator.Create(Compile(xpath, resolver, param).Evaluate());
            while (iter.MoveNext())
                yield return iter.Current.GetTypedValue();
        }

        public static XPath2Expression Compile(string xpath, IXmlNamespaceResolver resolver, object arg)
        {
            IDictionary<string, object> param = null;
            if (arg != null)
            {
                Type type = arg.GetType();
                if (type.IsArray)
                    throw new ArgumentException("arg");
                PropertyInfo[] props = type.GetProperties();
                param = new Dictionary<string, object>(props.Length);
                for (int k = 0; k < props.Length; k++)
                {
                    PropertyInfo property = props[k];
                    param.Add(property.Name, property.GetValue(arg, null)); 
                }
            }
            return Compile(xpath, resolver, param);
        }

        public static XPath2Expression Compile(string xpath, IXmlNamespaceResolver resolver, IDictionary<XmlQualifiedName, object> param)
        {
            if (xpath == null)
                throw new ArgumentNullException("xpath");
            if (xpath == "")
                throw new XPath2Exception("Empty xpath expression");
            NameBinder.ReferenceLink[] variables = null;
            object[] variableValues = null;
            XPath2Context context = new XPath2Context(resolver);
            if (param != null)
            {
                variables = new NameBinder.ReferenceLink[param.Count];
                variableValues = new object[param.Count];
                var array = new KeyValuePair<XmlQualifiedName, object>[param.Count];
                param.CopyTo(array, 0);
                for (int k = 0; k < array.Length; k++)
                {
                    variables[k] = context.NameBinder.PushVar(array[k].Key);
                    variableValues[k] = array[k].Value;
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

        public object Evaluate(IContextProvider provider)
        {
            object res = exprTree.Execute(provider, dataPool);           
            if (res is XPathItem)
            {
                XPathItem item = (XPathItem)res;
                if (!item.IsNode)
                    res = item.TypedValue;
            }
            returnType = CoreFuncs.GetXPath2ResultType(res);
            ValueProxy proxy = res as ValueProxy;
            if (proxy != null)
                return proxy.Value;
            return res;
        }

        public String Expression { get { return expr; } }

        public XPath2ResultType ReturnType
        { 
            get 
            {
                if (!returnType.HasValue)
                    returnType = exprTree.GetReturnType(dataPool);
                return returnType.Value;
            } 
        }
    }
}
