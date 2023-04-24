using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRObject : XODRBase
    {
        public XODRObject(XmlElement element) : base(element) { }
    }

    public static class XODRObjectExtentions
    {
         public static XODROutline AddOutlineElement(this XODRObject parent)
        {
            var outlineElement = new XODROutline(parent.OwnerDocument.CreateElement("outline"));

            parent.AppendChild(outlineElement.XmlElement);

            return outlineElement;
        }

    }




}