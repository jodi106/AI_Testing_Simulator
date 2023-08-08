using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// The class XODRBase contains methods which help creating a XML element.
    /// </summary>
    public class XODRBase
    {
        protected XmlElement element;

        public XmlElement XmlElement { get { return element; } }

        public XmlDocument OwnerDocument
        {
            get { return element.OwnerDocument; }
        }

        /// <summary>
        /// Constructor of the class XODRBase.
        /// </summary>
        /// <param name="element">The XML element.</param>
        public XODRBase(XmlElement element)
        {
            this.element = element;
        }

        /// <summary>
        /// This method appends a XML element to its parent element.
        /// </summary>
        /// <param name="element">The XML element which should be appended.</param>
        public void AppendChild(XmlElement element)
        {
            this.element.AppendChild(element);
        }

        /// <summary>
        /// This method sets an attribute of a XML element.
        /// </summary>
        /// <param name="element">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void SetAttribute(string name, string? value)
        {
            this.element.SetAttribute(name, value);
        }

    }
}