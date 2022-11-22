using System.ComponentModel;

namespace Assets.Enums
{
    public enum CloudState
    {
        [Description("cloudy")]
        Cloudy,
        [Description("free")]
        Free,
        [Description("overcast")]
        Overcast,
        [Description("rainy")]
        Rainy
    }
}