using System.Collections.Generic;
using UnityEngine;

namespace scripts
{
    public class RoadPiece : MonoBehaviour
    {

        private static int IdCounter = 0;
        public int Id { get; set; }
        public RoadType RoadType;

        private bool isLocked = false;
        public bool IsLocked
        {
            get { return isLocked; }
            set
            {
                isLocked = value;
                RoadManager.Instance.ColorRoadPiece(isLocked ? SelectionColor.lockedSelected : SelectionColor.selected);
            }
        }

        public VirtualAnchor LastNeighborSnappedAnchorPoint = null;
        public VirtualAnchor LastSelectedSnappedAnchorPoint = null;

        public float Width;
        public float Height;

        public float Rotation { get; set; } = 0;

        public List<VirtualAnchor> AnchorPoints = new List<VirtualAnchor>();

        // Awake method is called when instantiated
        void Awake()
        {
            PopulateVirtualAnchorPoints();
        }


        // Start is called before the first frame update
        void Start()
        {
            Id = IdCounter++;
            RoadManager.Instance.RoadList.Add(this);
            Debug.Log(this.AnchorPoints.Count != 0 ? this.AnchorPoints[0].Offset : null);
        }


        public void PopulateVirtualAnchorPoints()
        {
            AnchorPoints = new List<VirtualAnchor>();
            Debug.Log(RoadType);
            switch (RoadType)
            {
                case RoadType.StraightRoad:
                case RoadType.Crosswalk:
                case RoadType.ParkingTop:
                case RoadType.ParkingBottom:
                case RoadType.ParkingTopAndBottom:
                case RoadType.StraightShort:
                    AnchorPoints.Add(new VirtualAnchor(this, 0));
                    AnchorPoints.Add(new VirtualAnchor(this, 180));
                    break;
                case RoadType.Turn:
                    AnchorPoints.Add(new VirtualAnchor(this, 0));
                    AnchorPoints.Add(new VirtualAnchor(this, 270));
                    break;
                case RoadType.ThreeWayIntersection:
                    AnchorPoints.Add(new VirtualAnchor(this, 90));
                    AnchorPoints.Add(new VirtualAnchor(this, 180));
                    AnchorPoints.Add(new VirtualAnchor(this, 270));
                    break;
                case RoadType.FourWayIntersection:
                case RoadType.FourWayRoundAbout:
                    AnchorPoints.Add(new VirtualAnchor(this, 0));
                    AnchorPoints.Add(new VirtualAnchor(this, 90));
                    AnchorPoints.Add(new VirtualAnchor(this, 180));
                    AnchorPoints.Add(new VirtualAnchor(this, 270));
                    break;
                case RoadType.ThreeWayRoundAbout:
                    AnchorPoints.Add(new VirtualAnchor(this, 0));
                    AnchorPoints.Add(new VirtualAnchor(this, 180));
                    AnchorPoints.Add(new VirtualAnchor(this, 270));
                    break;
                default:
                    break;
            }

        }


    }
}