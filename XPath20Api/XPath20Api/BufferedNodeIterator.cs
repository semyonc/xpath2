// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Threading;
using System.Diagnostics;

namespace Wmhelp.XPath2
{
    public sealed class BufferedNodeIterator : XPath2NodeIterator
    {
        private List<XPathItem> buffer;
        private XPath2NodeIterator src;

        [DebuggerStepThrough]
        private BufferedNodeIterator()
        {
        }

        public BufferedNodeIterator(XPath2NodeIterator src)
            : this(src, true)
        {
        }

        public BufferedNodeIterator(XPath2NodeIterator src, bool clone)
        {
            this.src = clone ? src.Clone() : src;
            buffer = new List<XPathItem>();
        }

        public override int Count
        {
            get
            {
                if (src.IsFinished)
                    return buffer.Count;
                return base.Count;
            }
        }

        public override bool IsSingleIterator
        {
            get
            {
                if (buffer.Count > 1)
                    return false;
                else
                {
                    if (src.IsFinished && buffer.Count == 1)
                        return true;
                    return base.IsSingleIterator;
                }
            }
        }

        public void Fill()
        {
            if (!src.IsFinished)
            {
                while (src.MoveNext())
                    buffer.Add(src.Current.Clone());
            }
        }

        public static BufferedNodeIterator Preload(XPath2NodeIterator baseIter)
        {
            BufferedNodeIterator res = new BufferedNodeIterator(baseIter);
            res.Fill();
            return res;
        }

        [DebuggerStepThrough]
        public override XPath2NodeIterator Clone()
        {
            BufferedNodeIterator clone = new BufferedNodeIterator();
            clone.src = src;
            clone.buffer = buffer;
            return clone;
        }

        protected override XPathItem NextItem()
        {
            int index = CurrentPosition + 1;
            if (index < buffer.Count)
                return buffer[index];
            else
            {
                if (!src.IsFinished)
                {
                    if (src.MoveNext())
                    {
                        buffer.Add(src.Current.Clone());
                        return src.Current;
                    }
                }
                return null;
            }
        }

        public override void ResetSequentialPosition()
        {
            if (!src.IsFinished)
                src.ResetSequentialPosition();
        }

        public override XPath2NodeIterator CreateBufferedIterator()
        {
            return Clone();
        }
    }
}
