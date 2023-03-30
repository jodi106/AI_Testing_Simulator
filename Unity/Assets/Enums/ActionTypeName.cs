using System.ComponentModel;


/// <summary>
/// The "Assets.Enums" namespace contains the ActionTypeName enum.
/// </summary>
namespace Assets.Enums
{


    /// <summary>
    /// The "ActionTypeName" enum represents the different types of actions that can be performed.
    /// </summary>
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