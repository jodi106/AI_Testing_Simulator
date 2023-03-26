using System.ComponentModel;

namespace Assets.Enums
{
    public enum AdversaryCategory
    {
        [Description("Null")]
        Null,
        [Description("Car")]
        Car,
        [Description("Bike")]
        Bike,
        [Description("Motorcycle")]
        Motorcycle,
        [Description("Pedestrian")]
        Pedestrian
    }
}
