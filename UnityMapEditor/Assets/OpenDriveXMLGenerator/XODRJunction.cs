using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents a junction element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRJunction : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRJunction.
        /// </summary>
        /// <param name="element">The XML element of the junction.</param>
        public XODRJunction(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRJunction class.
    /// </summary>
    public static class XODRJunctionExtentions
    {
        /// <summary>
        /// Adds a new connection element to the XODRJunction and returns the created XODRConnection object.
        /// The connection element contains information about predecessors and successors of the junction and to which part of the junction they are connected.
        /// </summary>
        /// <param name="parent">The XODRJunction object to which the connection element will be added.</param>
        /// <param name="id">The id (unique within a junction) of the connection.</param>
        /// <param name="incomingRoadId">The id of the road that the connection is incoming from (not part of the junction).</param>
        /// <param name="connectingRoadId">The id of the road within the junction that the connection is connecting to.</param>
        /// <returns>The created XODRConnection object.</returns>
        public static XODRConnection AddConnectionElement(this XODRJunction parent, string id,  string incomingRoadId, string connectingRoadId)
        {
            var connection = new XODRConnection(parent.OwnerDocument.CreateElement("connection"));
            connection.SetAttribute("id", id);
            connection.SetAttribute("incomingRoad", incomingRoadId);
            connection.SetAttribute("connectingRoad", connectingRoadId);
            parent.AppendChild(connection.XmlElement);

            return connection;
        }

    }
}