using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRRoad : XmlElement
    {
        protected internal XODRRoad(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRRoadExtentions
    {
        public static XODRPlainView AddPlainViewElement(this XODRRoad parent)
        {
            var planView = (XODRPlainView)parent.OwnerDocument.CreateElement("planView");
            parent.AppendChild(planView);

            return planView;
        }

        public static XODRLink AddLinkElement(this XODRRoad parent)
        {
            var link = (XODRLink)parent.OwnerDocument.CreateElement("link");
            parent.AppendChild(link);

            return link;
        }

    }
}