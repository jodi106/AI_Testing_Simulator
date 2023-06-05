using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRJunction : XODRBase
    {
        public XODRJunction(XmlElement element) : base(element) { }
    }

    public static class XODRJunctionExtentions
    {
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