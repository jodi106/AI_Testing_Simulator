using System.ComponentModel;

namespace Assets.Enums
{

    ///<summary>
    /// Defines the type of precipitation in CARLA
    ///</summary>
    public enum PrecipitationType
    {
        [Description("dry")]
        Dry,
        [Description("rain")]
        Rain
    }
}