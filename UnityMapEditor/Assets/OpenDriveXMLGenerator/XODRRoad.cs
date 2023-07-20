using System.ComponentModel;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRRoad : XODRBase
    {
        public XODRRoad(XmlElement element) : base(element) { } 
    }

    public static class XODRRoadExtentions
    {
        public static XODRPlainView AddPlainViewElement(this XODRRoad parent)
        {
            var planView = new XODRPlainView(parent.OwnerDocument.CreateElement("planView"));
            parent.AppendChild(planView.XmlElement);

            return planView;
        }

        public static XODRLink AddLinkElement(this XODRRoad parent)
        {
            var link = new XODRLink(parent.OwnerDocument.CreateElement("link"));
            parent.AppendChild(link.XmlElement);

            return link;
        }

        public static XODRLanes AddLanesElement(this XODRRoad parent)
        {
            var lanes = new XODRLanes(parent.OwnerDocument.CreateElement("lanes"));
            parent.AppendChild(lanes.XmlElement);

            return lanes;
        }

        public static XODRObjects AddObjectsElement(this XODRRoad parent)
        {
            var objects = new XODRObjects(parent.OwnerDocument.CreateElement("objects"));
            parent.AppendChild(objects.XmlElement);

            return objects;
        }

        public static XODRSignals AddSignalsElement(this XODRRoad parent)
        {
            var signals = new XODRSignals(parent.OwnerDocument.CreateElement("Signals"));
            parent.AppendChild(signals.XmlElement);

            return signals;
        }

    }
}