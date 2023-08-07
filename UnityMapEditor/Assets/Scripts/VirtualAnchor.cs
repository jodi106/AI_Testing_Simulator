using UnityEngine;
using System.Collections.Generic;

namespace scripts
{
    /// <summary>
    ///  This class is the VirtualAnchor class. A VirtualAnchor (or also AnchorPoint) is a point on a RoadPiece where other RoadPieces can connect to. 
    /// It is used as a virtual anchor to create a reference to other RoadPieces and to align RoadPieces to each other. 
    /// </summary>
    public class VirtualAnchor
    {
        // The Offset describes the position of the AnchorPoint relative to the center of the RoadPiece it belongs to as a 3D-Vector. 
        public Vector3 Offset;

        // The Orientation describes the orientation of the AnchorPoints relative to the rotation of the RoadPiece. This allows to distinguish positions of AnchorPoints of a RoadPiece
        public float Orientation;

        // This property maintains a reference to the RoadPiece this AnchorPoint belongs to, to create a backwards reference. 
        public RoadPiece RoadPiece;

        // TO BE DELETED AND MOVED TO ROAD MANAGER
        public List<GameObject> ChildStraightPieces = new List<GameObject>();

        // This property creates a reference to another AnchorPoint of another Road and therefore indrectly creates a reference between the RoadPieces. 
        public VirtualAnchor ConnectedAnchorPoint = null;

        /// <summary>
        /// This method is the constructor. It receives the parent RoadPiece for future backwards reference and the orientation of the AnchorPoint
        /// Also, this method automatically calculates the offset of the AnchorPoint based on its orientation and the size of the parent RoadPiece.
        /// </summary>
        /// <param name="roadPiece"> The Road piece that the Virtual Anchor belongs to</param>
        /// <param name="orientation"> the orientation of the virtual anchor relative to the road piece's orientation </param>
        public VirtualAnchor(RoadPiece roadPiece, float orientation)
        {
            this.RoadPiece = roadPiece;
            this.Orientation = orientation;

            // Match the size of a piece to the scale the RoadPiece has in the Editor UI. 
            float widthScale = roadPiece.Width * roadPiece.transform.localScale.x;
            float heightScale = roadPiece.Height * roadPiece.transform.localScale.y;

            // For the offset, the rotation is needed in Radians, rather than Degree. So the angle is formatted to Radians and saved in a variable
            float angleInRadians = Orientation * Mathf.Deg2Rad;

            // The previously formatted angle is now used to calculate the offset based on the size specifications of the RoadPiece
            float offsetX = Mathf.Cos(angleInRadians) * widthScale / 2;
            float offsetY = Mathf.Sin(angleInRadians) * heightScale / 2;

            // Initialize the offset with the values previously calculated
            this.Offset = new Vector3(offsetX, offsetY, 0);
        }

        /// <summary>
        /// As this class does not inherit "MonoBehavior", it is no class from Unity. However, it will update the orientation and offset, when the RoadPiece is rotated
        /// For example: If a RoadPiece has a rotation of 0°, then the offset is (1,0,0). But when we rotate the roadpiece by 90°, then the Offset is not (1,0,0) anymore
        /// but changes to (0,1,0).  
        /// </summary>
        /// <param name="angleOfRotation"> The angle of rotation by which the road piece is rotated </param>
        public void Update(float angleOfRotation)
        {
            //This will check whether a piece will have an orientation higher than 360°, and uses modulo to keep the range of orientation from 0°-360°
            Orientation = (Orientation + angleOfRotation) % 360;
            //This will check, whether a rotation is negative and will format the representative positive value.
            if (Orientation < 0)
            {
                Orientation = 360 + Orientation;
            }

            // Here the Offset is Updated. "Quanternion.Euler" is a method, which calculates the new Offset Vector using the angle by which we the RoadPiece by
            Offset = Quaternion.Euler(0f, 0f, angleOfRotation) * Offset;
        }

        /// <summary>
        /// This method connects a VirtualAnchor to another VirtualAnchor. It automatically creates a 2-Way-Reference as well. 
        /// </summary>
        /// <param name="va"> the Virtual Anchor to which the current anchor point should be connected to </param>
        public void ConnectAnchorPoint(VirtualAnchor va)
        {
            ConnectedAnchorPoint = va;
            va.ConnectedAnchorPoint = this;
        }

        /// <summary>
        /// This method disconnects the reference to another AnchorPoints, in case there is a connection. It automatically removes the reference of itself to the other
        /// AnchorPoint aswell. 
        /// </summary>
        public void RemoveConntectedAnchorPoint()
        {
            if (ConnectedAnchorPoint != null)
            {
                ConnectedAnchorPoint.ConnectedAnchorPoint = null;
                ConnectedAnchorPoint = null;
            }
        }

        /// <summary>
        /// This method return all straights that are added at the current stretching operation
        /// </summary>
        /// <returns> A list of straight roads, which are represented by GameObjects </returns>
        public List<GameObject> GetStretchingStraights()
        {
            return ChildStraightPieces;
        }

        /// <summary>
        /// This method adds a Gameobject to the list of straights that have been added while stretching
        /// </summary>
        /// <param name="road"> The gameobject that should be added, which represents a road </param>
        public void AddStretchingStraight(GameObject road)
        {
            ChildStraightPieces.Add(road);
        }

        /// <summary>
        /// This method removes the last Ganeobject that has been added to the list of straight roads
        /// </summary>
        public void RemoveLastStretchingStraight()
        {
            ChildStraightPieces.RemoveAt(ChildStraightPieces.Count - 1);
        }
    }
}
