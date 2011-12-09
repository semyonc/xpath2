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
    sealed class ChildNodeIterator : SequentialAxisNodeIterator
    {
        public ChildNodeIterator(XPath2Context context, object nodeTest, XPath2NodeIterator iter)
            : base(context, nodeTest, false, iter)
        {
        }

        private ChildNodeIterator(AxisNodeIterator src)
        {
            AssignFrom(src);
        }

        public override XPath2NodeIterator Clone()
        {
            return new ChildNodeIterator(this);            
        }

        protected override bool MoveToFirst(XPathNavigator nav)
        {
            return nav.MoveToFirstChild();
        }

        protected override bool MoveToNext(XPathNavigator nav)
        {
            return nav.MoveToNext();
        }
    }
}
