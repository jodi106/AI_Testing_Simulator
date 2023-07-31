using System.ComponentModel;

namespace Assets.Enums
{
    public enum TrafficSign
    {
        [Description("none")]
        None,
        [Description("stop")]
        Stop,
        [Description("yield")]
        Yield,
        [Description("speedlimit30")]
        Limit30,
        [Description("speedlimit60")]
        Limit60,
        [Description("speedlimit90")]
        Limit90,
        [Description("trafficlight")]
        TrafficLight,
    }

    public static class TrafficSignExtensions
    {
        public static string GetDescription(this TrafficSign TrafficSign)
        {
            var fieldInfo = TrafficSign.GetType().GetField(TrafficSign.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return TrafficSign.ToString();
        }
    }
}