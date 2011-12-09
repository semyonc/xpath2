// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;

namespace Wmhelp.XPath2.Proxy
{
    internal class DoubleProxyFactory : ValueProxyFactory
    {
        public const int Code = 7;

        public override ValueProxy Create(object value)
        {
            return new DoubleProxy((double)value);
        }

        public override int GetValueCode()
        {
            return Code;
        }

        public override Type GetValueType()
        {
            return typeof(System.Double);
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
                case ShortFactory.Code:
                case UIntFactory.Code:
                case IntFactory.Code:
                case ULongFactory.Code:
                case LongFactory.Code:
                case IntegerProxyFactory.Code:
                case DecimalProxyFactory.Code:
                case FloatFactory.Code:
                    return 1;

                case DoubleProxyFactory.Code:
                    return 0;

                default:
                    return -2;
            }
        }
    }
}
