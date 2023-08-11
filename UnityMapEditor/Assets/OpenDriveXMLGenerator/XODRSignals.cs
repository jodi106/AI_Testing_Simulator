using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class inherits from the class XODRBase and represents a collection of signal elements in an Opendrive file.
    /// </summary>
    public class XODRSignals : XODRBase
    {
        /// <summary>
        /// A increasing variable to define the id of a signal.
        /// </summary>
        public static int signalsID = 0;
        /// <summary>
        /// The default constructor of the class XODRSignals.
        /// </summary>
        /// /// <param name="element">The XML element representing the signals collection.</param>
        public XODRSignals(XmlElement element) : base(element) { }


    }

    /// <summary>
    /// A static class containing extension methods for the XODRSignals class to create the XML element.
    /// </summary>
    public static class XODRSignalsExtentions
    {

        /// <summary>
        /// Adds a new signal element to the XODRSignals collection and returns the created XODRSignal object.
        /// </summary>
        /// <param name="parent">The XODRSignals object to which the signal element will be added.</param>
        /// <param name="country">The country code of the signal.</param>
        /// <param name="dynamic">The dynamic attribute value of the signal, which defines whether a signal is a dynamic signal.</param>
        /// <param name="hOffset">The heading offset of the signal.</param>
        /// <param name="height">The height of the signal.</param>
        /// <param name="id">The id of the signal.</param>
        /// <param name="name">The name of the signal.</param>
        /// <param name="orientation">The orientation of the signal, which defines in wich direction the sign is valid ("+": positive direction of the road, "-": negative direction of the road).</param>
        /// <param name="pitch">The pitch angle of the signal.</param>
        /// <param name="roll">The roll angle of the signal.</param>
        /// <param name="s">The s position of the signal.</param>
        /// <param name="subtype">The subtype identifier according to country code of the signal or "-1" / "none"..</param>
        /// <param name="t">The t position of the signal.</param>
        /// <param name="text">Additional text assiciated with the signal.</param>
        /// <param name="type">The type identifier according to country code of the signal or "-1" / "none".</param>
        /// <param name="value">The value of the signal.</param>
        /// <param name="width">The width attribute value of the signal.</param>
        /// <param name="zOffset">The zOffset from the road level.</param>
        /// <returns>The created XODRSignal object.</returns>
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