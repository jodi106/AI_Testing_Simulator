using Assets.Helpers;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRLaneSection : XODRBase
    {
        public XODRLaneSection(XmlElement element) : base(element) { }
    }

    public static class XODRLaneSectionExtentions
    {
        public static XODRDirection AddDirectionElement(this XODRLaneSection parent, Direction direction)
        {
            var directionElement = new XODRDirection(parent.OwnerDocument.CreateElement(direction.GetDescription()));

            parent.AppendChild(directionElement.XmlElement);

            return directionElement;
        }

    }
}