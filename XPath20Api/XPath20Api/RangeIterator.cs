// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

namespace Wmhelp.XPath2
{
    class RangeIterator: XPath2NodeIterator
    {
        private Integer _min;
        private Integer _max;
        private Integer _index;

        public RangeIterator(Integer min, Integer max)
        {
            _min = min;
            _max = max;
        }

        public override XPath2NodeIterator Clone()
        {
            return new RangeIterator(_min, _max);
        }

        public override int Count
        {
            get
            {
                Integer c = _max - _min + 1;
                return (int)Math.Max(0, (decimal)c);
            }
        }

        protected override void Init()
        {
            _index = _min;
        }

        protected override XPathItem NextItem()
        {
            if (_index <= _max)
                return new XPath2Item(_index++);
            return null;
        }

        public override XPath2NodeIterator CreateBufferedIterator()
        {
            return Clone();
        }

        public override bool IsRange
        {
            get
            {
                return true;
            }
        }
    }
}
