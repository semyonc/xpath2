//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
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

namespace Wmhelp.XPath2
{
    public class EmptyIterator : XPath2NodeIterator
    {
        public static EmptyIterator Shared = new EmptyIterator();

        private EmptyIterator()
        {
        }

        public override XPath2NodeIterator Clone()
        {
            return this;
        }

        public override bool IsFinished
        {
            get
            {
                return true;
            }
        }

        protected override XPathItem NextItem()
        {
            return null;
        }

        public override XPath2NodeIterator CreateBufferedIterator()
        {
            return this;
        }
    }
}
