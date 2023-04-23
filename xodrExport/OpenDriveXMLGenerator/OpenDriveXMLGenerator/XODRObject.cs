using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRObject : XODRBase
    {
        public XODRObject(XmlElement element) : base(element) { }
    }

    public static class XODRObjectExtentions
    {
         public static XODROutline AddOutlineElement(this XODRObjects parent)
        {
            var outlineElement = new XODROutline(parent.OwnerDocument.CreateElement("outline"));

            parent.AppendChild(outlineElement.XmlElement);

            return outlineElement;
        }

    }


    public class XODROutline : XODRBase
    {
        public XODROutline(XmlElement element) : base(element) { }
    }

}