// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Wmhelp.XPath2
{
    sealed class XPathNodeIteratorAdapter: XPathNodeIterator
    {
        private XPath2NodeIterator iter;

        public XPathNodeIteratorAdapter(XPath2NodeIterator iter)
        {
            this.iter = iter.Clone();
        }

        public override XPathNodeIterator Clone()
        {
            return new XPathNodeIteratorAdapter(iter);
        }

        public override XPathNavigator Current
        {
            get 
            {
                if (iter.Current.IsNode)
                    return (XPathNavigator)iter.Current;
                return null;
            }
        }

        public override int CurrentPosition
        {
            get 
            {
                return iter.CurrentPosition;
            }
        }

        public override bool MoveNext()
        {
            return iter.MoveNext();
        }
    }
}
