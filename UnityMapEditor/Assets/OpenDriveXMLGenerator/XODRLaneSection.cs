using Assets.Helpers;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents a lane section element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRLaneSection : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRLaneSection.
        /// </summary>
        /// <param name="element">The XML element representing the lane section.</param>
        public XODRLaneSection(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRLaneSection class.
    /// </summary>
    public static class XODRLaneSectionExtentions
    {
        /// <summary>
        /// Adds a new direction element to the XODRLaneSection and returns the created XODRDirection object.
        /// </summary>
        /// <param name="parent">The XODRLaneSection object to which the direction element will be added.</param>
        /// <param name="direction">The direction of the lane section.</param>
        /// <returns>The created XODRDirection object.</returns>
        public static XODRDirection AddDirectionElement(this XODRLaneSection parent, Direction direction)
        {
            var directionElement = new XODRDirection(parent.OwnerDocument.CreateElement(direction.GetDescription()));

            parent.AppendChild(directionElement.XmlElement);

            return directionElement;
        }

    }
}