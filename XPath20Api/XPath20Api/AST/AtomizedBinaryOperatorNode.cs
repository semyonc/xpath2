// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;

namespace Wmhelp.XPath2.AST
{
    class AtomizedBinaryOperatorNode: BinaryOperatorNode
    {
        public AtomizedBinaryOperatorNode(XPath2Context context, BinaryOperator action, object node1, object node2)
            : base(context, action, node1, node2)
        {
        }

        public override object Execute(IContextProvider provider, object[] dataPool)
        {
            object value1 = CoreFuncs.Atomize(this[0].Execute(provider, dataPool));
            if (value1 != Undefined.Value)
            {
                object value2 = CoreFuncs.Atomize(this[1].Execute(provider, dataPool));
                if (value2 != Undefined.Value)
                    return _binaryOper(provider, value1, value2);
            }
            return Undefined.Value;
        }
    }
}
