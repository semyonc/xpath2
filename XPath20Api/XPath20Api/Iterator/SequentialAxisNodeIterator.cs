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
    abstract class SequentialAxisNodeIterator : AxisNodeIterator
    {
        protected SequentialAxisNodeIterator()
        {
        }

        public SequentialAxisNodeIterator(XPath2Context context, object nodeTest, bool matchSelf, XPath2NodeIterator iter)
            : base(context, nodeTest, matchSelf, iter)
        {
        }

        private bool first = false;

        protected abstract bool MoveToFirst(XPathNavigator nav);

        protected abstract bool MoveToNext(XPathNavigator nav);

        protected override XPathItem NextItem()
        {
            while (true)
            {
                if (!accept)
                {
                    if (!MoveNextIter())
                        return null;
                    first = true;
                    if (matchSelf && TestItem())
                    {
                        sequentialPosition++;
                        return curr;
                    }
                }
                if (first)
                {
                    accept = MoveToFirst(curr);
                    first = false;
                }
                else
                    accept = MoveToNext(curr);
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
