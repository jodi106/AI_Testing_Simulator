using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRObjects : XODRBase
    {
        public XODRObjects(XmlElement element) : base(element) { }
    }

    public static class XODRObjectsExtentions
    {
        public static XODRObject AddObjectElement(this XODRObjects parent)
        {
            var objectElement = new XODRObject(parent.OwnerDocument.CreateElement("object"));

            parent.AppendChild(objectElement.XmlElement);

            return objectElement;
        }

    }

}