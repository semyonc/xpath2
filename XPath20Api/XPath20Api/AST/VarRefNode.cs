// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Wmhelp.XPath2.AST
{
    sealed class VarRefNode: AbstractNode
    {
        private Tokenizer.VarName _varName;
        private NameBinder.ReferenceLink _varRef;

        public NameBinder.ReferenceLink VarRef { get { return _varRef; } }

        public VarRefNode(XPath2Context context, Tokenizer.VarName varRef)
            : base(context)
        {
            _varName = varRef;
        }

        public override void Bind()
        {
            XmlQualifiedName qname = QNameParser.Parse(_varName.ToString(),
                Context.NamespaceManager, Context.NameTable);
            _varRef = Context.NameBinder.VarIndexByName(qname);            
        }

        public override object Execute(IContextProvider provider, object[] dataPool)
        {
            return _varRef.Get(dataPool);
        }
    }
}
