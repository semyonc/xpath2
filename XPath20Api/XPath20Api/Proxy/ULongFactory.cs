// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;

namespace Wmhelp.XPath2.Proxy
{
    internal class ULongFactory : ValueProxyFactory
    {
        public const int Code = 20;

        public override ValueProxy Create(object value)
        {
            return new ULong((ulong)value);
        }

        public override int GetValueCode()
        {
            return Code;
        }

        public override Type GetValueType()
        {
            return typeof(UInt64);
        }

        public override Type GetResultType()
        {
            return typeof(Integer);
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
                case UShortFactory.Code:
                case UIntFactory.Code:
                case ShortFactory.Code:
                case IntFactory.Code:
                case LongFactory.Code:
                    return 1;

                case ULongFactory.Code:
                    return 0;

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
