using Assets.Enums;
using scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace OpenDriveXMLGenerator
{
    public class XODRBase
    {
        protected XmlElement element;

        public XmlElement XmlElement { get { return element; } }

        public XmlDocument OwnerDocument
        {
            get { return element.OwnerDocument; }
        }

        public XODRBase(XmlElement element)
        {
            this.element = element;
        }

        public void AppendChild(XmlElement element)
        {
            this.element.AppendChild(element);
        }

        public void SetAttribute(string name, string? value)
        {
            this.element.SetAttribute(name, value);
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new OpenDriveXMLBuilder();

            Dictionary<Vector2, RoadPiece> pieces = new();

            foreach (var (startingPosition, piece) in pieces)
            {
                var heading = piece.AnchorPoints.First().Orientation + 3 * Mathf.PI / 2;

                switch (piece.RoadType)
                {
                    case RoadType.StraightRoad:
                        builder.AddStraightRoad(startingPosition.x, startingPosition.y, heading, 1);
                        break;
                    case RoadType.Crosswalk:
                        builder.AddStraightRoad(startingPosition.x, startingPosition.y, heading, 1, crossing: true);
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
                        throw new Exception("Not implemented");
                        break;
                    case RoadType.ParkingTop:
                        throw new Exception("Not implemented");
                        break;
                    case RoadType.ParkingTopAndBottom:
                        throw new Exception("Not implemented");
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

        builder.Document.Save("OpenDrive.xodr");

        }

        static void CreateBoschMap(OpenDriveXMLBuilder builder)
        {
            var road1 = builder.AddStraightRoad(startX: -10, startY: 0, length: 20);
            builder.Add3wayIntersection(startX: 10, startY: 0);
            var road2 = builder.AddStraightRoad(startX: 28, startY: 0, length: 20);
            builder.Add3wayIntersection(48, 0);
            var road3 = builder.AddStraightRoad(startX: 66, startY: 0, length: 20);
            builder.Add3wayIntersection(86, 0);
            var road4 = builder.AddStraightRoad(startX: 104, startY: 0, length: 60);
            var road5 = builder.Add90DegreeTurn(startX: 169, startY: -5, hdg: 1.5707963267949);
            var road6 = builder.AddStraightRoad(startX: 169, startY: -5, hdg: -1.5707963267949f, length: 200);
            var road7 = builder.Add90DegreeTurn(startX: 164, startY: -210);
            var road8 = builder.AddStraightRoad(startX: 164, startY: -210, length: 30, hdg: 3.1415f);
            var road9 = builder.Add90DegreeTurn(startX: 129, startY: -205, hdg: -1.5707963267949);
            builder.Add3wayIntersection(startX: 19, startY: -27, hdg: 1.57f);
            builder.Add3wayIntersection(startX: 19, startY: -45, hdg: 1.57f);
            var road12 = builder.AddStraightRoad(startX: 28, startY: -18, length: 20);
            builder.Add4wayIntersection(startX: 48, startY: -18);
            var road13 = builder.AddStraightRoad(startX: 66, startY: -18, length: 20);
            builder.Add3wayIntersection(startX: 95, startY: -9, hdg: -1.57f);
            var road14 = builder.AddStraightRoad(startX: 28, startY: -36, length: 20);
            builder.Add3wayIntersection(startX: 66, startY: -36, hdg: -3.1415f);
            var road15 = builder.AddStraightRoad(startX: 66, startY: -36, length: 20);
            builder.Add3wayIntersection(startX: 104, startY: -36, hdg: -3.1415f);
            var road16 = builder.AddStraightRoad(startX: 104, startY: -36, length: 20);
            var road17 = builder.Add90DegreeTurn(startX: 129, startY: -41, hdg: 1.5707963267949);
            var road18 = builder.AddStraightRoad(startX: 129, startY: -41, hdg: -1.5707963267949f, length: 59);
            var road19 = builder.AddStraightRoad(startX: 129, startY: -205, hdg: 1.5707963267949f, length: 65);
            var road20 = builder.AddStraightRoad(startX: 24, startY: -120, hdg: 0, length: 85);
            var road10 = builder.AddStraightRoad(startX: 19, startY: -44, hdg: -1.5707963267949f, length: 71);
            var road11 = builder.Add90DegreeTurn(startX: 19, startY: -115, hdg: -1.5707963267949);
            builder.Add3WayRoundAbout(startX: 129, startY: -100, hdg: -1.5707963267949f);
        }
    }
}
