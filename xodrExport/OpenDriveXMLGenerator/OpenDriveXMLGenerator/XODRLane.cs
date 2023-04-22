using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRLane : XmlElement
    {
        protected internal XODRLane(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRLaneExtentions
    {
        public static XODRWidth AddWidthElement(this XODRLane parent)
        {
            var width = (XODRWidth)parent.OwnerDocument.CreateElement("width");

            parent.AppendChild(width);

            return width;
        }

        public static XODRRoadMark AddRoadMarkElement(this XODRLane parent)
        {
            var roadmark = (XODRRoadMark)parent.OwnerDocument.CreateElement("roadMark");

            parent.AppendChild(roadmark);

            return roadmark;
        }

        public static XODRLink AddLinkElement(this XODRLane parent)
        {
            var link = (XODRLink)parent.OwnerDocument.CreateElement("link");
            parent.AppendChild(link);

            return link;
        }

    }

    public class XODRRoadMark: XmlElement
    {
        protected internal XODRRoadMark(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public class XODRWidth : XmlElement
    {
        protected internal XODRWidth(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }
}