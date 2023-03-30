using System.ComponentModel;

namespace Assets.Enums
{

    ///<summary>
    ///Defines the type of a trigger that is used to trigger an event
    public enum TriggerType
    {
        [Description("SimulationTimeCondition")]
        SimulationTimeCondition,
        [Description("DistanceCondition")]
        DistanceCondition,
        [Description("ReachPositionCondition")]
        ReachPositionCondition,
        [Description("RelativeDistanceCondition")]
        RelativeDistanceCondition,
    }
}