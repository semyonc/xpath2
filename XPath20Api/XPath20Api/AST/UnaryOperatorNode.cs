﻿// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;

namespace Wmhelp.XPath2.AST
{
    internal delegate object UnaryOperator(IContextProvider provider, object arg);

    class UnaryOperatorNode: AbstractNode
    {
        protected UnaryOperator _unaryOper;
        private XPath2ResultType _resultType;

        public UnaryOperatorNode(XPath2Context context, UnaryOperator action, object node, XPath2ResultType resultType)
            : base(context)
        {
            _unaryOper = action;
            _resultType = resultType;
            Add(node);
        }

        public override object Execute(IContextProvider provider, object[] dataPool)
        {
            return _unaryOper(provider, this[0].Execute(provider, dataPool));
        }

        public override XPath2ResultType GetReturnType(object[] dataPool)
        {
            return _resultType;
        }
    }
}
