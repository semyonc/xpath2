// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Wmhelp.XPath2.Value
{
    public class GYearMonthValue : DateTimeValueBase
    {
        public GYearMonthValue(bool sign, DateTimeOffset value)
            : base(sign, value)
        {
        }

        public GYearMonthValue(bool sign, DateTime value)
            : base(sign, value)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (S)
                sb.Append('-');
            if (IsLocal)
                sb.Append(Value.ToString("yyyy-MM"));
            else
                if (Value.Offset == TimeSpan.Zero)
                    sb.Append(Value.ToString("yyyy-MM'Z'"));
                else
                    sb.Append(Value.ToString("yyyy-MMzzz"));
            return sb.ToString();
        }

        private static string[] GYearMonthFormats = new string[] {             
            "yyyy-MM",             
            "'-'yyyy-MM"        
        };

        private static string[] GYearMonthOffsetFormats = new string[] {             
            "yyyy-MMzzz", 
            "'-'yyyy-MMzzz"        
        };

        public static GYearMonthValue Parse(string text)
        {
            DateTimeOffset dateTimeOffset;
            DateTime dateTime;
            text = text.Trim();
            bool s = text.StartsWith("-");
            if (text.EndsWith("Z"))
            {
                if (!DateTimeOffset.TryParseExact(text.Substring(0, text.Length - 1), GYearMonthFormats,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTimeOffset))
                    throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:gYearMonth");
                return new GYearMonthValue(s, dateTimeOffset);
            }
            else
            {
                if (DateTime.TryParseExact(text, GYearMonthFormats, CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out dateTime))
                    return new GYearMonthValue(s, dateTime);
                if (!DateTimeOffset.TryParseExact(text, GYearMonthOffsetFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTimeOffset))
                    throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:gYearMonth");
                return new GYearMonthValue(s, dateTimeOffset);
            }
        }    
    }
}
