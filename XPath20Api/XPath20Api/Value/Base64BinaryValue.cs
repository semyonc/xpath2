// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace Wmhelp.XPath2.Value
{
    public class Base64BinaryValue : IXmlConvertable
    {
        public Base64BinaryValue(byte[] binaryValue)
        {
            if (binaryValue == null)
                throw new NullReferenceException();
            BinaryValue = binaryValue;
        }

        public byte[] BinaryValue { get; private set; }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = new XmlTextWriter(sw);
            tw.WriteBase64(BinaryValue, 0, BinaryValue.Length);
            tw.Close();
            return sw.ToString();
        }

        public override bool Equals(object obj)
        {
            Base64BinaryValue other = obj as Base64BinaryValue;
            if (other != null && BinaryValue.Length == other.BinaryValue.Length)
            {
                for (int k = 0; k < BinaryValue.Length; k++)
                    if (BinaryValue[k] != other.BinaryValue[k])
                        return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return BinaryValue.GetHashCode();
        }


        #region IXmlConvertable Members

        object IXmlConvertable.ValueAs(SequenceType type, XmlNamespaceManager nsmgr)
        {
            switch (type.TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.Base64Binary:
                    return this;
                case XmlTypeCode.String:
                    return ToString();
                case XmlTypeCode.UntypedAtomic:
                    return new UntypedAtomic(ToString());
                case XmlTypeCode.HexBinary:
                    return new HexBinaryValue(BinaryValue);
                default:
                    throw new InvalidCastException();
            }
        }

        #endregion
    }
}
