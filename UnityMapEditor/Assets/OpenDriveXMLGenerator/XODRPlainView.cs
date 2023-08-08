using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents a plain view element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRPlainView : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRPlainView.
        /// </summary>
        /// <param name="element">The plain view XML element.</param>
        public XODRPlainView(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRPlainView class.
    /// </summary>
    public static class XODRPlainViewExtentions
    {
        /// <summary>
        /// Adds a new geometry element to the XODRPlainView of a road and returns the created XODRGeometry object.
        /// </summary>
        /// <param name="parent">The XODRPlainView object to which the geometry element will be added.</param>
        /// <param name="s">The starting position (s-coordinate) of the road.</param>
        /// <param name="x">The starting position (x inertial) of the road.</param>
        /// <param name="y">The starting position (y inertial) of the road.</param>
        /// <param name="hdg">The heading (rotation) of the road.</param>
        /// <param name="length">The length of the roady.</param>
        /// <param name="curvature">Optional. The curvature of the road. If null, a line element is added; otherwise, an arc element is added.</param>
        /// <returns>The created XODRGeometry object.</returns>
        public static XODRGeometry AddGeometryElement(this XODRPlainView parent, string s, string x, string y, string hdg, string length, string curvature = null)
        {
            var geometry = new XODRGeometry(parent.OwnerDocument.CreateElement("geometry"));
            geometry.SetAttribute("s", s);
            geometry.SetAttribute("x", x);
            geometry.SetAttribute("y", y);
            geometry.SetAttribute("hdg", hdg);
            geometry.SetAttribute("length", length);
            parent.AppendChild(geometry.XmlElement);

            if (curvature == null)
            {
                geometry.AddLineElement();
            }
            else
            {
                geometry.AddArcElement(curvature);
            }

            return geometry;
        }

    }
}