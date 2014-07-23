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
    public class GMonthValue: DateTimeValueBase
    {
        public GMonthValue(DateTimeOffset value)
            : base(false, value)
        {
        }

        public GMonthValue(DateTime value)
            : base(false, value)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IsLocal)
                sb.Append(Value.ToString("--MM"));
            else
                if (Value.Offset == TimeSpan.Zero)
                    sb.Append(Value.ToString("--MM'Z'"));
                else
                    sb.Append(Value.ToString("--MMzzz"));
            return sb.ToString();
        }

        public static GMonthValue Parse(string text)
        {
            DateTimeOffset dateTimeOffset;
            DateTime dateTime;
            text = "2008" + text.Trim();
            if (text.EndsWith("Z"))
            {
                if (!DateTimeOffset.TryParseExact(text.Substring(0, text.Length - 1), "yyyy--MM",
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTimeOffset))
                    throw new XPath2Exception("", Properties.Resources.InvalidFormat, text, "xs:gMonth");
                return new GMonthValue(dateTimeOffset);
            }
            else
            {
                if (DateTime.TryParseExact(text, "yyyy--MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return new GMonthValue(dateTime);
                if (!DateTimeOffset.TryParseExact(text, "yyyy--MMzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset))
                    throw new XPath2Exception("", Properties.Resources.InvalidFormat, text, "xs:gMonth");
                return new GMonthValue(dateTimeOffset);
            }
        }    

    }
}
