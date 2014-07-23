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
    sealed class ChildOverDescendantsNodeIterator: XPath2NodeIterator
    {
        public class NodeTest
        {
            public XmlQualifiedNameTest nameTest;
            public SequenceType typeTest;

            public NodeTest(object test)
            {
                if (test is XmlQualifiedNameTest)
                    nameTest = (XmlQualifiedNameTest)test;
                else if (test is SequenceType && test != SequenceType.Node)
                    typeTest = (SequenceType)test;
            }
        }

        private XPathNodeType kind;
        private XPath2Context context;
        private NodeTest[] nodeTest;
        private NodeTest lastTest;
        private XPath2NodeIterator iter;
        private XPathNavigator curr;

        public ChildOverDescendantsNodeIterator(XPath2Context context, NodeTest[] nodeTest, XPath2NodeIterator iter)
        {
            this.context = context;
            this.nodeTest = nodeTest;
            this.iter = iter;         
            lastTest = nodeTest[nodeTest.Length - 1];
            kind = XPathNodeType.All;
            if (lastTest.nameTest != null ||
                 (lastTest.typeTest != null && lastTest.typeTest.GetNodeKind() == XPathNodeType.Element))
                kind = XPathNodeType.Element;
        }

        private ChildOverDescendantsNodeIterator(ChildOverDescendantsNodeIterator src)
        {
            context = src.context;
            nodeTest = src.nodeTest;
            iter = src.iter.Clone();
            lastTest = src.lastTest;
            kind = src.kind;
        }

        public override XPath2NodeIterator Clone()
        {
            return new ChildOverDescendantsNodeIterator(this);
        }

        public override XPath2NodeIterator CreateBufferedIterator()
        {
            return new BufferedNodeIterator(Clone());
        }

        private bool TestItem(XPathNavigator nav, NodeTest nodeTest)
        {
            XmlQualifiedNameTest nameTest = nodeTest.nameTest;
            SequenceType typeTest = nodeTest.typeTest;
            if (nameTest != null)
            {

                return (nav.NodeType == XPathNodeType.Element || nav.NodeType == XPathNodeType.Attribute) &&
                    (nameTest.IsNamespaceWildcard || nameTest.Namespace == nav.NamespaceURI) &&
                    (nameTest.IsNameWildcard || nameTest.Name == nav.LocalName);
            }
            else
                if (typeTest != null)
                    return typeTest.Match(nav, context);
            return true;
        }

        private int depth;
        private bool accept;
        private XPathNavigator nav;
        private int sequentialPosition;

        protected override XPathItem NextItem()
        {
        MoveNextIter:
            if (!accept)
            {
                if (!iter.MoveNext())
                    return null;
                XPathNavigator current = iter.Current as XPathNavigator;
                if (current == null)
                    throw new XPath2Exception("XPTY0019", Properties.Resources.XPTY0019, iter.Current.Value);
                if (curr == null || !curr.MoveTo(current))
                    curr = current.Clone();
                sequentialPosition = 0;
                accept = true;
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
            if (depth < nodeTest.Length || !TestItem(curr, lastTest))
                goto MoveToFirstChild;
            if (nav == null || !nav.MoveTo(curr))
                nav = curr.Clone();            
            for (int k = nodeTest.Length - 2; k >= 0; k--)
                if (!(nav.MoveToParent() && TestItem(nav, nodeTest[k])))
                    goto MoveToFirstChild;
            sequentialPosition++;
            return curr;
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
            accept = false;
        }
    }
}
