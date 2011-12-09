// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Wmhelp.XPath2.Proxy
{
    internal abstract class ValueProxyFactory
    {
        public abstract ValueProxy Create(Object value);

        public abstract int GetValueCode();

        public abstract Type GetValueType();

        public virtual Type GetResultType()
        {
            return GetValueType();
        }

        public abstract bool IsNumeric { get; }        

        public abstract int Compare(ValueProxyFactory other);        
    }
}
