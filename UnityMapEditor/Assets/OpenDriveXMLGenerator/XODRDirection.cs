using System.ComponentModel;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// An enumeration representing different directions for Opendrive elements.
    /// </summary>
    public enum Direction
    {
        [Description("left")]
        Left,
        [Description("center")]
        Center,
        [Description("right")]
        Right
    }

    /// <summary>
    /// This class represents a direction element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRDirection : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRDirection.
        /// </summary>
        /// <param name="element">The XML element of the direction.</param>
        public XODRDirection(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRDirection class.
    /// </summary>
    public static class XODRDirectionExtentions
    {
        /// <summary>
        /// Adds a new lane element to the XODRDirection and returns the created XODRLane object.
        /// </summary>
        /// <param name="parent">The XODRDirection object to which the lane element will be added.</param>
        /// <param name="id">The id of the lane.</param>
        /// <param name="type">The type of the lane.</param>
        /// <param name="level">The level of the lane.</param>
        /// <returns>The created XODRLane object.</returns>
        public static XODRLane AddLaneElement(this XODRDirection parent, string id, string type, string level)
        {
            var lane = new XODRLane(parent.OwnerDocument.CreateElement("lane"));
            lane.SetAttribute("id", id);
            lane.SetAttribute("type", type);
            lane.SetAttribute("level", level);

            parent.AppendChild(lane.XmlElement);

            return lane;
        }

    }
}