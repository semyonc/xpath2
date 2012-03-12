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

using Wmhelp.XPath2.AST;

namespace Wmhelp.XPath2.Iterator
{
    sealed class ExprNodeIterator : XPath2NodeIterator
    {
        private XPath2Context context;
        private AbstractNode node;
        private object[] dataPool;
        private XPath2NodeIterator baseIter;
        private XPath2NodeIterator iter;
        private int sequentialPosition;

        private ExprNodeIterator()
        {
        }

        public ExprNodeIterator(XPath2Context context, AbstractNode node, object[] dataPool, XPath2NodeIterator baseIter)
        {
            this.context = context;
            this.node = node;
            this.dataPool = dataPool;
            this.baseIter = baseIter;
        }

        public override XPath2NodeIterator Clone()
        {
            ExprNodeIterator res = new ExprNodeIterator();
            res.context = context;
            res.node = node;
            res.dataPool = dataPool;
            res.baseIter = baseIter.Clone();
            return res;
        }

        public override XPath2NodeIterator CreateBufferedIterator()
        {
            return new BufferedNodeIterator(this);
        }

        protected override XPathItem NextItem()
        {
            while (true)
            {
                if (iter == null)
                {
                    if (!baseIter.MoveNext())
                        return null;
                    sequentialPosition = 0;
                    iter = XPath2NodeIterator.Create(node.Execute(new NodeProvider(baseIter.Current), dataPool));
                }
                if (iter.MoveNext())
                {
                    sequentialPosition++;
                    return iter.Current;
                }
                iter = null;
            }
        }

        public override int SequentialPosition
        {
            get
            {
                return sequentialPosition;
            }
        }

        public override void ResetSequentialPosition()
        {
            iter = null;
        }
    }

}
