using System.ComponentModel;

namespace Assets.Enums
{

    /// <summary>
    /// The "CloudState" enum represents the different states of the clouds and sky in CARLA
    /// </summary>
    public enum CloudState
    {
        [Description("Cloudy")]
        Cloudy,
        [Description("Free")]
        Free,
        [Description("Overcast")]
        Overcast,
        [Description("Rainy")]
        Rainy
    }
}