// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;

namespace Wmhelp.XPath2.Proxy
{
    internal class BoolFactory : ValueProxyFactory
    {
        public const int Code = 21;

        public override ValueProxy Create(object value)
        {
            return new Bool((bool)value);
        }

        public override int GetValueCode()
        {
            return Code;
        }

        public override Type GetValueType()
        {
            return typeof(System.Boolean);
        }

        public override bool IsNumeric
        {
            get { return false; }
        }

        public override int Compare(ValueProxyFactory other)
        {
            if (other.GetValueCode() == Code)
                return 0;
            return -2;
        }
    }
}
