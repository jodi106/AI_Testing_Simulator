using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRLink : XmlElement
    {
        protected internal XODRLink(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRLinkExtentions
    {
        public static XODRPredecessor AddPredecessor(this XODRLink parent, string elementType, string elementId)
        {
            var predecessor = (XODRPredecessor)parent.OwnerDocument.CreateElement("predecessor");
            predecessor.SetAttribute("elementType", elementType);
            predecessor.SetAttribute("elementId", elementId);
            parent.AppendChild(predecessor);

            return predecessor;
        }

        public static XODRSuccessor AddSuccessor(this XODRLink parent, string elementType, string elementId)
        {
            var successor = (XODRSuccessor)parent.OwnerDocument.CreateElement("successor");
            successor.SetAttribute("elementType", elementType);
            successor.SetAttribute("elementId", elementId);
            parent.AppendChild(successor);

            return successor;
        }

    }

    public class XODRPredecessor : XmlElement
    {
        protected internal XODRPredecessor(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public class XODRSuccessor : XmlElement
    {
        protected internal XODRSuccessor(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }
}