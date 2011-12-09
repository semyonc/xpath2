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
    class ItemIterator: XPath2NodeIterator
    {
        private XPath2NodeIterator iter;

        public ItemIterator(XPath2NodeIterator baseIter)
        {
            iter = baseIter;
        }

        public override XPath2NodeIterator Clone()
        {
            return new ItemIterator(iter.Clone());
        }

        public override XPath2NodeIterator CreateBufferedIterator()
        {
            return new BufferedNodeIterator(Clone());
        }

        protected override XPathItem NextItem()
        {
            if (CurrentPosition == -1 && iter.IsStarted)
                return iter.Current;
            if (iter.MoveNext())
            {
                if (iter.Current.IsNode)
                    throw new XPath2Exception(Properties.Resources.XPTY0018, "");
                return iter.Current;
            }
            return null;                
        }
    }
}
