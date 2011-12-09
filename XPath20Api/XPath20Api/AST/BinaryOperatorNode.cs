// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;

namespace Wmhelp.XPath2.AST
{
    internal delegate object BinaryOperator(IContextProvider provider, object arg1, object arg2);

    class BinaryOperatorNode: AbstractNode
    {
        protected BinaryOperator _binaryOper;

        public BinaryOperatorNode(XPath2Context context, BinaryOperator action, object node1, object node2)
            : base(context)
        {
            _binaryOper = action;
            Add(node1);
            Add(node2);
        }

        public override object Execute(IContextProvider provider, object[] dataPool)
        {
            return _binaryOper(provider, this[0].Execute(provider, dataPool), 
                this[1].Execute(provider, dataPool));
        }
    }
}
