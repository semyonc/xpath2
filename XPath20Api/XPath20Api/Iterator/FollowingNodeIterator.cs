// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.XPath;

using Wmhelp.XPath2.MS;

namespace Wmhelp.XPath2.Iterator
{
    sealed class FollowingNodeIterator : AxisNodeIterator
    {
        private XPathNodeType kind;

        public FollowingNodeIterator(XPath2Context context, object nodeTest, XPath2NodeIterator iter)
            : base(context, nodeTest, false, iter)
        {
            if (typeTest == null)
            {
                if (nameTest == null)
                    kind = XPathNodeType.All;
                else
                    kind = XPathNodeType.Element;
            }
            else
                kind = typeTest.GetNodeKind();
        }

        private FollowingNodeIterator(FollowingNodeIterator src)
        {
            AssignFrom(src);
            kind = src.kind;
        }

        public override XPath2NodeIterator Clone()
        {
            return new FollowingNodeIterator(this);
        }


        protected override XPathItem NextItem()
        {
            while (true)
            {
                if (!accept)
                {
                    if (!MoveNextIter())
                        return null;
                }
                accept = curr.MoveToFollowing(kind);
                if (accept)
                {
                    if (TestItem())
                    {
                        sequentialPosition++;
                        return curr;
                    }
                }
            } 
        }
    }
}
