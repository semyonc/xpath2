// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

namespace Wmhelp.XPath2
{
    class NodeList : XmlNodeList
    {
        private List<XmlNode> _list;
        private XPath2NodeIterator _iter;
        private bool _done;

        public NodeList(XPath2NodeIterator iter, XmlDocument doc)
        {
            _list = new List<XmlNode>();
            _iter = iter;
        }

        public override int Count
        {
            get
            {
                if (!_done)
                    Item(Int32.MaxValue);
                return _list.Count;
            }
        }

        private XmlNode GetNode(XPathItem item)
        {
            if (item.IsNode)
                return ToXmlNode((XPathNavigator)item);
            return null;
        }

        public static XmlNode ToXmlNode(XPathNavigator nav)
        {
            IHasXmlNode hasNode = nav as IHasXmlNode;
            if (hasNode != null)
            {
                XmlNode node = hasNode.GetNode();
                if (node != null)
                    return node;
            }
            return null;
        }

        public override IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public override XmlNode Item(int index)
        {
            if (_list.Count <= index && !_done)
            {
                int count = _list.Count;
                while (!_done && (count <= index))
                {
                    if (_iter.MoveNext())
                    {
                        XmlNode node = GetNode(_iter.Current);
                        if (node != null)
                        {
                            _list.Add(node);
                            count++;
                        }
                    }
                    else
                        _done = true;
                }
            }
            if (index >= 0 && _list.Count > index)
                return _list[index];
            return null;
        }

        private class Enumerator : IEnumerator, IEnumerator<XmlNode>
        {
            private NodeList _owner;
            private XmlNode _curr;
            private int _pos = -1;

            public Enumerator(NodeList owner)
            {
                _owner = owner;
            }

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get
                {
                    if (_pos == -1)
                        throw new InvalidOperationException();
                    return _curr;
                }
            }

            bool IEnumerator.MoveNext()
            {
                XmlNode node = _owner[_pos + 1];
                if (node != null)
                {
                    _curr = node;
                    _pos++;
                    return true;
                }
                return false;
            }

            void IEnumerator.Reset()
            {
                _pos = -1;
            }

            #endregion

            #region IEnumerator<XmlNode> Members

            XmlNode IEnumerator<XmlNode>.Current
            {
                get
                {
                    if (_pos == -1)
                        throw new InvalidOperationException();
                    return _curr;
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                return;
            }

            #endregion
        }
    }

}
