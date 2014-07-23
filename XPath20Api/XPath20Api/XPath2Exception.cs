//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Wmhelp.XPath2
{
    public class XPath2Exception: Exception
    {
        public string ErrorCode { get; internal set; }

		protected XPath2Exception (SerializationInfo info, StreamingContext context) : base (info, context) {}

		public XPath2Exception (string message, Exception innerException) : base (message, innerException) {}

        internal XPath2Exception(string errorCode, string message) : base(message, null) 
        {
            ErrorCode = errorCode;
        }

        internal XPath2Exception(string errorCode, string message, params object[] args) : base(String.Format(message, args), null) 
        { 
            ErrorCode = errorCode;  
        }
    }
}
