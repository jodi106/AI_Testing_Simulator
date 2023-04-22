using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRObjects : XmlElement
    {
        protected internal XODRObjects(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRObjectsExtentions
    {
        public static XODRObject AddObjectElement(this XODRObjects parent)
        {
            var objectElement = (XODRObject)parent.OwnerDocument.CreateElement("object");

            parent.AppendChild(objectElement);

            return objectElement;
        }

    }
}