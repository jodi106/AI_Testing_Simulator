using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRGeometry : XmlElement
    {
        protected internal XODRGeometry(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRGeometryExtentions
    {


        public static XODRLine AddLineElement(this XODRGeometry parent)
        {
            var line = (XODRLine)parent.OwnerDocument.CreateElement("line");

            parent.AppendChild(line);

            return line;
        }

        public static XODRArc AddArcElement(this XODRGeometry parent, string curvature)
        {
            var arc = (XODRArc)parent.OwnerDocument.CreateElement("arc");

            arc.SetAttribute("curvature", curvature);
            parent.AppendChild(arc);

            return arc;
        }

        public static XODRSpiral AddSpiralElement(this XODRGeometry parent, string curvature) //TODO update this
        {
            var spiral = (XODRSpiral)parent.OwnerDocument.CreateElement("spiral");

            spiral.SetAttribute("curvature", curvature);
            parent.AppendChild(spiral);

            return spiral;
        }
    }

    public class XODRLine : XmlElement
    {
        protected internal XODRLine(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public class XODRSpiral : XmlElement
    {
        protected internal XODRSpiral(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public class XODRArc : XmlElement
    {
        protected internal XODRArc(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }
}