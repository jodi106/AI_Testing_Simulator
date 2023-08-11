using System.ComponentModel;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class inherits from the class XODRBase and represents a signal element in an Opendrive file. .
    /// </summary>
    public class XODRSignal : XODRBase
    {
        /// <summary>
        /// The default constructor of the class XODRSignal.
        /// </summary>
        /// <param name="element">The XML element of the signal.</param>
        public XODRSignal(XmlElement element) : base(element) { }
    }
}