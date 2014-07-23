// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Globalization;

using Wmhelp.XPath2.MS;
using Wmhelp.XPath2.AST;

namespace Wmhelp.XPath2
{
    public class XPath2Context
    {
        public XPath2Context(IXmlNamespaceResolver nsManager)
        {
            NameTable = new NameTable();
            NamespaceManager = new XmlNamespaceManager(NameTable);
            SchemaSet = new XmlSchemaSet(NameTable);
            
            if (nsManager != null)
            {
                foreach (KeyValuePair<String, String> ns in nsManager.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml))
                    NamespaceManager.AddNamespace(ns.Key, ns.Value);
            }

            if (NamespaceManager.LookupNamespace("xs") == null)
                NamespaceManager.AddNamespace("xs", XmlReservedNs.NsXs);
            if (NamespaceManager.LookupNamespace("xsi") == null)
                NamespaceManager.AddNamespace("xsi", XmlReservedNs.NsXsi);
            if (NamespaceManager.LookupNamespace("fn") == null)
                NamespaceManager.AddNamespace("fn", XmlReservedNs.NsXQueryFunc);
            if (NamespaceManager.LookupNamespace("local") == null)
                NamespaceManager.AddNamespace("local", XmlReservedNs.NsXQueryLocalFunc);
            if (NamespaceManager.LookupNamespace("wmh") == null)
                NamespaceManager.AddNamespace("wmh", XmlReservedNs.NsWmhExt);
        }

        public XPath2RunningContext RunningContext { get; set; }

        public XmlNameTable NameTable { get; private set; }

        public XmlNamespaceManager NamespaceManager { get; private set; }

        public XmlSchemaSet SchemaSet { get; set; }        
    }

    public class XPath2RunningContext
    {
        internal DateTime now;

        public XPath2RunningContext()
        {
            now = DateTime.Now;
            NameBinder = new NameBinder();
            
            DefaultCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            DefaultCulture.NumberFormat.CurrencyGroupSeparator = "";
            DefaultCulture.NumberFormat.NumberGroupSeparator = "";
            
            IsOrdered = true;
        }

        public CultureInfo GetCulture(string collationName)
        {
            if (String.IsNullOrEmpty(collationName) ||
                collationName == XmlReservedNs.NsCollationCodepoint)
                return null;
            try
            {
                return CultureInfo.GetCultureInfoByIetfLanguageTag(collationName);
            }
            catch (ArgumentException)
            {
                throw new XPath2Exception("XQST0076", Properties.Resources.XQST0076, collationName);
            }
        }

        public CultureInfo DefaultCulture { get; private set; }

        public String BaseUri { get; set; }

        public bool IsOrdered { get; set; }

        internal NameBinder NameBinder { get; private set; }
    }

}
