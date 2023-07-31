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
}