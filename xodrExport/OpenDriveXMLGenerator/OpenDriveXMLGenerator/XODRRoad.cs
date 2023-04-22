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

        public static XODRLane AddLaneElement(this XODRRoad parent)
        {
            var lane = (XODRLane)parent.OwnerDocument.CreateElement("lane");
            parent.AppendChild(lane);

            return lane;
        }

        public static XODRObjects AddObjectsElement(this XODRRoad parent)
        {
            var objects = (XODRObjects)parent.OwnerDocument.CreateElement("objects");
            parent.AppendChild(objects);

            return objects;
        }

        public static XODRSignals AddSignalsElement(this XODRRoad parent)
        {
            var signals = (XODRSignals)parent.OwnerDocument.CreateElement("signals");
            parent.AppendChild(signals);

            return signals;
        }

    }
}