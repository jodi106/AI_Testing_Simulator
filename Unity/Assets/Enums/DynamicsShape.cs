using System.ComponentModel;

namespace Assets.Enums
{
    public enum DynamicsShape
    {
        [Description("linear")]
        SpeedAction,
        [Description("step")]
        LaneChangeAction,
        [Description("cubic")]
        Cubic, // not sure if cubic is supported TODO
        [Description("sinusoidal")]
        Sinusoidal // not sure if sinusoidal is supported TODO
    }
}