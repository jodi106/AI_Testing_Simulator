using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents a lane element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRLane : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRLane.
        /// </summary>
        /// <param name="element">The lane XML element.</param>
        public XODRLane(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRLane class.
    /// </summary>
    public static class XODRLaneExtentions
    {
        /// <summary>
        /// Adds a new width element to the XODRLane and returns the created XODRWidth object.
        /// </summary>
        /// <param name="parent">The XODRLane object to which the width element will be added.</param>
        /// <param name="sOffset">Optional. The sOffset of the width element, which is the start position (s-coordinate) relative to the position of the preceding laneSection record.</param>
        /// <param name="a">Optional. The a attribute value (width at s = 0) of the width element.</param>
        /// <param name="b">Optional. The b attribute value of the width element.</param>
        /// <param name="c">Optional. The c attribute value of the width element.</param>
        /// <param name="d">Optional. The d attribute value of the width element.</param>
        /// <returns>The created XODRWidth object.</returns>
        public static XODRWidth AddWidthElement(this XODRLane parent, string sOffset = "0.0000000000000000e+000",string a="3.50000000000000000e+000" ,string b="0.0000000000000000e+000",string c="0.0000000000000000e+000",string d="0.0000000000000000e+000" )
        {
            var width = new XODRWidth(parent.OwnerDocument.CreateElement("width"));
            width.SetAttribute("sOffset", sOffset);
            width.SetAttribute("a",a);
            width.SetAttribute("b",b);
            width.SetAttribute("c",c);
            width.SetAttribute("d",d);

            parent.AppendChild(width.XmlElement);

            return width;
        }

        /// <summary>
        /// Adds a new roadMark element to the XODRLane and returns the created XODRRoadMark object.
        /// </summary>
        /// <param name="parent">The XODRLane object to which the roadMark element will be added.</param>
        /// <returns>The created XODRRoadMark object.</returns>
        public static XODRRoadMark AddRoadMarkElement(this XODRLane parent)
        {
            var roadmark = new XODRRoadMark(parent.OwnerDocument.CreateElement("roadMark"));

            parent.AppendChild(roadmark.XmlElement);

            return roadmark;
        }

        /// <summary>
        /// Adds a new link element to the XODRLane and returns the created XODRLink object.
        /// </summary>
        /// <param name="parent">The XODRLane object to which the link element will be added.</param>
        /// <returns>The created XODRLink object.</returns>
        public static XODRLink AddLinkElement(this XODRLane parent)
        {
            var link = new XODRLink(parent.OwnerDocument.CreateElement("link"));

            parent.AppendChild(link.XmlElement);

            return link;
        }

    }

    /// <summary>
    /// This class represents a roadMark element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRRoadMark : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRRoadMark.
        /// </summary>
        /// <param name="element">The XML element representing the roadMark.</param>
        public XODRRoadMark(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// This class represents a width element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRWidth : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRWidth.
        /// </summary>
        /// <param name="element">The XML element of the width.</param>
        public XODRWidth(XmlElement element) : base(element) { }
    }
}