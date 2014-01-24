// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2014, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;

namespace Wmhelp.XPath2.AST
{
    sealed class RangeNode: AbstractNode
    {
        public RangeNode(XPath2Context context, object node1, object node2)
            : base(context)
        {
            Add(node1);
            Add(node2);
        }

        public override object Execute(IContextProvider provider, object[] dataPool)
        {
            return CoreFuncs.GetRange(this[0].Execute(provider, dataPool), 
                this[1].Execute(provider, dataPool));
        }

        public override XPath2ResultType GetReturnType(object[] dataPool)
        {
            return XPath2ResultType.NodeSet;
        }

        internal override XPath2ResultType GetItemType(object[] dataPool)
        {
            return XPath2ResultType.Number;
        }
    }
}
