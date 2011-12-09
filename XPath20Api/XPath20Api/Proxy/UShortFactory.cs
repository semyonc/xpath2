// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;

namespace Wmhelp.XPath2.Proxy
{
    internal class UShortFactory : ValueProxyFactory
    {
        public const int Code = 18;

        public override ValueProxy Create(object value)
        {
            return new UShort((ushort)value);
        }

        public override int GetValueCode()
        {
            return Code;
        }

        public override Type GetValueType()
        {
            return typeof(System.UInt16);
        }

        public override Type GetResultType()
        {
            return typeof(System.Int32);
        }

        public override bool IsNumeric
        {
            get { return true; }
        }

        public override int Compare(ValueProxyFactory other)
        {
            switch (other.GetValueCode())
            {
                case SByteProxyFactory.Code:
                case ByteProxyFactory.Code:
                case ShortFactory.Code:
                    return 1;

                case UShortFactory.Code:
                    return 0;

                case UIntFactory.Code:
                case ULongFactory.Code:
                case IntFactory.Code:
                case LongFactory.Code:
                case IntegerProxyFactory.Code:
                case DecimalProxyFactory.Code:
                case FloatFactory.Code:
                case DoubleProxyFactory.Code:
                    return -1;

                default:
                    return -2;
            }
        }
    }
}
