using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRJunction : XODRBase
    {
        public XODRJunction(XmlElement element) : base(element) { }
    }

    public static class XODRJunctionExtentions
    {
        public static XODRConnection AddConnectionElement(this XODRRoad parent, int id,  int incomingRoadId, int connectingRoadId)
        {
            var connection = new XODRConnection(parent.OwnerDocument.CreateElement("connection"));
            connection.SetAttribute("id", id.ToString());
            connection.SetAttribute("incomingRoad", incomingRoadId.ToString());
            connection.SetAttribute("connectingRoad", connectingRoadId.ToString());
            parent.AppendChild(connection.XmlElement);

            return connection;
        }

    }
}