using System.ComponentModel;

namespace Assets.Enums
{
    public enum ActionTypeName
    {
        [Description("SpeedAction")]
        SpeedAction,
        [Description("StopAction")]
        StopAction,
        [Description("LaneChangeAction")]
        LaneChangeAction,
        [Description("AssignRouteAction")]
        AssignRouteAction
    }
}