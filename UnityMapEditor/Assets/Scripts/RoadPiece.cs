using System.Collections.Generic;
using UnityEngine;

namespace scripts
{
    public class RoadPiece : MonoBehaviour
    {

        private static int idCounter = 0;
        private int id;
        public List<RoadPiece> neighbors = new List<RoadPiece>();
        public RoadType roadType;
        private bool isLocked = false;

        public VirtualAnchor lastNeighborSnappedAnchorPoint = null;
        public VirtualAnchor lastSelectedSnappedAnchorPoint = null;

        public float width;
        public float height;

        private float rotation = 0;

        public List<VirtualAnchor> anchorPoints = new List<VirtualAnchor>();

        // Awake method is called when instantiated
        void Awake()
        {
            setVirtualAnchorPoints();
        }


        // Start is called before the first frame update
        void Start()
        {
            id = idCounter++;
            RoadManager.Instance.AddRoadToList(this);
        }

        public int getID()
        {
            return this.id;
        }

        public void setIsLocked(bool value)
        {
            isLocked = value;
            RoadManager.Instance.colorRoadPiece(isLocked ? SelectionColor.lockedSelected : SelectionColor.selected);
        }

        public bool getIsLocked()
        {
            return isLocked;
        }

        public float getRotation()
        {
            return rotation;
        }

        public void setRotation(float rotation)
        {
            this.rotation = rotation;
        }

        void setVirtualAnchorPoints()
        {

            switch (roadType)
            {
                case RoadType.StraightRoad:
                case RoadType.Crosswalk:
                case RoadType.ParkingTop:
                case RoadType.ParkingBottom:
                case RoadType.ParkingTopAndBottom:
                case RoadType.StraightShort:
                    anchorPoints.Add(new VirtualAnchor(this, 0));
                    anchorPoints.Add(new VirtualAnchor(this, 180));
                    break;
                case RoadType.Turn:
                    anchorPoints.Add(new VirtualAnchor(this, 0));
                    anchorPoints.Add(new VirtualAnchor(this, 270));
                    break;
                case RoadType.ThreeWayIntersection:
                    anchorPoints.Add(new VirtualAnchor(this, 90));
                    anchorPoints.Add(new VirtualAnchor(this, 180));
                    anchorPoints.Add(new VirtualAnchor(this, 270));
                    break;
                case RoadType.FourWayIntersection:
                case RoadType.FourWayRoundAbout:
                    anchorPoints.Add(new VirtualAnchor(this, 0));
                    anchorPoints.Add(new VirtualAnchor(this, 90));
                    anchorPoints.Add(new VirtualAnchor(this, 180));
                    anchorPoints.Add(new VirtualAnchor(this, 270));
                    break;
                case RoadType.ThreeWayRoundAbout:
                    anchorPoints.Add(new VirtualAnchor(this, 0));
                    anchorPoints.Add(new VirtualAnchor(this, 180));
                    anchorPoints.Add(new VirtualAnchor(this, 270));
                    break;
                default:
                    break;
            }

        }

    }
}