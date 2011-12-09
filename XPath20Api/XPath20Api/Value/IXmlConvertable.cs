// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml;

namespace Wmhelp.XPath2.Value
{
    public interface IXmlConvertable
    {
        object ValueAs(SequenceType type, XmlNamespaceManager nsmgr);
    }
}
