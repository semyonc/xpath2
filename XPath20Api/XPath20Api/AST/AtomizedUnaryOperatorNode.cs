// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;

namespace Wmhelp.XPath2.AST
{
    class AtomizedUnaryOperatorNode : UnaryOperatorNode
    {
        public AtomizedUnaryOperatorNode(XPath2Context context, UnaryOperator action, object node)
            : base(context, action, node)
        {
        }

        public override object Execute(IContextProvider provider, object[] dataPool)
        {
            object value = CoreFuncs.Atomize(this[0].Execute(provider, dataPool));
            if (value != Undefined.Value)
                return _unaryOper(provider, value);
            return Undefined.Value;
        }
    }
}
