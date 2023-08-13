using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using OpenDriveXMLGenerator;
using System.Linq;
using Assets.Enums;

namespace scripts
{
    /// <summary>
    /// This class will handle the export button functionality
    /// </summary>
    public class ExportButtonHandler : MonoBehaviour
    {

        public Button ExportButton;
        public GameObject SuccessMessage;


        /// <summary>
        /// This method is called before the first frame update. It will add the export button on click functionality. 
        /// </summary>
        void Start()
        {
            ExportButton.onClick.AddListener(InitiateExport);
        }

        /// <summary>
        /// This will intitiate the export to create a XODR file 
        /// </summary>
        public async void InitiateExport()
        {
            var builder = new OpenDriveXMLBuilder();

            var pieces = RoadManager.Instance.RoadList;
            foreach (var piece in pieces)
            {
                var heading = (float)(piece.Rotation * Math.PI / 180f);

                var startingPosition = (piece.transform.position + piece.AnchorPoints.First().Offset) * 0.0264583333f * 2f;

                var length = piece.Width * 0.0264583333 * 10;

                switch (piece.RoadType)
                {
                    case RoadType.StraightRoad:
                        builder.AddStraightRoad(startX: startingPosition.x, startY: startingPosition.y,
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

            StartCoroutine(ShowSuccessMessage());
            int year = System.DateTime.Now.Year;
            int month = System.DateTime.Now.Month;
            int day = System.DateTime.Now.Day;
            int hour = System.DateTime.Now.Hour;
            int minute = System.DateTime.Now.Minute;
            int second = System.DateTime.Now.Second;

            string path = Path.Combine(Environment.CurrentDirectory, "Maps");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // builder.Document.Save(path + "/" + year + "_" + month + "_" + day + " - " + hour + ":" + minute + ":" + second);
            string documentFileName = $"{year}_{month:D2}_{day:D2} - {hour:D2}_{minute:D2}_{second:D2}.xodr";
            string documentPath = Path.Combine(path, documentFileName);
            builder.Document.Save(documentPath);
        }

        public System.Collections.IEnumerator ShowSuccessMessage()
        {
            SuccessMessage.GetComponent<CanvasGroup>().alpha = 1;
            yield return new WaitForSeconds(5);
            SuccessMessage.GetComponent<CanvasGroup>().alpha = 0;
        }
    }
}
