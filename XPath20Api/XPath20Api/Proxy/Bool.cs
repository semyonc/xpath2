// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;

namespace Wmhelp.XPath2.Proxy
{
    internal sealed class Bool : ValueProxy
    {
        private readonly bool _value;

        public Bool(bool value)
        {
            _value = value;
        }

        public override int GetValueCode()
        {
            return BoolFactory.Code;
        }

        public override object Value
        {
            get 
            {
                return _value;
            }
        }

        protected override bool Eq(ValueProxy val)
        {
            return _value == ((Bool)val)._value;
        }

        protected override bool Gt(ValueProxy val)
        {
            throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:gt",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override bool TryGt(ValueProxy val, out bool res)
        {
            res = false;
            return false;
        }

        protected override ValueProxy Promote(ValueProxy val)
        {
            return new Bool(Convert.ToBoolean(val));
        }

        protected override ValueProxy Neg()
        {
            throw new XPath2Exception(Properties.Resources.UnaryOperatorNotDefined, "fn:unary-minus",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One));
        }

        protected override ValueProxy Add(ValueProxy val)
        {
            throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:add",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override ValueProxy Sub(ValueProxy val)
        {
            throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:sub",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override ValueProxy Mul(ValueProxy val)
        {
            throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:mul",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override ValueProxy Div(ValueProxy val)
        {
            throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:div",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override Integer IDiv(ValueProxy val)
        {
            throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:idiv",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override ValueProxy Mod(ValueProxy val)
        {
            throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:mod",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }
    }
}
