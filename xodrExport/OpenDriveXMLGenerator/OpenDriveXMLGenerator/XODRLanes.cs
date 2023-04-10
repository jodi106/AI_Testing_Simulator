using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRLanes : XmlElement
    {
        protected internal XODRLanes(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRLanesExtentions
    {
        public static XODRLaneSection AddLaneSectionElement(this XODRLanes parent)
        {
            var laneSection = (XODRLaneSection)parent.OwnerDocument.CreateElement("laneSection");

            parent.AppendChild(laneSection);

            return laneSection;
        }

    }
}