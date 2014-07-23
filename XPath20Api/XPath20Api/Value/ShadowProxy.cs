// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Wmhelp.XPath2.Proxy;

namespace Wmhelp.XPath2.Value
{
    internal sealed class ShadowProxy : ValueProxy
    {

        private object _value;
        private int _valueCode;
        private bool _isNumeric;

        public ShadowProxy(ValueProxy proxy)
        {
            _value = proxy.Value;
            _valueCode = proxy.GetValueCode();
            _isNumeric = proxy.IsNumeric();
        }


        public override int GetValueCode()
        {
            return _valueCode;
        }

        public override bool IsNumeric()
        {
            return _isNumeric;
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
            throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:eq",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override bool Gt(ValueProxy val)
        {
            throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:gt",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override ValueProxy Promote(ValueProxy val)
        {
            throw new NotImplementedException();
        }

        protected override ValueProxy Neg()
        {
            throw new XPath2Exception("", Properties.Resources.UnaryOperatorNotDefined, "fn:unary-minus",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One));
        }

        protected override ValueProxy Add(ValueProxy val)
        {
            throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:add",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override ValueProxy Sub(ValueProxy val)
        {
            throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:sub",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override ValueProxy Mul(ValueProxy val)
        {
            switch (val.GetValueCode())
            {
                case YearMonthDurationValue.ProxyValueCode:
                    return new YearMonthDurationValue.Proxy(
                        YearMonthDurationValue.Multiply((YearMonthDurationValue)val.Value, Convert.ToDouble(_value)));
                case DayTimeDurationValue.ProxyValueCode:
                    return new DayTimeDurationValue.Proxy(
                        DayTimeDurationValue.Multiply((DayTimeDurationValue)val.Value, Convert.ToDouble(_value)));
                default:
                    throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:mul",
                        new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                        new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
            }
        }

        protected override ValueProxy Div(ValueProxy val)
        {
            throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:div",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override Integer IDiv(ValueProxy val)
        {
            throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:idiv",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }

        protected override ValueProxy Mod(ValueProxy val)
        {
            throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:mod",
                new SequenceType(Value.GetType(), XmlTypeCardinality.One),
                new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
        }
    }
}
