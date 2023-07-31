using Assets.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace scripts
{
    /*
    * This class is the RoadPiece Class. It contains the necessary properties for RoadPieces. 
    * A RoadPiece is a GameObject from Unity, which has the RoadPiece properties 
    */
    public class RoadPiece : MonoBehaviour
    {

        private static int IdCounter = 0;
        public int Id { get; set; }

        // This property describes, which type of road this RoadPiece is (e.g., Straight, Turn, Intersection etc.)
        public RoadType RoadType;

        // The user can lock a road, so no actions can be performed on it. This boolean states, whether the RoadPiece is locked
        private bool isLocked = false;
        // This is an extension of the previous boolean. When a piece is (un)locked, its color changes on the interface. 
        public bool IsLocked
        {
            get { return isLocked; }
            set
            {
                isLocked = value;
                RoadManager.Instance.ColorRoadPiece(this, SelectionColor.normal);
            }
        }

        // TODO
        public VirtualAnchor LastNeighborSnappedAnchorPoint = null;
        //TODO
        public VirtualAnchor LastSelectedSnappedAnchorPoint = null;

        // This is a manual width, which is necessary, because rotating will change the width given from Unity
        public float Width;
        // This is a manual height, which is necessary, because rotating will change the height given from Unity
        public float Height;

        // This property displays the rotation of the RoadPiece as a float for easy reference
        public float Rotation { get; set; } = 0;

        /*
        * This List describes the AnchorPoints (or here VirtualAnchors) of a RoadPiece. An AnchorPoint describes the point on a road where other roads connect to
        * For example: A 4-Way Intersection has 4 Anchorpoints --> Top, Right, Bottom and Left, because there are 4 places other roads can connect to. 
        */
        public List<VirtualAnchor> AnchorPoints = new List<VirtualAnchor>();

        /*
        * This property refers to an enum, that will hold the value for the traffic sign, this piece is holding
        */
        public TrafficSign TrafficSign = TrafficSign.None;

        /* 
        * The Awake method is a "Monobehavior" method from Unity, which is automaticlly called when instantiated. 
        * This method will call "PopulateVirtualAnchorPoints"
        */
        void Awake()
        {
            PopulateVirtualAnchorPoints();
        }


        /* 
        * The Start method is a "MonoBehavior" method from Unity, which is automatically called before the first frame update
        *   This method will increase the count of RoadPieces and add the Road to a List of all Roads in the RoadManager
        */
        void Start()
        {
            Id = IdCounter++;
            RoadManager.Instance.RoadList.Add(this);
        }

        /*
        *   This method initializes the AnchorPoints for a RoadPiece based on its RoadType. For example, a Straight will have 2 AP and an Intersection can have 3 or 4 AP. 
        *   Also, each Anchor Points will receive an orientation based on its location of the RoadPiece
        */
        public void PopulateVirtualAnchorPoints()
        {
            AnchorPoints = new List<VirtualAnchor>();
            switch (RoadType)
            {
                case RoadType.StraightRoad:
                case RoadType.Crosswalk:
                case RoadType.ParkingTop:
                case RoadType.ParkingBottom:
                case RoadType.ParkingTopAndBottom:
                    AnchorPoints.Add(new VirtualAnchor(this, 180));
                    AnchorPoints.Add(new VirtualAnchor(this, 0));
                    break;
                case RoadType.Turn:
                    AnchorPoints.Add(new VirtualAnchor(this, 180));
                    AnchorPoints.Add(new VirtualAnchor(this, 270));
                    break;
                case RoadType.ThreeWayIntersection:
                    AnchorPoints.Add(new VirtualAnchor(this, 180));
                    AnchorPoints.Add(new VirtualAnchor(this, 270));
                    AnchorPoints.Add(new VirtualAnchor(this, 0));
                    break;
                case RoadType.FourWayIntersection:
                case RoadType.FourWayRoundAbout:
                    AnchorPoints.Add(new VirtualAnchor(this, 180));
                    AnchorPoints.Add(new VirtualAnchor(this, 270));
                    AnchorPoints.Add(new VirtualAnchor(this, 0));
                    AnchorPoints.Add(new VirtualAnchor(this, 90));
                    break;
                case RoadType.ThreeWayRoundAbout:
                    AnchorPoints.Add(new VirtualAnchor(this, 180));
                    AnchorPoints.Add(new VirtualAnchor(this, 270));
                    AnchorPoints.Add(new VirtualAnchor(this, 0));
                    break;
                case RoadType.Turn15:
                    AnchorPoints.Add(new VirtualAnchor(this, 180));
                    AnchorPoints.Add(new VirtualAnchor(this, 345));
                    break;
                default:
                    break;
            }
        }
    }
}