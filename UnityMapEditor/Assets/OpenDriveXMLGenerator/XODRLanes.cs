using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents a lanes element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRLanes : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRLanes.
        /// </summary>
        /// <param name="element">The XML element representing the lanes.</param>
        public XODRLanes(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRLanes class.
    /// </summary>
    public static class XODRLanesExtentions
    {
        /// <summary>
        /// Adds a new laneSection element to the XODRLanes and returns the created XODRLaneSection object.
        /// </summary>
        /// <param name="parent">The XODRLanes object to which the laneSection element will be added.</param>
        /// <param name="s">The start position of the lane section.</param>
        /// <returns>The created XODRLaneSection object.</returns>
        public static XODRLaneSection AddLaneSectionElement(this XODRLanes parent, string s)
        {
            var laneSection = new XODRLaneSection(parent.OwnerDocument.CreateElement("laneSection"));
            laneSection.SetAttribute("s", s);

            parent.AppendChild(laneSection.XmlElement);

            return laneSection;
        }

    }
}