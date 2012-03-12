﻿// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Wmhelp.XPath2.AST
{
    sealed class IfNode: AbstractNode
    {
        public IfNode(XPath2Context context, object cond, object thenBranch, object elseBranch)
            : base(context)
        {
            Add(cond);
            Add(thenBranch);
            Add(elseBranch);
        }

        public override object Execute(IContextProvider provider, object[] dataPool)
        {
            if (CoreFuncs.BooleanValue(this[0].Execute(provider, dataPool)) == CoreFuncs.True)
                return this[1].Execute(provider, dataPool);
            else
                return this[2].Execute(provider, dataPool);
        }
    }
}