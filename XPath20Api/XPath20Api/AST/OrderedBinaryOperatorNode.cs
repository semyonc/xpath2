// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;

namespace Wmhelp.XPath2.AST
{
    sealed class OrderedBinaryOperatorNode: BinaryOperatorNode
    {
        public OrderedBinaryOperatorNode(XPath2Context context, BinaryOperator action, object node1, object node2)
            : base(context, action, node1, node2)
        {
        }
    }
}
