using System.ComponentModel;

namespace Assets.Enums
{
    public enum PriorityType
    {
        //	If a starting event has priority Overwrite, all events in running state, within the same scope (maneuver) as the starting event, should be issued a stop command (stop transition).
        [Description("overwrite")]
        Overwrite,
        //If a starting event has priority Skip, then it will not be ran if there is any other event in the same scope (maneuver) in the running state.
        [Description("skip")]
        Skip,
        //Execute in parallel to other events.
        [Description("parallel")]
        Parallel

    }
}
