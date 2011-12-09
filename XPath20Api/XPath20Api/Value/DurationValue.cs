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
    public class DurationValue: IXmlConvertable
    {
        public const int ProxyValueCode = 15;

        public DurationValue(TimeSpan hi, TimeSpan low)
        {
            HighPartValue = hi;
            LowPartValue = low;
        }

        public TimeSpan LowPartValue { get; private set; }
        public TimeSpan HighPartValue { get; private set; }

        public override bool Equals(object obj)
        {
            DurationValue other = obj as DurationValue;
            if (other != null)
            {
                if (other.IsZero && IsZero)
                    return true;
                return HighPartValue == other.HighPartValue &&
                    LowPartValue == other.LowPartValue;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return LowPartValue.GetHashCode() ^ HighPartValue.GetHashCode();
        }

        public bool IsZero
        {
            get
            {
                return LowPartValue == TimeSpan.Zero &&
                    HighPartValue == TimeSpan.Zero;
            }
        }

        public int Years
        {
            get
            {
                return HighPartValue.Days / 365;
            }
        }

        public int Months
        {
            get
            {
                return (int)Math.Round((double)(HighPartValue.Days - Years * 365) / 30);
            }
        }

        public int Days
        {
            get
            {
                return LowPartValue.Days;
            }
        }

        public int Hours
        {
            get
            {
                return LowPartValue.Hours;
            }
        }

        public int Minutes
        {
            get
            {
                return LowPartValue.Minutes;
            }
        }

        public int Seconds
        {
            get
            {
                return LowPartValue.Seconds;
            }
        }

        public int Milliseconds
        {
            get
            {
                return LowPartValue.Milliseconds;
            }
        }

        public int TotalDays
        {
            get
            {
                return HighPartValue.Days + LowPartValue.Days;
            }
        }

        public int TotalMonths
        {
            get
            {
                return Years * 12 + Months;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(10);
            if (HighPartValue < TimeSpan.Zero || LowPartValue < TimeSpan.Zero)
                sb.Append('-');
            sb.Append('P');
            if (Years != 0)
            {
                sb.Append(Math.Abs(Years));
                sb.Append('Y');
            }
            if (Months != 0)
            {
                sb.Append(Math.Abs(Months));
                sb.Append('M');
            }
            if (Days != 0)
            {
                sb.Append(Math.Abs(Days));
                sb.Append("D");
            }
            if (Hours != 0 || Minutes != 0 || Seconds != 0 || Milliseconds != 0)
            {
                sb.Append("T");
                if (Hours != 0)
                {
                    sb.Append(Math.Abs(Hours));
                    sb.Append('H');
                }
                if (Minutes != 0)
                {
                    sb.Append(Math.Abs(Minutes));
                    sb.Append('M');
                }
                if (Seconds != 0 || Milliseconds != 0)
                {
                    sb.Append(Math.Abs(Seconds));
                    if (Milliseconds != 0)
                    {
                        sb.Append('.');
                        sb.Append(Math.Abs(Milliseconds).ToString().PadLeft(3, '0'));
                    }
                    sb.Append('S');
                }
            }
            if (sb.Length == 1)
                return ZeroStringValue();
            return sb.ToString();
        }

        public virtual string ZeroStringValue()
        {
            return "PT0S";
        }

        public static DurationValue Parse(string text)
        {
            int year = -1;
            int month = -1;
            int day = -1;
            int h = -1;
            int m = -1;
            int sec = 0;
            int ms = 0;
            int value = 0;
            StringTokenizer tok = new StringTokenizer(text.Trim());
            tok.NextToken();
            bool s = false;
            if (tok.Token == '-')
            {                
                s = true;
                tok.NextToken();
            }
            if (tok.Token != 'P')
                throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
            if (tok.NextToken() == StringTokenizer.TokenInt)
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (tok.Token == 0)
                        break;
                    if (tok.Token != StringTokenizer.TokenInt)
                        goto ParseDayTimeFraction;
                    value = Int32.Parse(tok.Value);
                    switch (tok.NextToken())
                    {
                        case 'Y':
                            if (year != -1)
                                throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
                            year = value;
                            tok.NextToken();
                            break;

                        case 'M':
                            if (month != -1)
                                throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
                            month = value;
                            tok.NextToken();
                            break;

                        case 'D':
                            if (day != -1)
                                throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
                            day = value;
                            tok.NextToken();
                            break;

                        default:
                            throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
                    }
                }
                if (tok.Token == 0)
                    goto Complete;
            }
        ParseDayTimeFraction:
            if (tok.Token != 'T' || tok.NextToken() != StringTokenizer.TokenInt)
                throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
            for (int i = 1; i <= 3; i++)
            {
                if (tok.Token == 0)
                    goto Complete;
                if (tok.Token != StringTokenizer.TokenInt)
                    throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
                value = Int32.Parse(tok.Value);
                switch (tok.NextToken())
                {                    
                    case 'H':
                        if (h != -1)
                            throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
                        h = value;
                        tok.NextToken();
                        break;
                    
                    case 'M':
                        if (m != -1)
                            throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
                        m = value;
                        tok.NextToken();
                        break;

                    default:
                        sec = value;
                        if (tok.Token == '.')
                        {
                            if (tok.NextToken() != StringTokenizer.TokenInt)
                                throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
                            ms = Int32.Parse(tok.Value.PadRight(3, '0'));
                            tok.NextToken();
                        }
                        if (tok.Token != 'S')
                            throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
                        goto Complete;
                }
            }
        Complete:
            if (tok.NextToken() != 0)
                throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:duration");
            if (year == -1)
                year = 0;
            if (month == -1)
                month = 0;
            if (day == -1)
                day = 0;
            if (h == -1)
                h = 0;
            if (m == -1)
                m = 0;
            TimeSpan high = new TimeSpan(year * 365 + month / 12 * 365 + month % 12 * 30, 0, 0, 0);
            TimeSpan low = new TimeSpan(day, h, m, sec, ms);
            if (s)
                return new DurationValue(-high, -low);
            else
                return new DurationValue(high, low);
        }

        #region IXmlConvertable Members

        public object ValueAs(SequenceType type, XmlNamespaceManager nsmgr)
        {
            switch (type.TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.Duration:
                    return this;

                case XmlTypeCode.YearMonthDuration:
                    return new YearMonthDurationValue(HighPartValue);

                case XmlTypeCode.DayTimeDuration:
                    return new DayTimeDurationValue(LowPartValue);

                case XmlTypeCode.String:
                    return ToString();

                case XmlTypeCode.UntypedAtomic:
                    return new UntypedAtomic(ToString());

                default:
                    throw new InvalidCastException();
            }
        }

        #endregion

        internal class ProxyFactory : ValueProxyFactory
        {

            public override ValueProxy Create(object value)
            {
                return new Proxy((DurationValue)value);
            }

            public override int GetValueCode()
            {
                return ProxyValueCode;
            }

            public override Type GetValueType()
            {
                return typeof(DurationValue);
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
            private DurationValue _value;

            public Proxy(DurationValue value)
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
                return _value.Equals(val.Value);
            }

            protected override bool Gt(ValueProxy val)
            {
                throw new XPath2Exception(Properties.Resources.BinaryOperatorNotDefined, "op:gt",
                    new SequenceType(_value.GetType(), XmlTypeCardinality.One),
                    new SequenceType(val.Value.GetType(), XmlTypeCardinality.One));
            }

            protected override ValueProxy Promote(ValueProxy val)
            {
                throw new NotImplementedException();
            }

            protected override ValueProxy Neg()
            {
                throw new XPath2Exception(Properties.Resources.UnaryOperatorNotDefined, "fn:unary-minus",
                    new SequenceType(_value.GetType(), XmlTypeCardinality.One));
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
}
