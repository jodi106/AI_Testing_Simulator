using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents a link element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRLink : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRLink.
        /// </summary>
        /// <param name="element">The XML elemen of the link.</param>
        public XODRLink(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRLink class.
    /// </summary>
    public static class XODRLinkExtentions
    {
        /// <summary>
        /// Adds a new predecessor element to the XODRLink and returns the created XODRPredecessor object.
        /// </summary>
        /// <param name="parent">The XODRLink object to which the predecessor element will be added.</param>
        /// <param name="elementType">The element type of the predecessor element  ("road" or "junction").</param>
        /// <param name="elementId">The id of the predecessor element.</param>
        /// <returns>The created XODRPredecessor object.</returns>
        public static XODRPredecessor AddPredecessor(this XODRLink parent, string elementType, string elementId)
        {
            var predecessor = new XODRPredecessor(parent.OwnerDocument.CreateElement("predecessor"));
            predecessor.SetAttribute("elementType", elementType);
            predecessor.SetAttribute("elementId", elementId);
            parent.AppendChild(predecessor.XmlElement);

            return predecessor;
        }

        /// <summary>
        /// Adds a new successor element to the XODRLink and returns the created XODRSuccessor object.
        /// </summary>
        /// <param name="parent">The XODRLink object to which the successor element will be added.</param>
        /// <param name="elementType">The element type of the successor element ("road" or "junction").</param>
        /// <param name="elementId">The id of the successor element.</param>
        /// <returns>The created XODRSuccessor object.</returns>
        public static XODRSuccessor AddSuccessor(this XODRLink parent, string elementType, string elementId)
        {
            var successor = new XODRSuccessor(parent.OwnerDocument.CreateElement("successor"));
            successor.SetAttribute("elementType", elementType);
            successor.SetAttribute("elementId", elementId);
            parent.AppendChild(successor.XmlElement);

            return successor;
        }

        /// <summary>
        /// Adds a new predecessor element to the XODRLink (lane predecessor) and returns the created XODRPredecessor object.
        /// </summary>
        /// <param name="parent">The XODRLink object to which the lane predecessor element will be added.</param>
        /// <param name="elementId">The road id of the predecessor element.</param>
        /// <returns>The created XODRPredecessor object.</returns>
        public static XODRPredecessor AddLanePredecessor(this XODRLink parent, string elementId)
        {
            var predecessor = new XODRPredecessor(parent.OwnerDocument.CreateElement("predecessor"));
            predecessor.SetAttribute("elementId", elementId);
            parent.AppendChild(predecessor.XmlElement);

            return predecessor;
        }

        /// <summary>
        /// Adds a new successor element to the XODRLink (lane successor) and returns the created XODRSuccessor object.
        /// </summary>
        /// <param name="parent">The XODRLink object to which the lane successor element will be added.</param>
        /// <param name="elementId">The road id of the successor element.</param>
        /// <returns>The created XODRSuccessor object.</returns>
        public static XODRSuccessor AddLaneSuccessor(this XODRLink parent, string elementId)
        {
            var successor = new XODRSuccessor(parent.OwnerDocument.CreateElement("successor"));
            successor.SetAttribute("elementId", elementId);
            parent.AppendChild(successor.XmlElement);

            return successor;
        }

    }

    /// <summary>
    /// This class represents a predecessor element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRPredecessor : XODRBase
    {
        // <summary>
        /// The default constructor of the class XODRPredecessor.
        /// </summary>
        /// <param name="element">The XML element of the predecessor.</param>
        public XODRPredecessor(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// This class represents a successor element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRSuccessor : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRSuccessor.
        /// </summary>
        /// <param name="element">The XML element of the successor.</param>
        public XODRSuccessor(XmlElement element) : base(element) { }
    }
}