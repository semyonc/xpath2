// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

using TimSort;
using System.Runtime.InteropServices;
using System.Collections;

namespace Wmhelp.XPath2
{
    class ItemSet: IEnumerable, IEnumerable<XPathItem>  
    {
        private int _size;
        private XPathItem[] _items;

        private static XPathItem[] emptySet = new XPathItem[0];

        public ItemSet()
        {
            _items = emptySet;
        }

        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int num = (_items.Length == 0) ? 4 : (_items.Length * 2);
                if (num < min)
                    num = min;
                Capacity = num;
            }
        }

        public int Capacity
        {
            get
            {
                return _items.Length;
            }
            set
            {
                if (value < _size)
                    throw new ArgumentOutOfRangeException();
                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        XPathItem[] destinationArray = new XPathItem[value];
                        if (_size > 0)
                            Array.Copy(_items, 0, destinationArray, 0, this._size);
                        _items = destinationArray;
                    }
                    else
                        _items = emptySet;
                }
            }
        }

        public int Count { get { return _size; } }

        public XPathItem this[int index]
        {
            get
            {
                if (index >= _size)
                    throw new ArgumentOutOfRangeException();
                return _items[index];
            }
            set
            {
                if (index >= _size)
                    throw new ArgumentOutOfRangeException();
                _items[index] = value;
            }
        }

        public void Add(XPathItem item)
        {
            if (_size == _items.Length)
                EnsureCapacity(_size + 1);
            _items[_size++] = item;
        }

        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size);
                _size = 0;
            }
        }

        public bool Completed { get; set; }

        public void Sort()
        {
            TimSort<XPathItem>.sort(_items, 0, _size, new XPathComparer());
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<XPathItem>, IDisposable, IEnumerator
        {
            private ItemSet itemSet;
            private int index;
            private XPathItem current;
            
            internal Enumerator(ItemSet set)
            {
                itemSet = set;
                index = 0;
                current = null;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (index < itemSet._size)
                {
                    current = itemSet._items[index];
                    index++;
                    return true;
                }
                index = itemSet._size + 1;
                current = null;
                return false;
            }

            public XPathItem Current
            {
                get
                {
                    return current;
                }
            }
            
            object IEnumerator.Current
            {
                get
                {
                    return current;
                }
            }
            
            void IEnumerator.Reset()
            {            
                index = 0;
                current = null;
            }
        }

        #region IEnumerable<XPathItem> Members

        public IEnumerator<XPathItem> GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion
    }
}
