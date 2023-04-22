using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRObject : XmlElement
    {
        protected internal XODRObject(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRObjectExtentions
    {


    }
}