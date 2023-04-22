using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRSignals : XmlElement
    {
        protected internal XODRSignals(string prefix, string localName, string? namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }
    }

    public static class XODRSignalsExtentions
    {
        public static XODRSignal AddSignalElement(this XODRSignals parent)
        {
            var signal = (XODRSignal)parent.OwnerDocument.CreateElement("signal");

            parent.AppendChild(signal);

            return signal;
        }
    }
}