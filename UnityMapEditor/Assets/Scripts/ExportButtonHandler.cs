using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenDriveXMLGenerator;
using System.Linq;
using Assets.Enums;

namespace scripts
{
    public class ExportButtonHandler : MonoBehaviour
    {

        public Button ExportButton;
        // Start is called before the first frame update
        void Start()
        {
            ExportButton.onClick.AddListener(InitiateExport);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void InitiateExport()
        {
            var builder = new OpenDriveXMLBuilder();

            Dictionary<Vector2, RoadPiece> pieces = new();

            foreach (var (centerPosition, piece) in pieces)
            {
                var heading = piece.AnchorPoints.First().Orientation + 3 * Mathf.PI / 2;

                var predecessors = new SequenceInfo
                {
                    IsJunction = piece.RoadType.IsJunction()
                };
                
                

                switch (piece.RoadType)
                {
                    case RoadType.StraightRoad:
                        builder.AddStraightRoad(startX: centerPosition.x, startY: centerPosition.y, 
                                                hdg: heading, length: 1, predecessorInfo:  ,successorInfo:);
                        break;
                    case RoadType.Crosswalk:
                        builder.AddStraightRoad(centerPosition.x, centerPosition.y, heading, 1, crossing: true);
                        break;
                    case RoadType.Turn:
                        builder.Add90DegreeTurn(centerPosition.x, centerPosition.y, heading);
                        break;
                    case RoadType.ThreeWayIntersection:
                        builder.Add3wayIntersection(centerPosition.x, centerPosition.y, heading);
                        break;
                    case RoadType.FourWayIntersection:
                        builder.Add4wayIntersection(centerPosition.x, centerPosition.y, heading);
                        break;
                    case RoadType.ParkingBottom:
                        throw new Exception("Not implemented");
                        break;
                    case RoadType.ParkingTop:
                        throw new Exception("Not implemented");
                        break;
                    case RoadType.ParkingTopAndBottom:
                        throw new Exception("Not implemented");
                        break;
                    case RoadType.ThreeWayRoundAbout:
                        builder.Add3WayRoundAbout(centerPosition.x, centerPosition.y, heading);
                        break;
                    case RoadType.FourWayRoundAbout:
                        builder.Add4WayRoundAbout(centerPosition.x, centerPosition.y, heading);
                        break;
                    case RoadType.StraightShort: // This type should not exist
                    default:
                        break;
                }
            }

            Debug.Log("Waiting for Program to be in code");
        }
    }
}
