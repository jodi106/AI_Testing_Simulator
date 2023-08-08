using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class represents an objects element in an Opendrive file and inherits from the class XODRBase.
    /// </summary>
    public class XODRObjects : XODRBase
    {
        /// <summary>
        /// A increasing variable to define the id of a object.
        /// </summary>
        public static int objectsID = 0;

        /// <summary>
        /// The default constructor of the class XODRObjects.
        /// </summary>
        /// <param name="element">The XML element representing the objects.</param>
        public XODRObjects(XmlElement element) : base(element) { }
    }

    /// <summary>
    /// A static class containing extension methods for the XODRObjects class.
    /// </summary>
    public static class XODRObjectsExtentions
    {
        /// <summary>
        /// Adds a new object element to the XODRObjects and returns the created XODRObject object.
        /// </summary>
        /// <param name="parent">The XODRObjects object to which the object element will be added.</param>
        /// <param name="zOffset">The zOffset from track level of the object element.</param>
        /// <param name="s">The s value of the object element, which represents the road position of the object's origin.</param>
        /// <param name="t">The t value of the object element, which represents the road position of the object's origin.</param>
        /// <param name="hdg">Optional. The heading (rotation)of the object element relative to the road. Default is "1.57".</param>
        /// <param name="id">Optional. The id of the object element. If null, a unique id is generated.</param>
        /// <param name="length">Optional. The length of the object element. Default is "6".</param>
        /// <param name="name">Optional. The name of the object element. Default is "cross".</param>
        /// <param name="orientation">Optional. The orientation of the object element.
        /// ("+": valid in positive track direction, "-": valid in negative track direction, "none": valid in both direction. Default is "-".</param>
        /// <param name="pitch">Optional. The pitch angle of the object relative to road pitch. Default is "0.0000000000000000e+0".</param>
        /// <param name="roll">Optional. The roll angle of the object relative to road roll. Default is "0.0000000000000000e+0".</param>
        /// <param name="type">Optional. The type of the object element, e.g., parkingSpace. Default is "crosswalk".</param>
        /// <param name="width">Optional. The width of the object element. Default is "10.0000000000000000e+0".</param>
        /// <returns>The created XODRObject object.</returns>
        public static XODRObject AddObjectElement(this XODRObjects parent, string zOffset, string s, string t, string hdg = "1.57", string id = null, string length = "6", string name = "cross", string orientation = "-", string pitch = "0.0000000000000000e+0", string roll = "0.0000000000000000e+0", string type = "crosswalk", string width = "10.0000000000000000e+0")
        {
            var objectElement = new XODRObject(parent.OwnerDocument.CreateElement("object"));

            
            objectElement.SetAttribute("type", type);
            objectElement.SetAttribute("id", id);
            objectElement.SetAttribute("s", s);
            objectElement.SetAttribute("t", t);
            objectElement.SetAttribute("zOffset", zOffset);
            objectElement.SetAttribute("orientation", orientation);
            objectElement.SetAttribute("length", length);
            objectElement.SetAttribute("width", width);
            objectElement.SetAttribute("hdg", hdg);
            if (id == null)
            {
                id = XODRObjects.objectsID.ToString();
                XODRObjects.objectsID++;
            }
            objectElement.SetAttribute("pitch", pitch);
            objectElement.SetAttribute("roll", roll);
            objectElement.SetAttribute("name", name);

            parent.AppendChild(objectElement.XmlElement);

            return objectElement;
        }

    }

}