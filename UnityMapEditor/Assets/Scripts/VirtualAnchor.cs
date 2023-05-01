using UnityEngine;

namespace scripts
{
    public class VirtualAnchor
    {
        public Vector3 offset;
        public float orientation;
        public RoadPiece referencedRoadPiece;


        public VirtualAnchor connectedAnchorPoint = null;

        public VirtualAnchor(RoadPiece referencedRoadPiece, float orientation)
        {
            this.referencedRoadPiece = referencedRoadPiece;
            this.orientation = orientation;

            switch (orientation)
            {
                case 0:
                    this.offset = new Vector3(referencedRoadPiece.width * referencedRoadPiece.transform.localScale.x / 2, 0, 0);
                    break;
                case 90:
                    this.offset = new Vector3(0, referencedRoadPiece.height * referencedRoadPiece.transform.localScale.y / 2, 0);
                    break;
                case 180:
                    this.offset = new Vector3(-referencedRoadPiece.width * referencedRoadPiece.transform.localScale.x / 2, 0, 0);
                    break;
                case 270:
                    this.offset = new Vector3(0, -referencedRoadPiece.height * referencedRoadPiece.transform.localScale.y / 2, 0);
                    break;
                default:
                    Debug.Log("Error when initializing offset. Please make sure, that this Piece is of Type RoadPiece! ");
                    break;
            }
        }

        public void Update(float angleOfRotation)
        {
            orientation = (orientation + angleOfRotation) % 360;
            if (orientation < 0)
            {
                orientation = 360 + orientation;
            }

            offset = Quaternion.Euler(0f, 0f, angleOfRotation) * offset;
        }

        public void ConnectAnchorPoint(VirtualAnchor va)
        {
            connectedAnchorPoint = va;
            va.connectedAnchorPoint = this;
        }

        public void RemoveConntectedAnchorPoint()
        {
            connectedAnchorPoint = null;
        }


    }
}
