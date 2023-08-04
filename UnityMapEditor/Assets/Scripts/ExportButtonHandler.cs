using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenDriveXMLGenerator;
using System.Linq;
using Assets.Enums;

namespace scripts
{
    /// <summary>
    /// 
    /// </summary>
    public class ExportButtonHandler : MonoBehaviour
    {

        public Button ExportButton;


        // Start is called before the first frame update
        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            ExportButton.onClick.AddListener(InitiateExport);
        }

        // Update is called once per frame
        /// <summary>
        /// 
        /// </summary>
        void Update()
        {

        }
        public void InitiateExport()
        {
            var builder = new OpenDriveXMLBuilder();

            var pieces = RoadManager.Instance.RoadList;
            foreach (var piece in pieces)
            {
                var heading = (float) (piece.Rotation * Math.PI / 180f);

                var startingPosition = (piece.transform.position + piece.AnchorPoints.First().Offset ) * 0.0264583333f * 2f;

                var length = piece.Width * 0.0264583333 * 10;

                switch (piece.RoadType)
                {
                    case RoadType.StraightRoad:
                        builder.AddStraightRoad(startX: startingPosition.x , startY: startingPosition.y,
                                                hdg: heading, length: length, trafficSign: piece.TrafficSign);
                        break;
                    case RoadType.Crosswalk:
                        builder.AddStraightRoad(startingPosition.x, startingPosition.y, heading, length, crossing: true);
                        break;
                    case RoadType.Turn:
                        builder.Add90DegreeTurn(startingPosition.x, startingPosition.y, heading);
                        break;
                    case RoadType.Turn15:
                        builder.Add15DegreeTurn(startingPosition.x, startingPosition.y, heading);
                        break;
                    case RoadType.ThreeWayIntersection:
                        builder.Add3wayIntersection(startingPosition.x, startingPosition.y, heading, traffic_light: piece.TrafficSign == TrafficSign.TrafficLight);
                        break;
                    case RoadType.FourWayIntersection:
                        builder.Add4wayIntersection(startingPosition.x, startingPosition.y, heading, traffic_light: piece.TrafficSign == TrafficSign.TrafficLight);
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
                    default:
                        break;
                }
            }

            Debug.Log("Waiting for Program to be in code");

            builder.Document.Save("OpenDrive.xodr");
        }
    }
}
