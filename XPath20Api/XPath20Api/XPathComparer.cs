// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Wmhelp.XPath2
{
    class XPathComparer : IComparer<XPathItem>
    {
        #region IComparer<XPathItem> Members

        public int Compare(XPathItem x, XPathItem y)
        {
            XPathNavigator nav1 = x as XPathNavigator;
            XPathNavigator nav2 = y as XPathNavigator;
            if (nav1 != null && nav2 != null)
                switch (nav1.ComparePosition(nav2))
                {
                    case XmlNodeOrder.Before:
                        return -1;

                    case XmlNodeOrder.After:
                        return 1;

                    case XmlNodeOrder.Same:
                        return 0;

                    default:
                        {
                            XPathNavigator root1 = nav1.Clone();
                            root1.MoveToRoot();
                            XPathNavigator root2 = nav2.Clone();
                            root2.MoveToRoot();
                            int hashCode1 = root1.GetHashCode();
                            int hashCode2 = root2.GetHashCode();
                            if (hashCode1 < hashCode2)
                                return -1;
                            else if (hashCode1 > hashCode2)
                                return 1;
                            else
                                throw new InvalidOperationException();
                        }
                }
            else
                throw new XPath2Exception(Properties.Resources.XPTY0004,
                    "xs:anyAtomicType", "node()* in function op:union,op:intersect and op:except");
        }
        #endregion
    }

    class XPathNavigatorEqualityComparer : IEqualityComparer<XPathItem>
    {
        #region IEqualityComparer<XPathItem> Members

        public bool Equals(XPathItem x, XPathItem y)
        {
            XPathNavigator nav1 = x as XPathNavigator;
            XPathNavigator nav2 = y as XPathNavigator;
            if (nav1 != null && nav2 != null)
                return nav1.IsSamePosition(nav2);
            else
                throw new XPath2Exception(Properties.Resources.XPTY0004, "xs:anyAtomicType",
                    "node()* in function op:union,op:intersect and op:except");
        }

        public int GetHashCode(XPathItem obj)
        {
            if (obj.IsNode)
                return XPathNavigator.NavigatorComparer.GetHashCode(obj);
            else
                throw new XPath2Exception(Properties.Resources.XPTY0004, "xs:anyAtomicType",
                    "node()* in function op:union,op:intersect and op:except");
        }

        #endregion
    }
}
