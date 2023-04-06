using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRPlainView : XmlElement
    {
        protected internal XODRPlainView(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRPlainViewExtentions
    {
        public static XODRGeometry AddGeometryElement(this XODRPlainView parent, string s, string x, string y, string hdg, string length, string curvature)
        {
            var geometry = (XODRGeometry)parent.OwnerDocument.CreateElement("geometry");
            geometry.SetAttribute("s", s);
            geometry.SetAttribute("x", x);
            geometry.SetAttribute("y", y);
            geometry.SetAttribute("hdg", hdg);
            geometry.SetAttribute("length", length);
            parent.AppendChild(geometry);

            if (curvature == null)
            {
                geometry.AddLineElement();
            }
            else
            {
                geometry.AddArcElement(curvature);
            }

            return geometry;
        }

    }
}