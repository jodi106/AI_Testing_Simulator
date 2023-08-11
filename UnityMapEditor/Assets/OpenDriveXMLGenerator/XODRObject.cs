using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents an object element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRObject : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRObject.
        /// </summary>
        /// <param name="element">The XML element of the object.</param>
        public XODRObject(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRObject class.
    /// </summary>
    public static class XODRObjectExtentions
    {
        /// <summary>
        /// Adds a new outline element to the XODRObject and returns the created XODROutline object.
        /// </summary>
        /// <param name="parent">The XODRObject object to which the outline element will be added.</param>
        /// <returns>The created XODROutline object.</returns>
        public static XODROutline AddOutlineElement(this XODRObject parent)
        {
            var outlineElement = new XODROutline(parent.OwnerDocument.CreateElement("outline"));

            parent.AppendChild(outlineElement.XmlElement);

            return outlineElement;
        }

    }
}