using System.ComponentModel;

namespace Assets.Enums
{
    /// <summary>
    /// The "AdversaryCategory" enum represents the different categories of adversarial entities that exist.
    /// </summary>
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
