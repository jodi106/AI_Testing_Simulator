using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRBase
    {
        protected XmlElement element;

        public XmlElement XmlElement { get { return element; } }

        public XmlDocument OwnerDocument
        {
            get { return element.OwnerDocument; }
        }

        public XODRBase(XmlElement element)
        {
            this.element = element;
        }

        public void AppendChild(XmlElement element)
        {
            this.element.AppendChild(element);
        }

        public void SetAttribute(string name, string? value)
        {
            this.element.SetAttribute(name, value);
        }

    }
}