using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRPlainView : XODRBase
    {
        public XODRPlainView(XmlElement element) : base(element) { }
    }

    public static class XODRPlainViewExtentions
    {
        public static XODRGeometry AddGeometryElement(this XODRPlainView parent, string s, string x, string y, string hdg, string length, string curvature = null)
        {
            var geometry = new XODRGeometry(parent.OwnerDocument.CreateElement("geometry"));
            geometry.SetAttribute("s", s);
            geometry.SetAttribute("x", x);
            geometry.SetAttribute("y", y);
            geometry.SetAttribute("hdg", hdg);
            geometry.SetAttribute("length", length);
            parent.AppendChild(geometry.XmlElement);

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