using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRGeometry : XODRBase
    {
        public XODRGeometry(XmlElement element) : base(element) { }
    }

    public static class XODRGeometryExtentions
    {


        public static XODRLine AddLineElement(this XODRGeometry parent)
        {
            var line = new XODRLine(parent.OwnerDocument.CreateElement("line"));

            parent.AppendChild(line.XmlElement);

            return line;
        }

        public static XODRArc AddArcElement(this XODRGeometry parent, string curvature)
        {
            var arc = new XODRArc(parent.OwnerDocument.CreateElement("arc"));

            arc.SetAttribute("curvature", curvature);
            parent.AppendChild(arc.XmlElement);

            return arc;
        }

        public static XODRSpiral AddSpiralElement(this XODRGeometry parent, string curvature) //TODO update this
        {
            var spiral = new XODRSpiral(parent.OwnerDocument.CreateElement("spiral"));

            spiral.SetAttribute("curvature", curvature);
            parent.AppendChild(spiral.XmlElement);

            return spiral;
        }
    }

    public class XODRLine : XODRBase
    {
        public XODRLine(XmlElement element) : base(element) { }
    }
    public class XODRSpiral : XODRBase
    {
        public XODRSpiral(XmlElement element) : base(element) { }
    }
    public class XODRArc : XODRBase
    {
        public XODRArc(XmlElement element) : base(element) { }
    }

}