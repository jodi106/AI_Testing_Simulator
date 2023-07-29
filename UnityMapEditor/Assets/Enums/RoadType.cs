using System.ComponentModel;

namespace Assets.Enums
{
    public enum RoadType
    {
        [Description("None")]
        None,
        [Description("StraightRoad")]
        StraightRoad,
        [Description("Turn")]
        Turn,
        [Description("Turn15")]
        Turn15,
        [Description("ThreeWayIntersection")]
        ThreeWayIntersection,
        [Description("FourWayIntersection")]
        FourWayIntersection,
        [Description("ParkingBottom")]
        ParkingBottom,
        [Description("ParkingTop")]
        ParkingTop,
        [Description("ParkingTopAndBottom")]
        ParkingTopAndBottom,
        [Description("Crosswalk")]
        Crosswalk,
        [Description("FourWayRoundAbout")]
        FourWayRoundAbout,
        [Description("ThreeWayRoundAbout")]
        ThreeWayRoundAbout,
        [Description("StraightShort")]
        StraightShort
    }

    public static class RoadTypeExtensions
    {
        public static bool IsJunction(this RoadType roadType)
        {
            switch (roadType)
            {
                case RoadType.ThreeWayIntersection:
                case RoadType.FourWayIntersection:
                case RoadType.FourWayRoundAbout:
                case RoadType.ThreeWayRoundAbout:
                    return true;
                default:
                    return false;
            }
        }
    }
}