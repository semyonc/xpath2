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
    public class GDayValue: DateTimeValueBase
    {
        public GDayValue(DateTimeOffset value)
            : base(false, value)
        {
        }

        public GDayValue(DateTime value)
            : base(false, value)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IsLocal)
                sb.Append(Value.ToString("---dd"));
            else
                if (Value.Offset == TimeSpan.Zero)
                    sb.Append(Value.ToString("---dd'Z'"));
                else    
                    sb.Append(Value.ToString("---ddzzz"));
            return sb.ToString();            
        }


        public static GDayValue Parse(string text)
        {
            DateTimeOffset dateTimeOffset;
            DateTime dateTime;
            text = "2008" + text.Trim();
            if (text.EndsWith("Z"))
            {
                if (!DateTimeOffset.TryParseExact(text.Substring(0, text.Length - 1), "yyyy---dd",
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTimeOffset))
                    throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:gDay");
                return new GDayValue(dateTimeOffset);
            }
            else
            {
                if (DateTime.TryParseExact(text, "yyyy---dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return new GDayValue(dateTime);
                if (!DateTimeOffset.TryParseExact(text, "yyyy---ddzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset))
                    throw new XPath2Exception(Properties.Resources.InvalidFormat, text, "xs:gDay");
                return new GDayValue(dateTimeOffset);
            }
        }    
    }
}
