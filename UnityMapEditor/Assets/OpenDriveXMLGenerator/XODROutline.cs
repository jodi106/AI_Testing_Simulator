using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents an outline element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODROutline : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODROutline.
        /// </summary>
        /// <param name="element">The XML element of the outline.</param>
        public XODROutline(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODROutline class.
    /// </summary>
    public static class XODROutlineExtentions
    {
        /// <summary>
        /// Adds a new cornerLocal element to the XODROutline and returns the created XODRCornerLocal object.
        /// </summary>
        /// <param name="parent">The XODROutline object to which the cornerLocal element will be added.</param>
        /// <param name="u">The u coordinate off the corner.</param>
        /// <param name="v">The v coordinate off the corner.</param>
        /// <returns>The created XODRCornerLocal object.</returns>
        public static XODRCornerLocal AddCornerLocalElement(this XODROutline parent, string u, string v)
        {
            var cornerLocal = new XODRCornerLocal(parent.OwnerDocument.CreateElement("cornerLocal"));

            cornerLocal.SetAttribute("u", u);
            cornerLocal.SetAttribute("v", v);

            parent.AppendChild(cornerLocal.XmlElement);

            return cornerLocal;
        }

    }

    /// <summary>
    /// This class represents a cornerLocal element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRCornerLocal : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRCornerLocal.
        /// </summary>
        /// <param name="element">The XML element of the cornerLocal.</param>
        public XODRCornerLocal(XmlElement element) : base(element) { }
    }

}