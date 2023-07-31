using Assets.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace scripts
{
    public class PrefabManager : MonoBehaviour
    {
        private static PrefabManager instance;
        public static PrefabManager Instance
        {
            get
            {
                return instance;
            }
        }

        public RoadPiece Straight;
        public RoadPiece Turn;
        public RoadPiece Turn15;
        public RoadPiece ThreeWayIntersection;
        public RoadPiece FourWayIntersection;
        public RoadPiece ParkingBottom;
        public RoadPiece ParkingTopAndBottom;
        public RoadPiece ParkingTop;
        public RoadPiece Crosswalk;
        public RoadPiece FourWayRoundAbout;
        public RoadPiece ThreeWayRoundAbout;
        public RoadPiece StraightShort;
        public RoadPiece None;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public RoadPiece GetPieceOfType(RoadType roadType)
        {
            switch (roadType)
            {
                case RoadType.StraightRoad:
                    return Straight;
                case RoadType.Turn:
                    return Turn;
                case RoadType.Turn15:
                    return Turn15;
                case RoadType.ThreeWayIntersection:
                    return ThreeWayIntersection;
                case RoadType.FourWayIntersection:
                    return FourWayIntersection;
                case RoadType.ParkingBottom:
                    return ParkingBottom;
                case RoadType.ParkingTop:
                    return ParkingTop;
                case RoadType.ParkingTopAndBottom:
                    return ParkingTopAndBottom;
                case RoadType.Crosswalk:
                    return Crosswalk;
                case RoadType.FourWayRoundAbout:
                    return FourWayRoundAbout;
                case RoadType.ThreeWayRoundAbout:
                    return ThreeWayRoundAbout;
                case RoadType.StraightShort:
                    return StraightShort;
                case RoadType.None:
                    return None;
                default:
                    return null;
            }
        }
    }
}
