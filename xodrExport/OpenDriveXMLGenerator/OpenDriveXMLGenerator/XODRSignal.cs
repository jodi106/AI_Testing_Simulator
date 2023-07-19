using System.ComponentModel;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODRSignal : XODRBase
    {
        public XODRSignal(XmlElement element) : base(element) { }
    }

    public enum TrafficSignal
    {
        [Description("none")]
        None,

        [Description("stop")]
        Stop,

        [Description("speed30")]
        Speed30,

        [Description("speed60")]
        Speed60,

        [Description("speed90")]
        Speed90,

        [Description("yield")]
        Yield
    }

    public static class TrafficSignalExtensions
    {
        public static string GetDescription(this TrafficSignal trafficSignal)
        {
            var fieldInfo = trafficSignal.GetType().GetField(trafficSignal.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return trafficSignal.ToString();
        }
    }



}