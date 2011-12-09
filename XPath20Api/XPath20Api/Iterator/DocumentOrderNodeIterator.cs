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
    sealed class DocumentOrderNodeIterator : XPath2NodeIterator
    {
        private ItemSet itemSet;
        private XPathNavigator lastNode;
        private int index;

        private DocumentOrderNodeIterator(ItemSet itemSet)
        {
            this.itemSet = itemSet;
        }

        public DocumentOrderNodeIterator(XPath2NodeIterator baseIter)
        {
            bool? isNode = null;
            itemSet = new ItemSet();
            while (baseIter.MoveNext())
            {
                if (!isNode.HasValue)
                    isNode = baseIter.Current.IsNode;
                else
                    if (baseIter.Current.IsNode != isNode)
                        throw new XPath2Exception(Properties.Resources.XPTY0018, baseIter.Current.Value);
                itemSet.Add(baseIter.Current.Clone());
            }
            if (isNode.HasValue && isNode.Value)
                itemSet.Sort();
        }

        public override XPath2NodeIterator Clone()
        {
            return new DocumentOrderNodeIterator(itemSet);
        }

        public override XPath2NodeIterator CreateBufferedIterator()
        {
            return Clone();
        }

        protected override XPathItem NextItem()
        {
            while (index < itemSet.Count)
            {
                XPathItem item = itemSet[index++];
                XPathNavigator node = item as XPathNavigator;
                if (node != null)
                {
                    if (lastNode != null)
                    {
                        if (lastNode.IsSamePosition(node))
                            continue;
                    }
                    lastNode = node.Clone();
                }
                return item;
            }
            return null;
        }
    }
}
