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

    public class XODRDirection : XmlElement
    {
        protected internal XODRDirection(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRDirectionExtentions
    {
        public static XODRLane AddLaneSectionElement(this XODRDirection parent)
        {
            var lane = (XODRLane)parent.OwnerDocument.CreateElement("lane");

            parent.AppendChild(lane);

            return lane;
        }

    }
}