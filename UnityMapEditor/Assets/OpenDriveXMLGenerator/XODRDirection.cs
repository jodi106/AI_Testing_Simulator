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
        public static XODRLane AddLaneElement(this XODRDirection parent, string id, string type, string level)
        {
            var lane = new XODRLane(parent.OwnerDocument.CreateElement("lane"));
            lane.SetAttribute("id", id);
            lane.SetAttribute("type", type);
            lane.SetAttribute("level", level);

            parent.AppendChild(lane.XmlElement);

            return lane;
        }

    }
}