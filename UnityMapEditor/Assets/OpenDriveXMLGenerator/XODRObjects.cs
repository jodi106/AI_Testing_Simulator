using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRObjects : XODRBase
    {
        public static int objectsID = 0;
        public XODRObjects(XmlElement element) : base(element) { }
    }

    public static class XODRObjectsExtentions
    {
        

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