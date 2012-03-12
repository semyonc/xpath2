// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Wmhelp.XPath2
{
    public interface IContextProvider
    {
        XPathItem Context { get; }
       
        int CurrentPosition { get; }
        
        int LastPosition { get; }        
                
    }

    internal sealed class ContextProvider : IContextProvider
    {
        private XPath2NodeIterator m_iter;

        public ContextProvider(object value)
        {
            m_iter = XPath2NodeIterator.Create(value);
        }

        public ContextProvider(XPath2NodeIterator iter)
        {
            m_iter = iter;
        }

        public XPath2NodeIterator Iterator
        {
            get
            {
                return m_iter;
            }
        }

        public bool MoveNext()
        {
            return m_iter.MoveNext();
        }

        #region IContextProvider Members

        public XPathItem Context
        {
            get
            {
                return m_iter.Current;
            }
        }

        public int CurrentPosition
        {
            get
            {
                return m_iter.CurrentPosition + 1;
            }
        }

        public int LastPosition
        {
            get
            {
                return m_iter.Count;
            }
        }

        #endregion
    }


    [DebuggerDisplay("{curr}")]
    //[DebuggerTypeProxy(typeof(XQueryNodeIteratorDebugView))]
    public abstract class XPath2NodeIterator: ICloneable, IEnumerable, IEnumerable<XPathItem>
    {
        internal int count = -1;
        private XPathItem curr;
        private int pos;
        private bool iteratorStarted;
        private bool iteratorFinished;

        public XPath2NodeIterator()
        {
        }

        public abstract XPath2NodeIterator Clone();

        public virtual int Count
        {
            get
            {
                if (this.count == -1)
                {
                    count = 0;
                    XPath2NodeIterator iter = Clone();
                    while (iter.MoveNext())
                        count++;
                }
                return count;
            }
        }

        public virtual bool IsSingleIterator
        {
            get
            {
                XPath2NodeIterator iter = Clone();
                if (iter.MoveNext() && !iter.MoveNext())
                    return true;
                return false;
            }
        }

        public virtual bool IsRange
        {
            get
            {
                return false;
            }
        }

        public XPathItem Current 
        {
            get
            {
                if (!iteratorStarted)
                    throw new InvalidOperationException();
                return curr;
            }
        }

        public int CurrentPosition 
        {
            get
            {
                if (!iteratorStarted)
                    throw new InvalidOperationException();
                return pos;
            }
        }

        public virtual int SequentialPosition
        {
            get
            {
                return CurrentPosition + 1;
            }
        }

        public virtual void ResetSequentialPosition()
        {
            return;
        }

        public bool IsStarted
        {
            get
            {
                return iteratorStarted;
            }
        }

        public virtual bool IsFinished
        {
            get
            {
                return iteratorFinished;
            }
        }

        public bool MoveNext()
        {
            if (!iteratorStarted)
            {
                Init();
                pos = -1;
                iteratorStarted = true;
            }
            XPathItem item = NextItem();
            if (item != null)
            {
                pos++;
                curr = item;
                return true;
            }
            iteratorFinished = true;
            return false;
        }
       
        public virtual List<XPathItem> ToList()
        {
            XPath2NodeIterator iter = Clone();
            List<XPathItem> res = new List<XPathItem>();
            while (iter.MoveNext())
                res.Add(iter.Current.Clone());
            return res;
        }

        public abstract XPath2NodeIterator CreateBufferedIterator();

        protected virtual void Init()
        {
        }

        protected abstract XPathItem NextItem();        

        public static XPath2NodeIterator Create(object value)
        {
            if (value == Undefined.Value)
                return EmptyIterator.Shared;
            XPath2NodeIterator iter = value as XPath2NodeIterator;
            if (iter != null)
                return iter.Clone();
            XPathItem item = value as XPathItem;
            if (item == null)
                item = new XPath2Item(value);
            return new SingleIterator(item);
        }
        

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion        

        #region IEnumerable<XPathItem> Members

        IEnumerator<XPathItem> IEnumerable<XPathItem>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        private class Enumerator : IEnumerator, IEnumerator<XPathItem>
        {
            private XPath2NodeIterator current;
            private bool iterationStarted;
            private XPath2NodeIterator original;

            public Enumerator(XPath2NodeIterator iter)
            {
                original = iter.Clone();
            }

            public object Current
            {
                get 
                {
                    if (!iterationStarted || current == null)
                        throw new InvalidOperationException();
                    return current.Current;
                }
            }

            [DebuggerStepThrough]
            public bool MoveNext()
            {
                if (!iterationStarted)
                {
                    current = original.Clone();
                    iterationStarted = true;
                }
                if (current != null && current.MoveNext())
                    return true;
                current = null;
                return false;
            }

            public void Reset()
            {
                iterationStarted = false;
            }

            #region IEnumerator<XPathItem> Members

            XPathItem IEnumerator<XPathItem>.Current
            {
                get 
                {
                    if (!iterationStarted || current == null)
                        throw new InvalidOperationException();
                    return current.Current;
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                return;
            }

            #endregion
        }

        internal class SingleIterator : XPath2NodeIterator
        {
            private XPathItem _item;            

            public SingleIterator(XPathItem item)
            {
                _item = item;
            }

            public override XPath2NodeIterator Clone()
            {
                return new SingleIterator(_item);
            }

            public override bool IsSingleIterator
            {
                get
                {
                    return true;
                }
            }

            protected override XPathItem NextItem()
            {
                if (CurrentPosition == -1)
                    return _item;
                return null;
            }

            public override XPath2NodeIterator CreateBufferedIterator()
            {
                return Clone();
            }
        }
                       
        internal class XQueryNodeIteratorDebugView
        {
            private XPath2NodeIterator iter;

            public XQueryNodeIteratorDebugView(XPath2NodeIterator iter)
            {
                this.iter = iter.Clone();
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public XPathItem[] Items
            {
                get
                {
                    List<XPathItem> res = new List<XPathItem>();
                    foreach (XPathItem item in iter)
                    {
                        if (res.Count == 10)
                            break;
                        res.Add(item.Clone());
                    }
                    return res.ToArray();
                }
            }

            public XPathItem Current
            {
                get
                {
                    return iter.curr;
                }
            }

            public int CurrentPosition
            {
                get
                {
                    return iter.pos;
                }
            }
        }

    }
}
