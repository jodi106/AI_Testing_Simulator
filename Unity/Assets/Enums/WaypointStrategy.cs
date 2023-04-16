using System.ComponentModel;

namespace Assets.Enums
{
    ///<summary>
    ///Defines the type of strategy of a waypoint
    public enum WaypointStrategy
    {
        [Description("Fastest")]
        FASTEST,
        [Description("Shortest")]
        SHORTEST,
    }
}