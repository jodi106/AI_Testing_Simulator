using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRLink : XODRBase
    {
        public XODRLink(XmlElement element) : base(element) { }
    }

    public static class XODRLinkExtentions
    {
        public static XODRPredecessor AddPredecessor(this XODRLink parent, string elementType, string elementId)
        {
            var predecessor = new XODRPredecessor(parent.OwnerDocument.CreateElement("predecessor"));
            predecessor.SetAttribute("elementType", elementType);
            predecessor.SetAttribute("elementId", elementId);
            parent.AppendChild(predecessor.XmlElement);

            return predecessor;
        }

        public static XODRSuccessor AddSuccessor(this XODRLink parent, string elementType, string elementId)
        {
            var successor = new XODRSuccessor(parent.OwnerDocument.CreateElement("successor"));
            successor.SetAttribute("elementType", elementType);
            successor.SetAttribute("elementId", elementId);
            parent.AppendChild(successor.XmlElement);

            return successor;
        }

    }

    public class XODRPredecessor : XODRBase
    {
        public XODRPredecessor(XmlElement element) : base(element) { }
    }

    public class XODRSuccessor : XODRBase
    {
        public XODRSuccessor(XmlElement element) : base(element) { }
    }
}