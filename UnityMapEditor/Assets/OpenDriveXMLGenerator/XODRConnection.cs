using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents a connection element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRConnection : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRConnection.
        /// </summary>
        /// <param name="element">The XML element of the connection.</param>
        public XODRConnection(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRConnection class.
    /// </summary>
    public static class XODRConnectionExtentions
    {
        /// <summary>
        /// Adds a new laneLink element to the XODRConnection and returns the created XODRLaneLink object.
        /// </summary>
        /// <param name="parent">The XODRConnection object to which the laneLink element will be added.</param>
        /// <param name="from">The id of the lane from the other road.</param>
        /// <param name="to">The the id of the lane within the road.</param>
        /// <returns>The created XODRLaneLink object.</returns>
        public static XODRLaneLink AddLaneLinkElement(this XODRConnection parent, string from, string to)
        {
            var laneLink = new XODRLaneLink(parent.OwnerDocument.CreateElement("laneLink"));
            laneLink.SetAttribute("from", from);
            laneLink.SetAttribute("to", to);
            parent.AppendChild(laneLink.XmlElement);

            return laneLink;
        }
    }

    /// <summary>
    /// This class represents a laneLink element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRLaneLink : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRLaneLink.
        /// </summary>
        /// <param name="element">The XML element of the lane link.</param>
        public XODRLaneLink(XmlElement element) : base(element) { }
    }
}