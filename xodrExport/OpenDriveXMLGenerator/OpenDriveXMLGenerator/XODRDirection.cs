using System.ComponentModel;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    public enum Direction
    {
        [Description("left")]
        Left,
        [Description("center")]
        Center,
        [Description("right")]
        Right
    }

    public class XODRDirection : XODRBase
    {
        public XODRDirection(XmlElement element) : base(element) { }
    }

    public static class XODRDirectionExtentions
    {
        public static XODRLane AddLaneSectionElement(this XODRDirection parent)
        {
            var lane = new XODRLane(parent.OwnerDocument.CreateElement("lane"));

            parent.AppendChild(lane.XmlElement);

            return lane;
        }

    }
}