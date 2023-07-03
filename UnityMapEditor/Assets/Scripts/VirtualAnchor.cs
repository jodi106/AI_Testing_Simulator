using UnityEngine;
using System.Collections.Generic;

namespace scripts
{
    public class VirtualAnchor
    {
        public Vector3 Offset;
        public float Orientation;
        public RoadPiece RoadPiece;
        public List<GameObject> ChildStraightPieces = new List<GameObject>();

        public VirtualAnchor ConnectedAnchorPoint = null;

        public VirtualAnchor(RoadPiece roadPiece, float orientation)
        {
            this.RoadPiece = roadPiece;
            this.Orientation = orientation;

            float widthScale = roadPiece.width * roadPiece.transform.localScale.x;
            float heightScale = roadPiece.height * roadPiece.transform.localScale.y;

            float angleInRadians = Orientation * Mathf.Deg2Rad;

            float offsetX = Mathf.Cos(angleInRadians) * widthScale / 2;
            float offsetY = Mathf.Sin(angleInRadians) * heightScale / 2;

            this.Offset = new Vector3(offsetX, offsetY, 0);

        }

        public void Update(float angleOfRotation)
        {
            Orientation = (Orientation + angleOfRotation) % 360;
            if (Orientation < 0)
            {
                Orientation = 360 + Orientation;
            }

            Offset = Quaternion.Euler(0f, 0f, angleOfRotation) * Offset;
        }

        public void ConnectAnchorPoint(VirtualAnchor va)
        {
            ConnectedAnchorPoint = va;
            va.ConnectedAnchorPoint = this;
        }

        public void RemoveConntectedAnchorPoint()
        {
            if (ConnectedAnchorPoint != null)
            {
                ConnectedAnchorPoint.ConnectedAnchorPoint = null;
                ConnectedAnchorPoint = null;
            }
        }
        public List<GameObject> GetStretchingStraights()
        {
            return ChildStraightPieces;
        }
        public void AddStretchingStraight(GameObject road)
        {
            ChildStraightPieces.Add(road);
        }

        public void RemoveLastStretchingStraight()
        {
            ChildStraightPieces.RemoveAt(ChildStraightPieces.Count - 1);
        }


    }
}
