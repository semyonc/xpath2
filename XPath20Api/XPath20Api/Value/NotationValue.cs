// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;

namespace Wmhelp.XPath2.Value
{
    public class NotationValue : IXmlConvertable
    {
        public NotationValue(QNameValue name)
        {
            Prefix = name.Prefix;
            LocalName = name.LocalName;
            NamespaceUri = name.NamespaceUri;
        }

        public String Prefix { get; private set; }
        public String LocalName { get; private set; }
        public String NamespaceUri { get; private set; }

        public override bool Equals(object obj)
        {
            NotationValue other = obj as NotationValue;
            if (other != null)
            {
                if (LocalName == other.LocalName &&
                    NamespaceUri == other.NamespaceUri)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return LocalName.GetHashCode() ^ NamespaceUri.GetHashCode() << 8; 
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Prefix != "")
            {
                sb.Append(Prefix);
                sb.Append(':');
            }
            sb.Append(LocalName);
            return sb.ToString();
        }

        #region IXmlConvertable Members

        object IXmlConvertable.ValueAs(SequenceType type, XmlNamespaceManager nsmgr)
        {
            switch (type.TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.Notation:
                    return this;
                case XmlTypeCode.String:
                    return ToString();
                case XmlTypeCode.UntypedAtomic:
                    return new UntypedAtomic(ToString());
                default:
                    throw new InvalidCastException();
            }
        }

        #endregion

        public static NotationValue Parse(string name, XmlNamespaceManager resolver)
        {
            return new NotationValue(QNameValue.Parse(name, resolver));
        }
    }
}
