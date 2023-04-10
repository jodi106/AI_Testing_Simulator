using Assets.Helpers;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRLaneSection : XmlElement
    {
        protected internal XODRLaneSection(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRLaneSectionExtentions
    {
        public static XODRDirection AddDirectionElement(this XODRLaneSection parent, Direction direction)
        {
            var directionElement = (XODRDirection)parent.OwnerDocument.CreateElement(direction.GetDescription());

            parent.AppendChild(directionElement);

            return directionElement;
        }

    }
}