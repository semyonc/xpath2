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

namespace Wmhelp.XPath2.Iterator
{
    sealed class SpecialDescendantNodeIterator: AxisNodeIterator
    {
        private XPathNodeType kind;

        public SpecialDescendantNodeIterator(XPath2Context context, object nodeTest, bool matchSelf, XPath2NodeIterator iter)
            : base(context, nodeTest, matchSelf, iter)
        {
            kind = XPathNodeType.All;
            if (nameTest != null ||
                 (typeTest != null && typeTest.GetNodeKind() == XPathNodeType.Element))
                kind = XPathNodeType.Element;
        }

        private SpecialDescendantNodeIterator(SpecialDescendantNodeIterator src)
        {
            AssignFrom(src);
            kind = src.kind;
        }

        public override XPath2NodeIterator Clone()
        {
            return new SpecialDescendantNodeIterator(this);
        }

        private int depth;

        protected override XPathItem NextItem()
        {
        MoveNextIter:
            if (!accept)
            {
                if (!MoveNextIter())
                    return null;
                if (matchSelf && TestItem())
                {
                    sequentialPosition++;
                    return curr;
                }
            }

        MoveToFirstChild:
            if (curr.MoveToChild(kind))
            {
                depth++;
                goto TestItem;
            }

        MoveToNext:
            if (depth == 0)
            {
                accept = false;
                goto MoveNextIter;
            }
            if (!curr.MoveToNext(kind))
            {
                curr.MoveToParent();
                depth--;
                goto MoveToNext;
            }

        TestItem:
            if (!TestItem())
                goto MoveToFirstChild;
            sequentialPosition++;
            return curr;
        }
    }
}
