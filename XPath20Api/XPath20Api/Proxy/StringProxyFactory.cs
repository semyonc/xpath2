// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;

namespace Wmhelp.XPath2.Proxy
{
    internal class StringProxyFactory : ValueProxyFactory
    {
        public const int Code = 8;

        public override ValueProxy Create(object value)
        {
            return new StringProxy((String)value);
        }

        public override int GetValueCode()
        {
            return Code;
        }

        public override Type GetValueType()
        {
            return typeof(System.String);
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
