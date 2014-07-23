// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011-2014, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Linq;

namespace Wmhelp.XPath2
{
    public static class Extensions
    {
        public static object XPath2Evaluate(this XPathNavigator nav, string xpath2)
        {
            return XPath2Evaluate(nav, xpath2, null);
        }

        public static object XPath2Evaluate(this XPathNavigator nav, string xpath2, IXmlNamespaceResolver nsResolver)
        {
            return XPath2Evaluate(nav, xpath2, nsResolver, null);
        }

        public static object XPath2Evaluate(this XPathNavigator nav, string xpath2, IXmlNamespaceResolver nsResolver, object arg)
        {
            return XPath2Evaluate(nav, XPath2Expression.Compile(xpath2, nsResolver), arg);
        }

        public static object XPath2Evaluate(this XPathNavigator nav, XPath2Expression expr)
        {
            return XPath2Evaluate(nav, expr, null);
        }

        public static object XPath2Evaluate(this XPathNavigator nav, XPath2Expression expr, object arg)
        {
            return expr.EvaluateWithProperties(new NodeProvider(nav), arg);
        } 

        public static XPath2NodeIterator XPath2Select(this XPathNavigator nav, string xpath)
        {
            return XPath2Select(nav, xpath, null);
        }        

        public static XPath2NodeIterator XPath2Select(this XPathNavigator nav, string xpath, object arg)
        {
            return XPath2Select(nav, XPath2Expression.Compile(xpath, null), arg);
        }

        public static XPath2NodeIterator XPath2Select(this XPathNavigator nav, string xpath, IXmlNamespaceResolver resolver)
        {
            return XPath2Select(nav, XPath2Expression.Compile(xpath, resolver), null);
        }

        public static XPath2NodeIterator XPath2Select(this XPathNavigator nav, string xpath, IXmlNamespaceResolver resolver, object arg)
        {
            return XPath2Select(nav, XPath2Expression.Compile(xpath, resolver), arg);
        }

        public static XPath2NodeIterator XPath2Select(this XPathNavigator nav, XPath2Expression expr, object arg)
        {
            return XPath2NodeIterator.Create(XPath2Evaluate(nav, expr));
        }

        public static XPathNodeIterator XPath2SelectNodes(this XPathNavigator nav, string xpath)
        {
            return XPath2SelectNodes(nav, xpath);
        }

        public static XPathNodeIterator XPath2SelectNodes(this XPathNavigator nav, XPath2Expression expr)
        {
            return XPath2SelectNodes(nav, expr, null);
        }

        public static XPathNodeIterator XPath2SelectNodes(this XPathNavigator nav, XPath2Expression expr, object arg)
        {
            return new XPathNodeIteratorAdapter(XPath2Select(nav, expr, arg));
        }

        public static XPathNodeIterator XPath2SelectNodes(this XPathNavigator nav, string xpath, IXmlNamespaceResolver resolver)
        {
            return XPath2SelectNodes(nav, XPath2Expression.Compile(xpath, resolver));
        }

        public static XPathNavigator XPath2SelectSingleNode(this XPathNavigator nav, string xpath)
        {
            return XPath2SelectSingleNode(nav, XPath2Expression.Compile(xpath));
        }

        public static XPathNavigator XPath2SelectSingleNode(this XPathNavigator nav, XPath2Expression expression)
        {
            return XPath2SelectSingleNode(nav, expression, null);
        }

        public static XPathNavigator XPath2SelectSingleNode(this XPathNavigator nav, XPath2Expression expression, object arg)
        {
            XPath2NodeIterator iter = nav.XPath2Select(expression, arg);
            if (iter.MoveNext() && iter.Current.IsNode)
                return (XPathNavigator)iter.Current;
            return null;
        }

        public static XPathNavigator XPath2SelectSingleNode(this XPathNavigator nav, string xpath, IXmlNamespaceResolver resolver)
        {
            return XPath2SelectSingleNode(nav, XPath2Expression.Compile(xpath, resolver));
        }

        public static XmlNodeList XPath2SelectNodes(this XmlNode node, string xpath)
        {
            return XPath2SelectNodes(node, xpath, null);
        }

        public static XmlNodeList XPath2SelectNodes(this XmlNode node, string xpath, IXmlNamespaceResolver nsmgr)
        {
            XPathNavigator nav = node.CreateNavigator();
            return new NodeList(nav.XPath2Select(xpath, nsmgr), node.OwnerDocument);
        }

        public static object XPath2Evaluate(this XmlNode node, string xpath)
        {
            return XPath2Evaluate(node, xpath, null);
        }

        public static object XPath2Evaluate(this XmlNode node, string xpath, object arg)
        {
            return XPath2Evaluate(node, xpath, null, arg);
        }

        public static object XPath2Evaluate(this XmlNode node, string xpath, IXmlNamespaceResolver nsmgr)
        {
            return XPath2Evaluate(node, xpath, nsmgr, null);
        }

        public static object XPath2Evaluate(this XmlNode node, string xpath, IXmlNamespaceResolver nsmgr, object arg)
        {
            XPathNavigator nav = node.CreateNavigator();
            return nav.XPath2Evaluate(xpath, nsmgr, arg);
        }

        public static XmlNode XPath2SelectSingleNode(this XmlNode node, string xquery)
        {
            return XPath2SelectSingleNode(node, xquery, null);
        }

        public static XmlNode XPath2SelectSingleNode(this XmlNode node, string xquery, IXmlNamespaceResolver nsmgr)
        {
            return XPath2SelectSingleNode(node, xquery, nsmgr, null);
        }

        public static XmlNode XPath2SelectSingleNode(this XmlNode node, string xquery, object arg)
        {
            return XPath2SelectSingleNode(node, xquery, null, arg);
        }

        public static XmlNode XPath2SelectSingleNode(this XmlNode node, string xquery, IXmlNamespaceResolver nsmgr, object arg)
        {
            XPathNavigator nav = node.CreateNavigator();
            XPath2NodeIterator iter = nav.XPath2Select(xquery, nsmgr, arg);
            if (iter.MoveNext() && iter.Current.IsNode)
                return NodeList.ToXmlNode((XPathNavigator)iter.Current);
            return null;
        }

        public static IEnumerable<T> XPath2Select<T>(this XNode node, string xpath)
            where T : XObject
        {
            return XPath2Select<T>(node, xpath, null);
        }

        public static IEnumerable<T> XPath2Select<T>(this XNode node, string xpath, IXmlNamespaceResolver nsResolver)
           where T : XObject
        {
            return XPath2Select<T>(node, xpath, nsResolver, null);
        }

        public static IEnumerable<T> XPath2Select<T>(this XNode node, string xpath, object arg)
            where T : XObject
        {
            return XPath2Select<T>(node, xpath, null, arg);
        }

        public static IEnumerable<T> XPath2Select<T>(this XNode node, string xpath, IXmlNamespaceResolver nsResolver, object arg)
            where T : XObject
        {
            return XPath2Select<T>(node, XPath2Expression.Compile(xpath, nsResolver), arg);
        }

        public static IEnumerable<T> XPath2Select<T>(this XNode node, XPath2Expression expression, object arg)
            where T : XObject
        {
            XPathNavigator nav = node.CreateNavigator();
            XPath2NodeIterator iter = nav.XPath2Select(expression, arg);
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

        public static IEnumerable<Object> XPath2Select(this XNode node, string xpath)
        {
            return XPath2Select(node, xpath, null);
        }

        public static IEnumerable<Object> XPath2Select(this XNode node, string xpath, IXmlNamespaceResolver nsResolver)
        {
            return XPath2Select(node, xpath, nsResolver, null);
        }

        public static IEnumerable<Object> XPath2Select(this XNode node, string xpath, object arg)
        {
            return XPath2Select(node, xpath, null, arg);
        }

        public static IEnumerable<Object> XPath2Select(this XNode node, string xpath, IXmlNamespaceResolver nsResolver, object arg)
        {
            return XPath2Select(node, XPath2Expression.Compile(xpath, nsResolver), arg);
        }

        public static IEnumerable<Object> XPath2Select(this XNode node, XPath2Expression expression, object arg)
        {
            XPathNavigator nav = node.CreateNavigator();
            XPath2NodeIterator iter = nav.XPath2Select(expression, arg);
            foreach (XPathItem item in iter)
                if (item.IsNode)
                {
                    XPathNavigator curr = (XPathNavigator)item;
                    yield return curr.UnderlyingObject;
                }
                else
                    yield return item.TypedValue;
        }

        public static T XPath2SelectOne<T>(this XNode node, string xpath)
            where T : XObject
        {
            return XPath2SelectOne<T>(node, xpath, null);
        }

        public static T XPath2SelectOne<T>(this XNode node, string xpath, IXmlNamespaceResolver nsResolver)
            where T : XObject
        {
            return XPath2SelectOne<T>(node, xpath, nsResolver, null);
        }

        public static T XPath2SelectOne<T>(this XNode node, string xpath, object arg)
            where T : XObject
        {
            return XPath2SelectOne<T>(node, xpath, null, arg);
        }

        public static T XPath2SelectOne<T>(this XNode node, string xpath, IXmlNamespaceResolver nsResolver, object arg)
            where T : XObject
        {
            return XPath2SelectOne<T>(node, XPath2Expression.Compile(xpath, nsResolver), arg);
        }

        public static T XPath2SelectOne<T>(this XNode node, XPath2Expression expression, object arg)
            where T : XObject
        {
            return XPath2Select<T>(node, expression, arg).FirstOrDefault();
        }

        public static Object XPath2SelectOne(this XNode node, string xpath)
        {
            return XPath2SelectOne(node, xpath, null);
        }

        public static Object XPath2SelectOne(this XNode node, string xpath, IXmlNamespaceResolver nsResolver)
        {
            return XPath2SelectOne(node, xpath, nsResolver, null);
        }

        public static Object XPath2SelectOne(this XNode node, string xpath, object arg)
        {
            return XPath2SelectOne(node, xpath, null, arg);
        }

        public static Object XPath2SelectOne(this XNode node, string xpath, IXmlNamespaceResolver nsResolver, object arg)
        {
            return XPath2SelectOne(node, XPath2Expression.Compile(xpath, nsResolver), arg);
        }

        public static Object XPath2SelectOne(this XNode node, XPath2Expression expression, object arg)
        {
            return XPath2Select(node, expression, arg).FirstOrDefault<Object>();
        }

        public static IEnumerable<XElement> XPath2SelectElements(this XNode node, string xpath)
        {
            return XPath2SelectElements(node, xpath, null);
        }

        public static IEnumerable<XElement> XPath2SelectElements(this XNode node, string xpath, IXmlNamespaceResolver nsResolver)
        {
            return XPath2SelectElements(node, xpath, nsResolver, null);
        }

        public static IEnumerable<XElement> XPath2SelectElements(this XNode node, string xpath, object arg)
        {
            return XPath2SelectElements(node, xpath, null, arg);
        }

        public static IEnumerable<XElement> XPath2SelectElements(this XNode node, string xpath, IXmlNamespaceResolver nsResolver, object arg)
        {
            return XPath2SelectElements(node, XPath2Expression.Compile(xpath, nsResolver), arg);
        }

        public static IEnumerable<XElement> XPath2SelectElements(this XNode node, XPath2Expression expression)
        {
            return XPath2SelectElements(node, expression, null);
        }

        public static IEnumerable<XElement> XPath2SelectElements(this XNode node, XPath2Expression expression, object arg)
        {
            return XPath2Select<XElement>(node, expression, arg);
        }

        public static XElement XPath2SelectElement(this XNode node, string xpath)
        {
            return XPath2SelectElement(node, xpath, null);
        }

        public static XElement XPath2SelectElement(this XNode node, string xpath, IXmlNamespaceResolver nsResolver)
        {
            return XPath2SelectElement(node, xpath, nsResolver, null);
        }

        public static XElement XPath2SelectElement(this XNode node, string xpath, object arg)
        {
            return XPath2SelectElement(node, xpath, null, arg);
        }

        public static XElement XPath2SelectElement(this XNode node, string xpath, IXmlNamespaceResolver nsResolver, object arg)
        {
            return XPath2SelectElement(node, XPath2Expression.Compile(xpath, nsResolver), arg);
        }

        public static XElement XPath2SelectElement(this XNode node, XPath2Expression expression, object arg)
        {
            return XPath2SelectOne<XElement>(node, expression, arg);
        }

        public static IEnumerable<Object> XPath2SelectValues(this XNode node, string xpath)
        {
            return XPath2SelectValues(node, xpath, null, null);
        }

        public static IEnumerable<Object> XPath2SelectValues(this XNode node, string xpath, IXmlNamespaceResolver nsResolver)
        {
            return XPath2SelectValues(node, xpath, nsResolver, null);
        }

        public static IEnumerable<Object> XPath2SelectValues(this XNode node, string xpath, object arg)
        {
            return XPath2SelectValues(node, xpath, null, arg);
        }

        public static IEnumerable<Object> XPath2SelectValues(this XNode node, string xpath, IXmlNamespaceResolver nsResolver, object arg)
        {
            return XPath2SelectValues(node, XPath2Expression.Compile(xpath, nsResolver), arg);
        }

        public static IEnumerable<Object> XPath2SelectValues(this XNode node, XPath2Expression expr)
        {
            return XPath2SelectValues(node, expr);
        }

        public static IEnumerable<Object> XPath2SelectValues(this XNode node, XPath2Expression expr, object arg)
        {
            XPathNavigator nav = node.CreateNavigator();
            XPath2NodeIterator iter = XPath2NodeIterator.Create(nav.XPath2Evaluate(expr,arg));
            while (iter.MoveNext())
                yield return iter.Current.GetTypedValue();
        }
    }
}