using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wmhelp.XPath2
{
    public class Value 
    {
        protected object data;

        public Value(object data)
        {
            this.data = data;
        }

        public override string ToString()
        {
            return data.ToString();
        }

        public object Data
        {
            get
            {
                return data;
            }
        }

        public virtual object Clone()
        {
            return new Value(data);
        }

        public override bool Equals(object obj)
        {
            if (obj is Value)
            {
                Value value = (Value)obj;
                return data.Equals(value.data);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }
    }

    public class Literal : Value
    {
        public Char Quote { get; private set; }

        public Literal(object data)
            : base(data)
        {
        }

        public Literal(object data, char quote)
            : base(data)
        {
            Quote = quote;
        }

        public override string ToString()
        {
            return "'" + data.ToString() + "'";
        }

        new public string Data
        {
            get
            {
                return (string)data;
            }
        }

        public override object Clone()
        {
            return new Literal(Data, Quote);
        }
    }

    public class IntegerValue : Value
    {
        public IntegerValue(object data)
            : base(data)
        {
        }

        public override object Clone()
        {
            return new IntegerValue(Data);
        }
    }

    public class DoublelValue : Value
    {
        public DoublelValue(object data)
            : base(data)
        {
        }

        new public double Data
        {
            get
            {
                return (double)data;
            }
        }

        public override object Clone()
        {
            return new DoublelValue(Data);
        }
    }

    public class DecimalValue : Value
    {
        public DecimalValue(object data)
            : base(data)
        {
        }

        new public decimal Data
        {
            get
            {
                return (decimal)data;
            }
        }

        public override object Clone()
        {
            return new DecimalValue(data);
        }
    }

    public class Qname : Value
    {
        public Qname()
            : base(null)
        {
        }

        public Qname(string name)
            : this()
        {
            data = name;
        }

        public override string ToString()
        {
            return data.ToString();
        }

        public String Name
        {
            get
            {
                return (String)data;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Qname)
            {
                Qname dest = (Qname)obj;
                return dest.data.Equals(data);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override object Clone()
        {
            Qname qname = new Qname();
            qname.data = data;
            return qname;
        }
    }

    public class VarName : Value
    {
        public VarName(string prefix, string localName)
            : base(prefix + ":" + localName)
        {
            Prefix = prefix;
            LocalName = localName;
        }

        public override string ToString()
        {
            return '$' + data.ToString();
        }

        public String Prefix { get; private set; }

        public String LocalName { get; private set; }

        public String Name
        {
            get
            {
                return (String)data;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is VarName)
            {
                VarName dest = (VarName)obj;
                return dest.data.Equals(data);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override object Clone()
        {
            return new VarName(Prefix, Name);
        }
    }
}
