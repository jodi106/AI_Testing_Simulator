using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRLanes : XODRBase
    {
        public XODRLanes(XmlElement element) : base(element) { }
    }

    public static class XODRLanesExtentions
    {
        public static XODRLaneSection AddLaneSectionElement(this XODRLanes parent, string s)
        {
            var laneSection = new XODRLaneSection(parent.OwnerDocument.CreateElement("laneSection"));
            laneSection.SetAttribute("s", s);

            parent.AppendChild(laneSection.XmlElement);

            return laneSection;
        }

    }
}