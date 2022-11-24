using System.ComponentModel;

namespace Assets.Enums
{
    public enum ActionTypeName
    {
        [Description("SpeedAction")]
        SpeedAction,
        [Description("BreakAction")]
        BreakAction,
        [Description("LaneChangeAction")]
        LaneChangeAction,
        [Description("AssignRouteAction")]
        AssignRouteAction
    }
}