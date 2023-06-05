using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRConnection : XODRBase
    {
        public XODRConnection(XmlElement element) : base(element) { }
    }

    public static class XODRConnectionExtentions
    {
        public static XODRLaneLink AddLaneLinkElement(this XODRConnection parent, string from, string to)
        {
            var laneLink = new XODRLaneLink(parent.OwnerDocument.CreateElement("laneLink"));
            laneLink.SetAttribute("from", from);
            laneLink.SetAttribute("to", to);
            parent.AppendChild(laneLink.XmlElement);

            return laneLink;
        }
    }

    public class XODRLaneLink : XODRBase
    {
        public XODRLaneLink(XmlElement element) : base(element) { }
    }
}