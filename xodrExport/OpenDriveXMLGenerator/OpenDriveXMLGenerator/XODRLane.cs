using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRLane : XODRBase
    {
        public XODRLane(XmlElement element) : base(element) { }
    }

    public static class XODRLaneExtentions
    {
        public static XODRWidth AddWidthElement(this XODRLane parent, string sOffset = "0.0000000000000000e+000",string a="3.5000000000000000e+000" ,string b="0.0000000000000000e+000",string c="0.0000000000000000e+000",string d="0.0000000000000000e+000" )
        {
            var width = new XODRWidth(parent.OwnerDocument.CreateElement("width"));
            width.SetAttribute("sOffset", sOffset);
            width.SetAttribute("a",a);
            width.SetAttribute("b",b);
            width.SetAttribute("c",c);
            width.SetAttribute("d",d);

            parent.AppendChild(width.XmlElement);

            return width;
        }

        public static XODRRoadMark AddRoadMarkElement(this XODRLane parent)
        {
            var roadmark = new XODRRoadMark(parent.OwnerDocument.CreateElement("roadMark"));

            parent.AppendChild(roadmark.XmlElement);

            return roadmark;
        }

        public static XODRLink AddLinkElement(this XODRLane parent)
        {
            var link = new XODRLink(parent.OwnerDocument.CreateElement("link"));

            parent.AppendChild(link.XmlElement);

            return link;
        }

    }

    public class XODRRoadMark : XODRBase
    {
        public XODRRoadMark(XmlElement element) : base(element) { }
    }


    public class XODRWidth : XODRBase
    {
        public XODRWidth(XmlElement element) : base(element) { }
    }
}