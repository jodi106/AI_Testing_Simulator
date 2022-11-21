using System.ComponentModel;

namespace Assets.Enums
{
    public enum CloudState
    {
        [Description("cloudy")]
        cloudy,
        [Description("free")]
        free,
        [Description("overcast")]
        overcast,
        [Description("rainy")]
        rainy
    }
}