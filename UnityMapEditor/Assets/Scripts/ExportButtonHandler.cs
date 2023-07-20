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

            var pieces = RoadManager.Instance.RoadList;
            foreach (var piece in pieces)
            {
                var heading = piece.Rotation;

                var startingPosition = (piece.transform.position + piece.AnchorPoints.First().Offset ) * 0.0264583333f;

                var length = piece.Width * 0.0264583333 * 5;

                Debug.Log("Starting Position: " + startingPosition);

                switch (piece.RoadType)
                {
                    case RoadType.StraightRoad:
                        builder.AddStraightRoad(startX: startingPosition.x , startY: startingPosition.y,
                                                hdg: heading, length: length);
                        break;
                    case RoadType.Crosswalk:
                        builder.AddStraightRoad(startingPosition.x, startingPosition.y, heading, length, crossing: true);
                        break;
                    case RoadType.Turn:
                        builder.Add90DegreeTurn(startingPosition.x, startingPosition.y, heading);
                        break;
                    case RoadType.ThreeWayIntersection:
                        builder.Add3wayIntersection(startingPosition.x, startingPosition.y, heading);
                        break;
                    case RoadType.FourWayIntersection:
                        builder.Add4wayIntersection(startingPosition.x, startingPosition.y, heading);
                        break;
                    case RoadType.ParkingBottom:
                        builder.AddParking(false, true, startingPosition.x, startingPosition.y, heading);
                        break;
                    case RoadType.ParkingTop:
                        builder.AddParking(true, false, startingPosition.x, startingPosition.y, heading);
                        break;
                    case RoadType.ParkingTopAndBottom:
                        builder.AddParking(true, true, startingPosition.x, startingPosition.y, heading);
                        break;
                    case RoadType.ThreeWayRoundAbout:
                        builder.Add3WayRoundAbout(startingPosition.x, startingPosition.y, heading);
                        break;
                    case RoadType.FourWayRoundAbout:
                        builder.Add4WayRoundAbout(startingPosition.x, startingPosition.y, heading);
                        break;
                    case RoadType.StraightShort: // This type should not exist
                    default:
                        break;
                }
            }

            Debug.Log("Waiting for Program to be in code");

            builder.Document.Save("OpenDrive.xodr");
        }
    }
}
