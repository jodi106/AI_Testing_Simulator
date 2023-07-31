using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRSignals : XODRBase
    {
        public static int signalsID = 0;
        public XODRSignals(XmlElement element) : base(element) { }


    }

    public static class XODRSignalsExtentions
    {

        public static XODRSignal AddSignalElement(this XODRSignals parent, string country, string dynamic, string hOffset, string height, string id, string name, string orientation, string pitch, string roll, string s, string subtype, string t, string text, string type, string value, string width, string zOffset)
        {
            var signalElement = new XODRSignal(parent.OwnerDocument.CreateElement("signal"));

            signalElement.SetAttribute("country", country);
            signalElement.SetAttribute("dynamic", dynamic);
            signalElement.SetAttribute("hOffset", hOffset);
            signalElement.SetAttribute("height", height);
            signalElement.SetAttribute("id", id);
            signalElement.SetAttribute("name", name);
            signalElement.SetAttribute("orientation", orientation);
            signalElement.SetAttribute("pitch", pitch);
            signalElement.SetAttribute("roll", roll);
            signalElement.SetAttribute("s", s);
            signalElement.SetAttribute("subtype", subtype);
            signalElement.SetAttribute("t", t);
            signalElement.SetAttribute("text", text);
            signalElement.SetAttribute("type", type);
            signalElement.SetAttribute("value", value);
            signalElement.SetAttribute("width", width);
            signalElement.SetAttribute("zOffset", zOffset);

            parent.AppendChild(signalElement.XmlElement);

            return signalElement;
        }
    }
}