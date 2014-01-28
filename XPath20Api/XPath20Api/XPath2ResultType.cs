using System;
using System.Xml;

namespace Wmhelp.XPath2
{
    public enum XPath2ResultType
    {
        // Summary:
        //     A numeric value.
        Number = 0,
        //
        // Summary:
        //     A System.String value.
        String = 1,
        //
        // Summary:
        //     A tree fragment.
        Navigator = 1,
        //
        // Summary:
        //     A System.Boolean true or false value.
        Boolean = 2,
        //
        // Summary:
        //     A node collection.
        NodeSet = 3,
        //
        // Summary:
        //     Any of the XPath node types.
        Any = 5,
        //
        // Summary:
        //     The expression does not evaluate to the correct XPath type.
        Error = 6,
        //
        // Summary:
        //     A System.DateTime value
        DateTime = 7,
        //
        // Summary:
        //     A System.DateTime value
        Duration = 8,
        //
        // Summary:
        //     A AnyUri value
        AnyUri = 9,
        //
        // Summary:
        //     A AnyUri value
        QName = 10,
        //
        // Summary:
        //     A Other typed value
        Other = 11
    };
}