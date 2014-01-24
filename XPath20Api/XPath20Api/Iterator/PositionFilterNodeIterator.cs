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
    sealed class PositionFilterNodeIterator: XPath2NodeIterator
    {
        private XPath2NodeIterator iter;
        private int position;        

        public PositionFilterNodeIterator(int pos, XPath2NodeIterator baseIter)
        {
            iter = baseIter;
            position = pos;
        }

        public override XPath2NodeIterator Clone()
        {
            return new PositionFilterNodeIterator(position, iter.Clone());
        }

        public override XPath2NodeIterator CreateBufferedIterator()
        {
            return new BufferedNodeIterator(Clone());
        }

        protected override XPathItem NextItem()
        {
            while (iter.MoveNext())
            {
                if (iter.SequentialPosition == position)
                {
                    iter.ResetSequentialPosition();
                    return iter.Current;
                }
            }
            return null;
        }
    }
}
