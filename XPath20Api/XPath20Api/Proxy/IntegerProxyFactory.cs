﻿// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;

namespace Wmhelp.XPath2.Proxy
{
    internal class IntegerProxyFactory : ValueProxyFactory
    {
        public const int Code = 4;

        public override ValueProxy Create(object value)
        {
            return new IntegerProxy((Integer)value);
        }

        public override int GetValueCode()
        {
            return Code;
        }

        public override Type GetValueType()
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
                case ShortFactory.Code:
                case UIntFactory.Code:
                case IntFactory.Code:
                case ULongFactory.Code:
                case LongFactory.Code:
                    return 1;

                case IntegerProxyFactory.Code:
                    return 0;

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
