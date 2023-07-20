using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRSignals : XODRBase
    {
        public XODRSignals(XmlElement element) : base(element) { }
    }

    public static class XODRSignalsExtentions
    {
        public static XODRSignal AddSignalElement(this XODRSignals parent)
        {
            var signal = new XODRSignal(parent.OwnerDocument.CreateElement("signal"));

            parent.AppendChild(signal.XmlElement);

            return signal;
        }
    }
}