using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents a geometry element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRGeometry : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRGeometry.
        /// </summary>
        /// <param name="element">The geometry XML element.</param>
        public XODRGeometry(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRGeometry class.
    /// </summary>
    public static class XODRGeometryExtentions
    {
        /// <summary>
        /// Adds a new line element to the XODRGeometry and returns the created XODRLine object.
        /// </summary>
        /// <param name="parent">The XODRGeometry object to which the line element will be added.</param>
        /// <returns>The created XODRLine object.</returns>
        public static XODRLine AddLineElement(this XODRGeometry parent)
        {
            var line = new XODRLine(parent.OwnerDocument.CreateElement("line"));

            parent.AppendChild(line.XmlElement);

            return line;
        }

        /// <summary>
        /// Adds a new arc element to the XODRGeometry and returns the created XODRArc object.
        /// </summary>
        /// <param name="parent">The XODRGeometry object to which the arc element will be added.</param>
        /// <param name="curvature">The curvature  of the arc element.</param>
        /// <returns>The created XODRArc object.</returns>
        public static XODRArc AddArcElement(this XODRGeometry parent, string curvature)
        {
            var arc = new XODRArc(parent.OwnerDocument.CreateElement("arc"));

            arc.SetAttribute("curvature", curvature);
            parent.AppendChild(arc.XmlElement);

            return arc;
        }

        /// <summary>
        /// Adds a new spiral element to the XODRGeometry and returns the created XODRSpiral object.
        /// </summary>
        /// <param name="parent">The XODRGeometry object to which the spiral element will be added.</param>
        /// <param name="curvature">The curvature of the spiral element.</param>
        /// <returns>The created XODRSpiral object.</returns>
        public static XODRSpiral AddSpiralElement(this XODRGeometry parent, string curvature) //TODO update this
        {
            var spiral = new XODRSpiral(parent.OwnerDocument.CreateElement("spiral"));

            spiral.SetAttribute("curvature", curvature);
            parent.AppendChild(spiral.XmlElement);

            return spiral;
        }
    }

    /// <summary>
    /// This class represents a line element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRLine : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRLine.
        /// </summary>
        /// <param name="element">The XML element of the line.</param>
        public XODRLine(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// This class represents a spiral element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRSpiral : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRSpiral.
        /// </summary>
        /// <param name="element">The XML element of the spiral.</param>
        public XODRSpiral(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// This class represents an arc element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRArc : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRArc.
        /// </summary>
        /// <param name="element">The XML element of the arc.</param>
        public XODRArc(XmlElement element) : base(element) { }
    }

}