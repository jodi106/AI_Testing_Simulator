using System.ComponentModel;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents a road element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRRoad : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRRoad.
        /// </summary>
        /// <param name="element">The XML element of the road.</param>
        public XODRRoad(XmlElement element) : base(element) { } 
    }

    /// <summary>
    /// A static class containing extension methods for the XODRRoad class.
    /// </summary>
    public static class XODRRoadExtentions
    {
        /// <summary>
        /// Adds a new plain view element to the XODRRoad and returns the created XODRPlainView object.
        /// </summary>
        /// <param name="parent">The XODRRoad object to which the plain view element will be added.</param>
        /// <returns>The created XODRPlainView object.</returns>
        public static XODRPlainView AddPlainViewElement(this XODRRoad parent)
        {
            var planView = new XODRPlainView(parent.OwnerDocument.CreateElement("planView"));
            parent.AppendChild(planView.XmlElement);

            return planView;
        }

        /// <summary>
        /// Adds a new link element to the XODRRoad and returns the created XODRLink object.
        /// </summary>
        /// <param name="parent">The XODRRoad object to which the link element will be added.</param>
        /// <returns>The created XODRLink object.</returns>
        public static XODRLink AddLinkElement(this XODRRoad parent)
        {
            var link = new XODRLink(parent.OwnerDocument.CreateElement("link"));
            parent.AppendChild(link.XmlElement);

            return link;
        }

        /// <summary>
        /// Adds a new lanes element to the XODRRoad and returns the created XODRLanes object.
        /// </summary>
        /// <param name="parent">The XODRRoad object to which the lanes element will be added.</param>
        /// <returns>The created XODRLanes object.</returns>
        public static XODRLanes AddLanesElement(this XODRRoad parent)
        {
            var lanes = new XODRLanes(parent.OwnerDocument.CreateElement("lanes"));
            parent.AppendChild(lanes.XmlElement);

            return lanes;
        }

        /// <summary>
        /// Adds a new objects element to the XODRRoad and returns the created XODRObjects object.
        /// </summary>
        /// <param name="parent">The XODRRoad object to which the objects element will be added.</param>
        /// <returns>The created XODRObjects object.</returns>
        public static XODRObjects AddObjectsElement(this XODRRoad parent)
        {
            var objects = new XODRObjects(parent.OwnerDocument.CreateElement("objects"));
            parent.AppendChild(objects.XmlElement);

            return objects;
        }

        /// <summary>
        /// Adds a new signals element to the XODRRoad and returns the created XODRSignals object.
        /// </summary>
        /// <param name="parent">The XODRRoad object to which the signals element will be added.</param>
        /// <returns>The created XODRSignals object.</returns>
        public static XODRSignals AddSignalsElement(this XODRRoad parent)
        {
            var signals = new XODRSignals(parent.OwnerDocument.CreateElement("signals"));
            parent.AppendChild(signals.XmlElement);

            return signals;
        }

    }
}