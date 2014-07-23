// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

using Wmhelp.XPath2.Proxy;

namespace Wmhelp.XPath2.Value
{
    public class DateValue: DateTimeValueBase, IXmlConvertable
    {
        public const int ProxyValueCode = 13;

        public DateValue(bool sign, DateTimeOffset value)
            : base(sign, value)
        {
        }

        public DateValue(bool sign, DateTime value)
            : base(sign, value)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (S)
                sb.Append("-");
            if (IsLocal)
                sb.Append(Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            else if (Value.Offset == TimeSpan.Zero)
                sb.Append(Value.ToString("yyyy-MM-dd'Z'", CultureInfo.InvariantCulture));
            else
                sb.Append(Value.ToString("yyyy-MM-ddzzz", CultureInfo.InvariantCulture));
            return sb.ToString();
        }

        private static string[] DateTimeFormats = new string[] {
            "yyyy-MM-dd", 
            "'-'yyyy-MM-dd" 
        };

        private static string[] DateTimeOffsetFormats = new string[] {
            "yyyy-MM-ddzzz", 
            "'-'yyyy-MM-ddzzz" 
        };

        public static DateValue Parse(string text)
        {
            DateTimeOffset dateTimeOffset;
            DateTime dateTime;
            bool s = text.StartsWith("-");
            if (text.EndsWith("Z"))
            {
                if (!DateTimeOffset.TryParseExact(text.Substring(0, text.Length - 1), DateTimeFormats,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | 
                        DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out dateTimeOffset))
                    throw new XPath2Exception("", Properties.Resources.InvalidFormat, text, "xs:date");
                return new DateValue(s, dateTimeOffset);
            }
            else
            {
                if (DateTime.TryParseExact(text, DateTimeFormats, CultureInfo.InvariantCulture, 
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out dateTime))
                    return new DateValue(s, dateTime);
                if (!DateTimeOffset.TryParseExact(text, DateTimeOffsetFormats, CultureInfo.InvariantCulture, 
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out dateTimeOffset))
                    throw new XPath2Exception("", Properties.Resources.InvalidFormat, text, "xs:date");
                return new DateValue(s, dateTimeOffset);
            }
        }

        public decimal ToJulianInstant()
        {
            int sign = S ? -1 : 1;
            DateTime dt = Value.Date;
            int julianDay = DateTimeValue.GetJulianDayNumber(sign * dt.Year, dt.Month, dt.Day);
            long julianSecond = julianDay * (24L * 60L * 60L);
            return (decimal)julianSecond;
        }

        public static DateValue Add(DateValue dat, YearMonthDurationValue duration)
        {
            try
            {

                Calendar calender = CultureInfo.InvariantCulture.Calendar;
                DateTime dt = dat.Value.DateTime;
                int year = dat.S ? -dt.Year : dt.Year - 1;
                int m = (dt.Month - 1) + duration.Months;
                year = year + duration.Years + m / 12;
                if (year >= 0)
                    year = year + 1;
                m = m % 12;
                if (m < 0)
                {
                    m += 12;
                    year -= 1;
                }
                m++;
                int day = Math.Min(dt.Day, calender.GetDaysInMonth(Math.Abs(year), m));
                if (year < 0)
                    dt = new DateTime(-year, m, day);
                else
                    dt = new DateTime(year, m, day);
                if (dat.IsLocal)
                    return new DateValue(year < 0, dt);
                else
                    return new DateValue(year < 0, new DateTimeOffset(dt, dat.Value.Offset));
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new XPath2Exception("FODT0001", Properties.Resources.FODT0001);
            }
        }

        public static DateValue Add(DateValue dat, DayTimeDurationValue duration)
        {
            try
            {

                decimal seconds = (decimal)duration.LowPartValue.Ticks / TimeSpan.TicksPerSecond;
                decimal julian = dat.ToJulianInstant();
                julian += seconds;
                DateTimeValue dt = DateTimeValue.CreateFromJulianInstant(julian);
                if (dat.IsLocal)
                    return new DateValue(dt.S, dt.Value.Date);
                else
                    return new DateValue(dt.S, new DateTimeOffset(dt.Value.Date, dat.Value.Offset));
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new XPath2Exception("FODT0001", Properties.Resources.FODT0001);
            }
        }

        private static DayTimeDurationValue Sub(DateValue dat1, DateValue dat2)
        {
            try
            {
                TimeSpan ts1;
                TimeSpan ts2;
                if (dat1.IsLocal && dat2.IsLocal)
                {
                    ts1 = dat1.Value.DateTime - DateTime.MinValue;
                    ts2 = dat2.Value.DateTime - DateTime.MinValue;
                }
                else
                {
                    ts1 = dat1.Value.ToUniversalTime().DateTime - DateTime.MinValue;
                    ts2 = dat2.Value.ToUniversalTime().DateTime - DateTime.MinValue;
                }
                if (dat1.S)
                    ts1 = -ts1;
                if (dat2.S)
                    ts2 = -ts2;
                return new DayTimeDurationValue(ts1 - ts2);
            }
            catch (OverflowException)
            {
                throw new XPath2Exception("FODT0001", Properties.Resources.FODT0001);
            }
        }

        internal class ProxyFactory : ValueProxyFactory
        {
            public override ValueProxy Create(object value)
            {
                return new Proxy((DateValue)value);
            }

            public override int GetValueCode()
            {
                return ProxyValueCode;
            }

            public override Type GetValueType()
            {
                return typeof(DateValue);
            }

            public override bool IsNumeric
            {
                get { return false; }
            }

            public override int Compare(ValueProxyFactory other)
            {
                return 0;
            }
        }


        internal class Proxy : ValueProxy
        {
            private DateValue _value;

            public Proxy(DateValue value)
            {
                _value = value;
            }

            public override int GetValueCode()
            {
                return ProxyValueCode;
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
                if (val.GetValueCode() != ProxyValueCode)
                    throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:eq",
                        new SequenceType(_value.GetType(), XmlTypeCardinality.One),
                        new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
                return _value.Equals(((Proxy)val)._value);
            }

            protected override bool Gt(ValueProxy val)
            {
                if (val.GetValueCode() != ProxyValueCode)
                    throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:gt",
                        new SequenceType(_value.GetType(), XmlTypeCardinality.One),
                        new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
                return ((IComparable)_value).CompareTo(((Proxy)val)._value) > 0;
            }

            protected override ValueProxy Promote(ValueProxy value)
            {
                throw new NotImplementedException();
            }

            protected override ValueProxy Neg()
            {
                throw new XPath2Exception("", Properties.Resources.UnaryOperatorNotDefined, "fn:unary-minus",
                    new SequenceType(_value.GetType(), XmlTypeCardinality.One));
            }

            protected override ValueProxy Add(ValueProxy value)
            {
                switch (value.GetValueCode())
                {
                    case YearMonthDurationValue.ProxyValueCode:
                        return new Proxy(DateValue.Add(_value, (YearMonthDurationValue)value.Value));
                    case DayTimeDurationValue.ProxyValueCode:
                        return new Proxy(DateValue.Add(_value, (DayTimeDurationValue)value.Value));

                    default:
                        throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:add",
                            new SequenceType(_value.GetType(), XmlTypeCardinality.One),
                            new SequenceType(value.Value.GetType(), XmlTypeCardinality.One));
                }
            }

            protected override ValueProxy Sub(ValueProxy value)
            {
                switch (value.GetValueCode())
                {
                    case DateValue.ProxyValueCode:
                        return new DayTimeDurationValue.Proxy(DateValue.Sub(_value, (DateValue)value.Value));
                    case YearMonthDurationValue.ProxyValueCode:
                        return new Proxy(DateValue.Add(_value, -(YearMonthDurationValue)value.Value));
                    case DayTimeDurationValue.ProxyValueCode:
                        return new Proxy(DateValue.Add(_value, -(DayTimeDurationValue)value.Value));

                    default:
                        throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:sub",
                            new SequenceType(_value.GetType(), XmlTypeCardinality.One),
                            new SequenceType(value.Value.GetType(), XmlTypeCardinality.One));
                }
            }

            protected override ValueProxy Mul(ValueProxy value)
            {
                throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:mul",
                    new SequenceType(_value.GetType(), XmlTypeCardinality.One),
                    new SequenceType(value.Value.GetType(), XmlTypeCardinality.One));
            }

            protected override ValueProxy Div(ValueProxy value)
            {
                throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:div",
                    new SequenceType(_value.GetType(), XmlTypeCardinality.One),
                    new SequenceType(value.Value.GetType(), XmlTypeCardinality.One));
            }

            protected override Integer IDiv(ValueProxy value)
            {
                throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:idiv",
                    new SequenceType(_value.GetType(), XmlTypeCardinality.One),
                    new SequenceType(value.Value.GetType(), XmlTypeCardinality.One));
            }

            protected override ValueProxy Mod(ValueProxy value)
            {
                throw new XPath2Exception("", Properties.Resources.BinaryOperatorNotDefined, "op:mod",
                    new SequenceType(_value.GetType(), XmlTypeCardinality.One),
                    new SequenceType(value.Value.GetType(), XmlTypeCardinality.One));
            }
        }
        
        #region IXmlConvertable Members

        object IXmlConvertable.ValueAs(SequenceType type, XmlNamespaceManager nsmgr)
        {
            switch (type.TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.Date:
                    return this;

                case XmlTypeCode.DateTime:
                    if (IsLocal)
                        return new DateTimeValue(S, Value.Date);
                    else
                        return new DateTimeValue(S, new DateTimeOffset(Value.Date, Value.Offset));

                case XmlTypeCode.GYear:
                    return ToGYear();

                case XmlTypeCode.GYearMonth:
                    return ToGYearMonth();

                case XmlTypeCode.GMonth:
                    return ToGMonth();

                case XmlTypeCode.GMonthDay:
                    return ToGMonthDay();

                case XmlTypeCode.GDay:
                    return ToGDay();

                case XmlTypeCode.String:
                    return ToString();

                case XmlTypeCode.UntypedAtomic:
                    return new UntypedAtomic(ToString());

                default:
                    throw new InvalidCastException();
            }
        }

        #endregion
    }
}
