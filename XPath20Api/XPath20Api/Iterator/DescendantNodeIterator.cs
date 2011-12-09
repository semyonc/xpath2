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
    sealed class DescendantNodeIterator : AxisNodeIterator
    {
        public DescendantNodeIterator(XPath2Context context, object nodeTest, bool matchSelf, XPath2NodeIterator iter)
            : base(context, nodeTest, matchSelf, iter)
        {
        }

        private DescendantNodeIterator(AxisNodeIterator src)
        {
            AssignFrom(src);
        }

        public override XPath2NodeIterator Clone()
        {
            return new DescendantNodeIterator(this);
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
            if (curr.MoveToFirstChild())
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
            if (!curr.MoveToNext())
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
