using System.ComponentModel;

namespace Assets.Enums
{
    public enum TriggerType
    {
        [Description("SimulationTimeCondition")]
        SimulationTimeCondition,
        [Description("DistanceCondition")]
        DistanceCondition,
        [Description("ReachPositionCondition")]
        ReachPositionCondition,
    }
}