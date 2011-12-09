// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;

namespace Wmhelp.XPath2.Proxy
{
    internal class StringProxy : ValueProxy
    {
        private readonly String _value;

        public StringProxy(String value)
        {
            _value = value;
        }

        public override int GetValueCode()
        {
            return StringProxyFactory.Code;
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
            return String.CompareOrdinal(_value, ((StringProxy)val)._value) == 0;
        }

        protected override bool Gt(ValueProxy val)
        {
            return String.CompareOrdinal(_value, ((StringProxy)val)._value) > 0;
        }

        protected override ValueProxy Promote(ValueProxy val)
        {
            return new StringProxy(Convert.ToString(val));
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

        public override TypeCode GetTypeCode()
        {
            return TypeCode.String;
        }

        public override bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(_value, provider);
        }

        public override byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(_value, provider);
        }

        public override char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(_value, provider);
        }

        public override DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(_value, provider);
        }

        public override decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(_value, provider);
        }

        public override double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(_value, provider);
        }

        public override short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(_value, provider);
        }

        public override int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(_value, provider);
        }

        public override long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(_value, provider);
        }

        public override sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(_value, provider);
        }

        public override float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(_value, provider);
        }

        public override string ToString(IFormatProvider provider)
        {
            return Convert.ToString(_value, provider);
        }

        public override object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(_value, conversionType, provider);
        }

        public override ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(_value, provider);
        }

        public override uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(_value, provider);
        }

        public override ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(_value, provider);
        }
    }
}
