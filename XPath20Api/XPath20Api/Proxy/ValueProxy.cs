// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Wmhelp.XPath2.Proxy
{
    internal abstract class ValueProxy: IConvertible
    {
        public const int TYPES_MAX = 24;

        public abstract int GetValueCode();

        public abstract Object Value { get; }

        protected abstract bool Eq(ValueProxy val);

        protected virtual bool TryEq(ValueProxy val, out bool res)
        {
            res = false;
            if (val.GetValueCode() == GetValueCode())
            {
                res = Eq(val);
                return true;
            }
            return false;
        }

        protected abstract bool Gt(ValueProxy val);

        protected virtual bool TryGt(ValueProxy val, out bool res)
        {
            res = false;
            if (val.GetValueCode() == GetValueCode())
            {
                res = Gt(val);
                return true;
            }
            return false;
        }

        protected abstract ValueProxy Promote(ValueProxy val);

        protected abstract ValueProxy Neg();

        protected abstract ValueProxy Add(ValueProxy val);

        protected abstract ValueProxy Sub(ValueProxy val);

        protected abstract ValueProxy Mul(ValueProxy val);

        protected abstract ValueProxy Div(ValueProxy val);

        protected abstract Integer IDiv(ValueProxy val);

        protected abstract ValueProxy Mod(ValueProxy val);

        public override bool Equals(object obj)
        {
            if (obj is ValueProxy)
            {
                bool res;
                if (Eq(this, (ValueProxy)obj, out res))
                    return res;
            }
            return false;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public virtual bool IsNaN()
        {
            return false;
        }

        public virtual bool IsNumeric()
        {
            return false;
        }

        private static readonly Type dynamicValueType = typeof(ValueProxy);
        protected static readonly Dictionary<Type, ValueProxyFactory> valueFactory =
            new Dictionary<Type, ValueProxyFactory>();
        private static int[,] conv_t = null;

        public static ValueProxy New(object value)
        {
            if (value is ValueProxy)
                return (ValueProxy)value;
            try
            {
                return valueFactory[value.GetType()].Create(value);
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidCastException(String.Format(
                    "The type {0} is not registred in ValueProxy", value.GetType()));
            }
        }

        public static bool IsProxyType(Type type)
        {
            return dynamicValueType.IsAssignableFrom(type);
        }

        public static void AddFactory(IEnumerable<ValueProxyFactory> factories)
        {
            foreach (ValueProxyFactory curr in factories)
            {
                if (valueFactory.Count == TYPES_MAX)
                    throw new InvalidOperationException();
                valueFactory[curr.GetValueType()] = curr;
            }
            Bind();
        }

        private static void Bind()
        {
            conv_t = new int[TYPES_MAX, TYPES_MAX];
            for (int k = 0; k < TYPES_MAX; k++)
                for (int s = 0; s < TYPES_MAX; s++)
                    conv_t[k, s] = -2;
            List<ValueProxyFactory> list = new List<ValueProxyFactory>(valueFactory.Values);
            foreach (ValueProxyFactory curr in list)
            {
                foreach (ValueProxyFactory curr2 in list)
                    if (curr == curr2)
                        conv_t[curr.GetValueCode(), curr2.GetValueCode()] = 0;
                    else
                        conv_t[curr.GetValueCode(), curr2.GetValueCode()] = curr.Compare(curr2);
            }
        }

        public static bool IsNumeric(Type type)
        {
            ValueProxyFactory factory;
            if (valueFactory.TryGetValue(type, out factory))
                return factory.IsNumeric;
            return false;
        }

        public static Type GetType(Type type1, Type type2)
        {
            if (dynamicValueType.IsAssignableFrom(type1) || 
                dynamicValueType.IsAssignableFrom(type2))
                return dynamicValueType;
            ValueProxyFactory factory1;
            if (!valueFactory.TryGetValue(type1, out factory1))
                return typeof(System.Object);
            ValueProxyFactory factory2;
            if (!valueFactory.TryGetValue(type2, out factory2))
                return typeof(System.Object);
            switch (conv_t[factory1.GetValueCode(), factory2.GetValueCode()])
            {
                case -1:
                    return factory2.GetResultType();
                
                case 0:
                case 1:
                    return factory1.GetResultType();

                default:
                    return typeof(System.Object);
            }
        }

        public static bool Equals(ValueProxy val1, ValueProxy val2)
        {
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    return val2.Promote(val1).Eq(val2);

                case 0:
                    return val1.Eq(val2);

                case 1:
                    return val1.Eq(val1.Promote(val2));

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            return val2.Promote(val1).Eq(val2);
                        throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:eq",
                            new SequenceType(val1.GetType(), XmlTypeCardinality.One),
                            new SequenceType(val2.GetType(), XmlTypeCardinality.One));
                    }
            }
        }

        public static bool Eq(object val1, object val2, out bool res)
        {
            res = false;
            ValueProxy a;
            if (val1 is ValueProxy)
                a = (ValueProxy)val1;
            else
            {
                ValueProxyFactory f;
                if (!valueFactory.TryGetValue(val1.GetType(), out f))
                    return false;
                a = f.Create(val1);
            }
            ValueProxy b;
            if (val2 is ValueProxy)
                b = (ValueProxy)val2;
            else
            {
                ValueProxyFactory f;
                if (!valueFactory.TryGetValue(val2.GetType(), out f))
                    return false;
                b = f.Create(val2);
            }
            bool status;
            if (Eq(a, b, out status))
            {
                if (status)
                    res = true;
                return true;
            }
            return false;
        }

        public static bool Eq(ValueProxy val1, ValueProxy val2, out bool res)
        {
            res = false;
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    return val2.Promote(val1).TryEq(val2, out res);

                case 0:
                    return val1.TryEq(val2, out res);

                case 1:
                    return val1.TryEq(val1.Promote(val2), out res);

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            return val2.Promote(val1).TryEq(val2, out res);
                        return false;
                    }
            }
        }

        public static bool operator >(ValueProxy val1, ValueProxy val2)
        {
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    return val2.Promote(val1).Gt(val2);

                case 0:
                    return val1.Gt(val2);

                case 1:
                    return val1.Gt(val1.Promote(val2));

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            return val2.Promote(val1).Gt(val2);
                        throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:gt",
                            new SequenceType(val1.GetType(), XmlTypeCardinality.One),
                            new SequenceType(val2.GetType(), XmlTypeCardinality.One));
                    }
            }
        }

        public static bool Gt(object val1, object val2, out bool res)
        {
            res = false;
            ValueProxy a;
            if (val1 is ValueProxy)
                a = (ValueProxy)val1;
            else
            {
                ValueProxyFactory f;
                if (!valueFactory.TryGetValue(val1.GetType(), out f))
                    return false;
                a = f.Create(val1);
            }
            ValueProxy b;
            if (val2 is ValueProxy)
                b = (ValueProxy)val2;
            else
            {
                ValueProxyFactory f;
                if (!valueFactory.TryGetValue(val2.GetType(), out f))
                    return false;
                b = f.Create(val2);
            }
            bool status;
            if (Gt(a, b, out status))
            {
                if (status)
                    res = true;
                return true;
            }
            return false;
        }

        public static bool Gt(ValueProxy val1, ValueProxy val2, out bool res)
        {
            res = false;
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    return val2.Promote(val1).TryGt(val2, out res);

                case 0:
                    return val1.TryGt(val2, out res);

                case 1:
                    return val1.TryGt(val1.Promote(val2), out res);

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            return val2.Promote(val1).TryGt(val2, out res);
                        return false;
                    }
            }
        }

        public static bool operator <(ValueProxy val1, ValueProxy val2)
        {
            return (val2 > val1);
        }

        public static bool operator >=(ValueProxy val1, ValueProxy val2)
        {
            return (Equals(val1, val2) || val1 > val2);
        }


        public static bool operator <=(ValueProxy val1, ValueProxy val2)
        {
            return (Equals(val1, val2) || val1 < val2);
        }

        public static ValueProxy operator +(ValueProxy val1, ValueProxy val2)
        {
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    return val2.Promote(val1).Add(val2);

                case 0:
                    return val1.Add(val2);

                case 1:
                    return val1.Add(val1.Promote(val2));

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            return val2.Promote(val1).Add(val2);
                        throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:add",
                            new SequenceType(val1.GetType(), XmlTypeCardinality.One),
                            new SequenceType(val2.GetType(), XmlTypeCardinality.One));
                    }
            }
        }

        public static ValueProxy operator -(ValueProxy val)
        {
            return val.Neg();
        }

        public static ValueProxy operator -(ValueProxy val1, ValueProxy val2)
        {
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    return val2.Promote(val1).Sub(val2);

                case 0:
                    return val1.Sub(val2);

                case 1:
                    return val1.Sub(val1.Promote(val2));

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            return val2.Promote(val1).Sub(val2);
                        throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:sub",
                            new SequenceType(val1.GetType(), XmlTypeCardinality.One),
                            new SequenceType(val2.GetType(), XmlTypeCardinality.One));
                    }
            }
        }

        public static ValueProxy operator *(ValueProxy val1, ValueProxy val2)
        {
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    return val2.Promote(val1).Mul(val2);

                case 0:
                    return val1.Mul(val2);

                case 1:
                    return val1.Mul(val1.Promote(val2));

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            return val2.Promote(val1).Mul(val2);
                        throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:mul",
                            new SequenceType(val1.GetType(), XmlTypeCardinality.One),
                            new SequenceType(val2.GetType(), XmlTypeCardinality.One));
                    }
            }
        }

        public static ValueProxy operator /(ValueProxy val1, ValueProxy val2)
        {
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    return val2.Promote(val1).Div(val2);

                case 0:
                    return val1.Div(val2);

                case 1:
                    return val1.Div(val1.Promote(val2));

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            return val2.Promote(val1).Div(val2);
                        throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:div",
                            new SequenceType(val1.GetType(), XmlTypeCardinality.One),
                            new SequenceType(val2.GetType(), XmlTypeCardinality.One));
                    }
            }
        }

        public static ValueProxy operator %(ValueProxy val1, ValueProxy val2)
        {
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    return val2.Promote(val1).Mod(val2);

                case 0:
                    return val1.Mod(val2);

                case 1:
                    return val1.Mod(val1.Promote(val2));

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            return val2.Promote(val1).Mod(val2);
                        throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:mod",
                            new SequenceType(val1.GetType(), XmlTypeCardinality.One),
                            new SequenceType(val2.GetType(), XmlTypeCardinality.One));
                    }
            }
        }

        public static Integer op_IntegerDivide(ValueProxy val1, ValueProxy val2)
        {
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    return val2.Promote(val1).IDiv(val2);

                case 0:
                    return val1.IDiv(val2);

                case 1:
                    return val1.IDiv(val1.Promote(val2));

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            return val2.Promote(val1).IDiv(val2);
                        throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:idiv",
                            new SequenceType(val1.GetType(), XmlTypeCardinality.One),
                            new SequenceType(val2.GetType(), XmlTypeCardinality.One));
                    }
            }
        }

        public static ValueProxy Max(ValueProxy val1, ValueProxy val2)
        {
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    val1 = val2.Promote(val1);
                    break;

                case 0:
                    break;

                case 1:
                    val2 = val1.Promote(val2);
                    break;

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            val1 = val2.Promote(val1);
                        else
                            throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:gt",
                                new SequenceType(val1.GetType(), XmlTypeCardinality.One),
                                new SequenceType(val2.GetType(), XmlTypeCardinality.One));
                    }
                    break;
            }
            if (val2.IsNaN() || val2.Gt(val1))
                return val2;
            return val1;
        }

        public static ValueProxy Min(ValueProxy val1, ValueProxy val2)
        {
            switch (conv_t[val1.GetValueCode(), val2.GetValueCode()])
            {
                case -1:
                    val1 = val2.Promote(val1);
                    break;

                case 0:
                    break;

                case 1:
                    val2 = val1.Promote(val2);
                    break;

                default:
                    {
                        if (conv_t[val2.GetValueCode(), val1.GetValueCode()] == 1)
                            val1 = val2.Promote(val1);
                        else
                            throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:gt",
                                new SequenceType(val1.GetType(), XmlTypeCardinality.One),
                                new SequenceType(val2.GetType(), XmlTypeCardinality.One));
                    }
                    break;
            }
            if (val2.IsNaN() || val1.Gt(val2))
                return val2;
            return val1;
        }

        public static object Unwrap(object value)
        {
            ValueProxy proxy = value as ValueProxy;
            if (proxy != null)
                return proxy.Value;
            return value;
        }

        public static implicit operator ValueProxy(sbyte value)
        {
            return new Proxy.SByteProxy(value);
        }

        public static implicit operator ValueProxy(byte value)
        {
            return new Proxy.ByteProxy(value);
        }
       
        public static implicit operator ValueProxy(short value)
        {
            return new Proxy.Short(value);
        }

        public static implicit operator ValueProxy(ushort value)
        {
            return new Proxy.UShort(value);
        }

        public static implicit operator ValueProxy(int value)
        {
            return new Proxy.Int(value);
        }

        public static implicit operator ValueProxy(uint value)
        {
            return new Proxy.UInt(value);
        }
        
        public static implicit operator ValueProxy(long value)
        {
            return new Proxy.Long(value);
        }

        public static implicit operator ValueProxy(ulong value)
        {
            return new Proxy.ULong(value);
        }

        public static implicit operator ValueProxy(decimal value)
        {
            return new Proxy.DecimalProxy(value);
        }

        public static implicit operator ValueProxy(Integer value)
        {
            return new Proxy.IntegerProxy(value);
        }

        public static implicit operator ValueProxy(float value)
        {
            return new Proxy.Float(value);
        }

        public static implicit operator ValueProxy(double value)
        {
            return new Proxy.DoubleProxy(value);
        }

        public static implicit operator ValueProxy(string value)
        {
            return new Proxy.StringProxy(value);
        }

        public static explicit operator SByte(ValueProxy dv)
        {
            return (SByte)dv.Value;
        }

        public static explicit operator Byte(ValueProxy dv)
        {
            return (Byte)dv.Value;
        }

        public static explicit operator Int16(ValueProxy dv)
        {
            return (Int16)dv.Value;
        }

        public static explicit operator UInt16(ValueProxy dv)
        {
            return (UInt16)dv.Value;
        }

        public static explicit operator Int32(ValueProxy dv)
        {
            return (Int32)dv.Value;
        }

        public static explicit operator UInt32(ValueProxy dv)
        {
            return (UInt32)dv.Value;
        }

        public static explicit operator Int64(ValueProxy dv)
        {
            return (Int64)dv.Value;
        }

        public static explicit operator UInt64(ValueProxy dv)
        {
            return (UInt64)dv.Value;
        }

        public static explicit operator Decimal(ValueProxy dv)
        {
            return (Decimal)dv.Value;
        }

        public static explicit operator Integer(ValueProxy dv)
        {
            return (Integer)dv.Value;
        }

        public static explicit operator Single(ValueProxy dv)
        {
            return (Single)dv.Value;
        }

        public static explicit operator Double(ValueProxy dv)
        {
            return (Double)dv.Value;
        }

        #region IConvertible Members

        public virtual TypeCode GetTypeCode()
        {
            return Type.GetTypeCode(Value.GetType());
        }

        public virtual bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(Value, provider);
        }

        public virtual byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(Value, provider);
        }

        public virtual char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(Value, provider);
        }

        public virtual DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(Value, provider);
        }

        public virtual decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(Value, provider);
        }

        public virtual double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(Value, provider);
        }

        public virtual short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(Value, provider);
        }

        public virtual int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(Value, provider);
        }

        public virtual long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(Value, provider);
        }

        public virtual sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(Value, provider);
        }

        public virtual float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(Value, provider);
        }

        public virtual string ToString(IFormatProvider provider)
        {
            return Convert.ToString(Value, provider);
        }

        public virtual object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Value, conversionType, provider);
        }

        public virtual ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Value, provider);
        }

        public virtual uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Value, provider);
        }

        public virtual ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Value, provider);
        }

        #endregion

    }
}
