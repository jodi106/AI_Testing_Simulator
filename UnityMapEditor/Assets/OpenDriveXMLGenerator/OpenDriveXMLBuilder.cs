using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class OpenDriveXMLBuilder
    {
        public XmlDocument Document { get; set; }
        public XmlElement RootElement { get; set; }

        private int id = 0;

        private int junctionId = 0;

        private int connectionId = 0;



        public OpenDriveXMLBuilder()
        {
            Document = new XmlDocument();
            RootElement = Document.CreateElement("OpenDRIVE");
            Document.AppendChild(RootElement);

            var rootElement = this.RootElement;

            rootElement.AddHeaderElement(
                revMajor: "1",
                revMinor: "4",
                name: "",
                version: "",
                north: "3.4040445389524979e+2",
                south: "-2.8943958368362757e+2",
                east: "3.3085867381572922e+2",
                west: "-3.1262489222340042e+2",
                vendor: "VectorZero");

            rootElement.AddGeoReferenceElement(
                geoRef: "+proj=tmerc +lat_0=0 +lon_0=0 +k=1 +x_0=0 +y_0=0 +datum=WGS84 +units=m " +
                        "+geoidgrids=egm96_15.gtx +vunits=m +no_defs");

            var userDataElement = rootElement.AddUserDataElement();

            userDataElement.AddVectorSceneElement(program: "RoadRunner", version: "2019.2.12 (build 5161c15)");
        }

        /**
         * Generates a straight piece of road
         */
        public XODRRoad AddStraightRoad(float startX = 0, float startY = 0, float hdg = 0, double length = 0,
            bool crossing = false, float crossingLength = 0.0f, float crossingWidth = 0.0f, string laneWidth = "3.5",
            SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: length.ToString(),
                id: id.ToString(),
                junction: "-1");

            id++;

            var link = road.AddLinkElement();
            if (predecessorInfo != null)
            {
                    if (predecessorInfo.IsJunction == true)
                    {
                        foreach (var id in predecessorInfo.Ids)
                        {
                            var predecessor = link.AddPredecessor("junction", id.ToString());
                        }
                    }
                    else
                    {
                        foreach (var id in predecessorInfo.Ids)
                        {
                            var predecessor = link.AddPredecessor("road", id.ToString());
                        }
                    }
            }

            if (successorInfo != null)
            {
                    if (successorInfo.IsJunction == true)
                    {
                        foreach (var id in successorInfo.Ids)
                        {
                            var successor = link.AddSuccessor("junction", id.ToString());
                        }
                    }
                    else
                    {
                        foreach (var id in successorInfo.Ids)
                        {
                            var successor = link.AddSuccessor("road", id.ToString());

                        }
                    }
            }

            var plainView = road.AddPlainViewElement();
            var geometry1 = plainView.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(CultureInfo.InvariantCulture),
                y: startY.ToString(),
                hdg: hdg.ToString(),
                length: length.ToString(CultureInfo.InvariantCulture));

            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var left = laneSection.AddDirectionElement(Direction.Left);
            var laneLeftSidewalk = left.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk.AddLinkElement();
                laneLeftSidewalk.AddWidthElement(a: "1.5");
            var laneLeft = left.AddLaneElement(id: "1", type: "driving", level: "false");
            var laneLeftLink = laneLeft.AddLinkElement();
            if (predecessorInfo != null)
            {
                    foreach (int leftLaneId in predecessorInfo.leftLaneIds)
                    {
                        laneLeftLink.AddLanePredecessor(leftLaneId.ToString());
                    }
            }

            if (successorInfo != null)
            {
                    foreach (int leftLaneId in successorInfo.leftLaneIds)
                    {
                        laneLeftLink.AddLaneSuccessor(leftLaneId.ToString());
                    }
            }

            laneLeft.AddWidthElement(a: laneWidth);
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Right);
            var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
            var laneRightLink = laneRight.AddLinkElement();
            if (predecessorInfo != null)
            {
                    foreach (int rightLaneId in predecessorInfo.rightLaneIds)
                    {
                        laneRightLink.AddLanePredecessor(rightLaneId.ToString());
                    }
            }

            if (successorInfo != null)
            {
                    foreach (int rightLaneId in successorInfo.rightLaneIds)
                    {
                        laneRightLink.AddLaneSuccessor(rightLaneId.ToString());
                    }
            }

            laneRight.AddWidthElement(a: laneWidth);
            var laneRightSidewalk = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
            laneRightSidewalk.AddLinkElement();
            laneRightSidewalk.AddWidthElement(a: "1.5");
            

            if (crossing)
            {

                if (crossingLength > 6)
                {
                    throw new ArgumentOutOfRangeException(nameof(crossingLength),
                        "Crosswalk length must pe smaller then road width");
                }


                var objects = road.AddObjectsElement();
                var obj = objects.AddObjectElement(s: (length / 2).ToString(), t: "0", zOffset: "0");
                var outline = obj.AddOutlineElement();
                /*
                    The order of the corners is important, it draws the shape sequential.
                */
                if (crossingLength == 0.0f || crossingWidth == 0.0f)
                {
                    outline.AddCornerLocalElement((-3).ToString(), (-1.5).ToString());
                    outline.AddCornerLocalElement((-3).ToString(), 1.5.ToString());
                    outline.AddCornerLocalElement(3.ToString(), 1.5.ToString());
                    outline.AddCornerLocalElement(3.ToString(), (-1.5).ToString());

                }
                else
                {
                    outline.AddCornerLocalElement((-crossingLength / 2).ToString(), (-crossingWidth / 2).ToString());
                    outline.AddCornerLocalElement((-crossingLength / 2).ToString(), (crossingWidth / 2).ToString());
                    outline.AddCornerLocalElement((crossingLength / 2).ToString(), (crossingWidth / 2).ToString());
                    outline.AddCornerLocalElement((crossingLength / 2).ToString(), (-crossingWidth / 2).ToString());
                }


            }


            return road;
        }

        public XODRRoad Add15DegreeTurn(float startX = 0, float startY = 0, double hdg = 0.0,
            SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "1.3089411479932687",
                id: id.ToString(),
                junction: "-1");

            id++;

            var link = road.AddLinkElement();
            if (predecessorInfo != null)
            {
                if (predecessorInfo.IsJunction == true)
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("road", id.ToString());
                    }
                }
            }

            if (successorInfo != null)
            {
                if (successorInfo.IsJunction == true)
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("road", id.ToString());

                    }
                }
            }

            var plainView = road.AddPlainViewElement();
            var geometry1 = plainView.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(),
                y: startY.ToString(),
                hdg: hdg.ToString(),
                length: "1.3089411479932687",
                curvature: "0.19999960000121728");

            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var left = laneSection.AddDirectionElement(Direction.Left);
            var laneLeftSidewalk = left.AddLaneElement(id: "2", type: "sidewalk", level: "false");
            laneLeftSidewalk.AddLinkElement();
            laneLeftSidewalk.AddWidthElement(a: "1.5");
            var laneLeft = left.AddLaneElement(id: "1", type: "driving", level: "false");
            var laneLeftLink = laneLeft.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int leftLaneId in predecessorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLanePredecessor(leftLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int leftLaneId in successorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLaneSuccessor(leftLaneId.ToString());
                }
            }

            laneLeft.AddWidthElement();
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Right);
            var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
            var laneRightLink = laneRight.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int rightLaneId in predecessorInfo.leftLaneIds)
                {
                    laneRightLink.AddLanePredecessor(rightLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int rightLaneId in successorInfo.leftLaneIds)
                {
                    laneRightLink.AddLaneSuccessor(rightLaneId.ToString());
                }
            }

            laneRight.AddWidthElement();
            var laneRightSidewalk = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
            laneRightSidewalk.AddLinkElement();
            laneRightSidewalk.AddWidthElement(a: "1.5");

            return road;
        }

        public XODRRoad Add90DegreeTurn(float startX = 0, float startY = 0, double hdg = 0.0,
            SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "7.8538002104133504",
                id: id.ToString(),
                junction: "-1");

            id++;

            var link = road.AddLinkElement();
            if (predecessorInfo != null)
            {
                if (predecessorInfo.IsJunction == true)
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("road", id.ToString());
                    }
                }
            }

            if (successorInfo != null)
            {
                if (successorInfo.IsJunction == true)
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("road", id.ToString());

                    }
                }
            }

            var plainView = road.AddPlainViewElement();
            var geometry1 = plainView.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(),
                y: startY.ToString(),
                hdg: hdg.ToString(),
                length: "7.8538002104133504",
                curvature: "0.19999960000121791");

            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var left = laneSection.AddDirectionElement(Direction.Left);
            var laneLeftSidewalk = left.AddLaneElement(id: "2", type: "sidewalk", level: "false");
            laneLeftSidewalk.AddLinkElement();
            laneLeftSidewalk.AddWidthElement(a: "1.5");
            var laneLeft = left.AddLaneElement(id: "1", type: "driving", level: "false");
            var laneLeftLink = laneLeft.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int leftLaneId in predecessorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLanePredecessor(leftLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int leftLaneId in successorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLaneSuccessor(leftLaneId.ToString());
                }
            }

            laneLeft.AddWidthElement();
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Right);
            var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
            var laneRightLink = laneRight.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int rightLaneId in predecessorInfo.leftLaneIds)
                {
                    laneRightLink.AddLanePredecessor(rightLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int rightLaneId in successorInfo.leftLaneIds)
                {
                    laneRightLink.AddLaneSuccessor(rightLaneId.ToString());
                }
            }

            laneRight.AddWidthElement();
            var laneRightSidewalk = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
            laneRightSidewalk.AddLinkElement();
            laneRightSidewalk.AddWidthElement(a: "1.5");

            return road;
        }

        public XODRRoad AddRightCurveToIntersection(XODRJunction junction, float startX = 0, float startY = 0,
            double hdg = 0, SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorInfo.Ids.ElementAt(0).ToString(),
                connectingRoadId: id.ToString());
            connectionId++;

            var curve = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "13.41",
                id: id.ToString(),
                junction: junctionId.ToString());
            id++;

            var link = curve.AddLinkElement();
            if (predecessorInfo != null)
            {
                if (predecessorInfo.IsJunction == true)
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("road", id.ToString());
                    }
                }
            }

            if (successorInfo != null)
            {
                if (successorInfo.IsJunction == true)
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("road", id.ToString());

                    }
                }
            }

            var plainView = curve.AddPlainViewElement();
            var geometry1 = plainView.AddGeometryElement(
                s: "0",
                x: (startX).ToString(),
                y: (startY).ToString(),
                hdg: hdg.ToString(),
                length: "0.14583694396573854");
            var geometry2 = plainView.AddGeometryElement(
                s: "0.14583694396573854",
                x: (startX + 0.1458369439657385f * (float)Math.Cos(hdg)).ToString(),
                y: (startY + 0.1458369439657385f * (float)Math.Sin(hdg)).ToString(),
                hdg: hdg.ToString(),
                length: "13.122688641864244",
                curvature: "-0.11970079986381091");
            var geometry3 = plainView.AddGeometryElement(
                s: "13.268525585829982",
                x: (startX + 8.35416305603426f * (float)Math.Cos(hdg - 1.5707963267948966) -
                    8.5f * (float)Math.Sin(hdg - 1.5707963267948966)).ToString(),
                y: (startY + 8.35416305603426f * (float)Math.Sin(hdg - 1.5707963267948966) +
                    8.5f * (float)Math.Cos(hdg - 1.5707963267948966)).ToString(),
                hdg: (hdg - 1.5707963267948966).ToString(),
                length: "0.1455");
            var lanes = curve.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Right);
            var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
            var laneRightLink = laneRight.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int rightLaneId in predecessorInfo.leftLaneIds)
                {
                    laneRightLink.AddLanePredecessor(rightLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int rightLaneId in successorInfo.leftLaneIds)
                {
                    laneRightLink.AddLaneSuccessor(rightLaneId.ToString());
                }
            }

            laneRight.AddWidthElement();
            var laneRightSidewalk = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
            laneRightSidewalk.AddLinkElement();
            laneRightSidewalk.AddWidthElement(a: "1.5");

            return curve;
        }

        public XODRRoad AddLeftCurveToIntersection(XODRJunction junction, float startX = 0, float startY = 0,
            double hdg = 0, SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorInfo.Ids.ElementAt(0).ToString(),
                connectingRoadId: id.ToString());
            connectionId++;

            var curve = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "14.679668513160259",
                id: id.ToString(),
                junction: junctionId.ToString());
            id++;

            var link = curve.AddLinkElement();
            if (predecessorInfo != null)
            {
                if (predecessorInfo.IsJunction == true)
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("road", id.ToString());
                    }
                }
            }

            if (successorInfo != null)
            {
                if (successorInfo.IsJunction == true)
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("road", id.ToString());

                    }
                }
            }

            var plainView = curve.AddPlainViewElement();
            var geometry1 = plainView.AddGeometryElement(
                s: "0",
                x: (startX).ToString(),
                y: (startY).ToString(),
                hdg: hdg.ToString(),
                length: "2.9999999999999991");
            var geometry2 = plainView.AddGeometryElement(
                s: "2.9999999999999991",
                x: (startX + 3.0002131237663852 * (float)Math.Cos(hdg)).ToString(),
                y: (startY + 3.0002131237663852 * (float)Math.Sin(hdg)).ToString(),
                hdg: hdg.ToString(),
                length: "0.094151957623209270");
            var geometry3 = plainView.AddGeometryElement(
                s: "3.0941519576232084",
                x: (startX + 3.0943650813895935 * (float)Math.Cos(hdg)).ToString(),
                y: (startY + 3.0943650813895935 * (float)Math.Sin(hdg)).ToString(),
                hdg: hdg.ToString(),
                length: "8.49115147414745414",
                curvature: "-0.1849921452440712");
            var geometry4 = plainView.AddGeometryElement(
                s: "11.585303431770662",
                x: (startX + 5.4056349186104047f * (float)Math.Cos(hdg - 1.5707963267948966) -
                    8.5f * (float)Math.Sin(hdg - 1.5707963267948966)).ToString(),
                y: (startY + 5.4056349186104047f * (float)Math.Sin(hdg - 1.5707963267948966) +
                    8.5f * (float)Math.Cos(hdg - 1.5707963267948966)).ToString(),
                hdg: (hdg - 1.5707963267948970).ToString(),
                length: "3.0943650813895953");

            var lanes = curve.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var left = laneSection.AddDirectionElement(Direction.Left);
            var laneLeft = left.AddLaneElement(id: "1", type: "driving", level: "false");
            var laneLeftLink = laneLeft.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int leftLaneId in predecessorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLanePredecessor(leftLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int leftLaneId in successorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLaneSuccessor(leftLaneId.ToString());
                }
            }

            laneLeft.AddWidthElement();

            return curve;

        }


        public XODRRoad AddRightStraight(XODRJunction junction, float startX = 0, float startY = 0, double hdg = 0,
            bool sidewalk = false, SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorInfo.Ids.ElementAt(0).ToString(),
                connectingRoadId: id.ToString());
            connectionId++;

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "17",
                id: id.ToString(),
                junction: junctionId.ToString());
            id++;

            var link = road.AddLinkElement();
            if (predecessorInfo != null)
            {
                if (predecessorInfo.IsJunction == true)
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("road", id.ToString());
                    }
                }
            }

            if (successorInfo != null)
            {
                if (successorInfo.IsJunction == true)
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("road", id.ToString());

                    }
                }
            }

            var plainView = road.AddPlainViewElement();
            var geometry = plainView.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(),
                y: startY.ToString(),
                hdg: hdg.ToString(),
                length: "17");

            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Right);
            var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
            var laneRightLink = laneRight.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int rightLaneId in predecessorInfo.leftLaneIds)
                {
                    laneRightLink.AddLanePredecessor(rightLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int rightLaneId in successorInfo.leftLaneIds)
                {
                    laneRightLink.AddLaneSuccessor(rightLaneId.ToString());
                }
            }

            laneRight.AddWidthElement();

            if (sidewalk == true)
            {
                var laneRight2 = right.AddLaneElement(id: "-2", type: "driving", level: "false");
                laneRight2.AddLinkElement();
                laneRight2.AddWidthElement(a: "1.5");
            }

            return road;
        }

        public XODRRoad AddLeftStraight(XODRJunction junction, float startX = 0, float startY = 0, double hdg = 0,
            bool sidewalk = false, SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorInfo.Ids.ElementAt(0).ToString(),
                connectingRoadId: id.ToString());
            connectionId++;

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "17",
                id: id.ToString(),
                junction: junctionId.ToString());
            id++;

            var link = road.AddLinkElement();
            if (predecessorInfo != null)
            {
                if (predecessorInfo.IsJunction == true)
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("road", id.ToString());
                    }
                }
            }

            if (successorInfo != null)
            {
                if (successorInfo.IsJunction == true)
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("road", id.ToString());

                    }
                }
            }

            var plainView = road.AddPlainViewElement();
            var geometry = plainView.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(),
                y: startY.ToString(),
                hdg: hdg.ToString(),
                length: "17");

            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var left = laneSection.AddDirectionElement(Direction.Left);
            var laneLeft = left.AddLaneElement(id: "1", type: "driving", level: "false");
            var laneLeftLink = laneLeft.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int leftLaneId in predecessorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLanePredecessor(leftLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int leftLaneId in successorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLaneSuccessor(leftLaneId.ToString());
                }
            }

            laneLeft.AddWidthElement();

            if (sidewalk == true)
            {
                var laneLeft2 = left.AddLaneElement(id: "2", type: "driving", level: "false");
                laneLeft2.AddLinkElement();
                laneLeft2.AddWidthElement(a: "1.5");
            }


            return road;
        }


        public void Add3wayIntersection(float startX = 0, float startY = 0, float hdg = 0,
            SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            var junction = RootElement.AddJunctionElement(
                name: "Junction " + junctionId.ToString(),
                id: junctionId.ToString());
            junctionId++;

            int incomingRoadId1 = id;
            float startX1 = startX;
            float startY1 = startY;
            SequenceInfo predecessorRoad1 = predecessorInfo;
            predecessorRoad1.Ids = new List<int> {predecessorInfo.Ids.ElementAt(2)};
            var incomingRoad1 = this.AddStraightRoad(startX1, startY1, hdg, 0.5, false,
                predecessorInfo: predecessorRoad1);

            float startX2 = startX + 9f * (float)Math.Cos(hdg) + 9f * (float)Math.Sin(hdg);
            float startY2 = startY + 9f * (float)Math.Sin(hdg) - 9f * (float)Math.Cos(hdg);
            float hdg2 = hdg + 1.5707963267949f;
            var incomingRoadId2 = id;
            SequenceInfo predecessorRoad2 = predecessorInfo;
            predecessorRoad2.Ids = new List<int> { predecessorInfo.Ids.ElementAt(1)};
            var incomingRoad2 = this.AddStraightRoad(startX2, startY2, hdg2, 0.5, false,
                predecessorInfo: predecessorRoad2);

            float startX3 = startX + 18f * (float)Math.Cos(hdg);
            float startY3 = startY + 18f * (float)Math.Sin(hdg);
            var incomingRoadId3 = id;
            SequenceInfo predecessorRoad3 = predecessorInfo;
            predecessorRoad3.Ids = new List<int> { predecessorInfo.Ids.ElementAt(0)};
            var incomingRoad3 = this.AddStraightRoad(startX3, startY3, hdg + 3.14f, 0.5, false,
                predecessorInfo: predecessorRoad3);

            float startXCurve1 = startX + 9f * (float)Math.Cos(hdg) + 8.5f * (float)Math.Sin(hdg);
            float startYCurve1 = startY + 9f * (float)Math.Sin(hdg) - 8.5f * (float)Math.Cos(hdg);
            float hdgCurve1 = hdg + 1.5707963267949f;
            var curve1Right = this.AddRightCurveToIntersection(junction, startXCurve1, startYCurve1, hdgCurve1,
                new SequenceInfo
                {
                    IsJunction = false, Ids = new List<int>{incomingRoadId2}, leftLaneIds = new List<int> { },
                    rightLaneIds = new List<int> { -1 }
                },
                new SequenceInfo
                {
                    IsJunction = false, Ids = new List<int> { incomingRoadId3 }, leftLaneIds = null, rightLaneIds = new List<int> { -1 }
                });
            var curve1Left = this.AddLeftCurveToIntersection(junction, startXCurve1, startYCurve1, hdgCurve1,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId3 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = null
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = null
                });

            float startXCurve2 = startX + 0.5f * (float)Math.Cos(hdg);
            float startYCurve2 = startY + 0.5f * (float)Math.Sin(hdg);
            var curve2Right = this.AddRightCurveToIntersection(junction, startXCurve2, startYCurve2, hdg,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId1 }, leftLaneIds = null, rightLaneIds = new List<int> { -1 }
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 }, leftLaneIds = null, rightLaneIds = new List<int> { 1 }
                });
            var curve2Left = this.AddLeftCurveToIntersection(junction, startXCurve2, startYCurve2, hdg,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 }, leftLaneIds = new List<int> { -1 }, rightLaneIds = null
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId1 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = null
                });

            float startX4 = startX + 0.5f * (float)Math.Cos(hdg);
            float startY4 = startY + 0.5f * (float)Math.Sin(hdg);
            var connectionRoad1 = this.AddRightStraight(junction, startX4, startY4, hdg, false,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId1} , leftLaneIds = null, rightLaneIds = new List<int> { -1 }
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 }, leftLaneIds = null, rightLaneIds = new List<int> { -1 }
                });
            var connectionRoad2 = this.AddLeftStraight(junction, startX4, startY4, hdg, true,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = null
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId1 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = null
                });
            connectionId = 0;

        }


        public void Add4wayIntersection(float startX = 0, float startY = 0, float hdg = 0,
            SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            XODRJunction junction = RootElement.AddJunctionElement(
                name: "Junction " + junctionId.ToString(),
                id: junctionId.ToString());
            junctionId++;

            int incomingRoadId1 = id;
            float startX1 = startX;
            float startY1 = startY;
            SequenceInfo predecessorRoad1 = predecessorInfo;
            predecessorRoad1.Ids = new List<int> { predecessorInfo.Ids.ElementAt(3)};
            var incomingRoad1 = this.AddStraightRoad(startX1, startY1, hdg, 0.5, false,
                predecessorInfo: predecessorRoad1);

            float startX2 = startX + 9f * (float)Math.Cos(hdg) + 9f * (float)Math.Sin(hdg);
            float startY2 = startY + 9f * (float)Math.Sin(hdg) - 9f * (float)Math.Cos(hdg);
            float hdg2 = hdg + 1.5707963267949f;
            var incomingRoadId2 = id;
            SequenceInfo predecessorRoad2 = predecessorInfo;
            predecessorRoad2.Ids = new List<int> { predecessorInfo.Ids.ElementAt(2)};
            var incomingRoad2 = this.AddStraightRoad(startX2, startY2, hdg2, 0.5, false,
                predecessorInfo: predecessorRoad2);

            float startX3 = startX + 18f * (float)Math.Cos(hdg);
            float startY3 = startY + 18f * (float)Math.Sin(hdg);
            var incomingRoadId3 = id;
            SequenceInfo predecessorRoad3 = predecessorInfo;
            predecessorRoad3.Ids = new List<int> { predecessorInfo.Ids.ElementAt(1)};
            var incomingRoad3 = this.AddStraightRoad(startX3, startY3, hdg + 3.14f, 0.5, false,
                predecessorInfo: predecessorRoad3);

            float startX4 = startX + 9f * (float)Math.Cos(hdg) - 9f * (float)Math.Sin(hdg);
            float startY4 = startY + 9f * (float)Math.Sin(hdg) + 9f * (float)Math.Cos(hdg);
            float hdg4 = hdg - 1.5707963267949f;
            var incomingRoadId4 = id;
            SequenceInfo predecessorRoad4 = predecessorInfo;
            predecessorRoad4.Ids = new List<int> { predecessorInfo.Ids.ElementAt(4)};
            var incomingRoad4 = this.AddStraightRoad(startX4, startY4, hdg4, 0.5f, false,
                predecessorInfo: predecessorRoad4);

            float startXCurve1 = startX + 9f * (float)Math.Cos(hdg) + 8.5f * (float)Math.Sin(hdg);
            float startYCurve1 = startY + 9f * (float)Math.Sin(hdg) - 8.5f * (float)Math.Cos(hdg);
            float hdgCurve1 = hdg + 1.5707963267949f;
            var curve1Right = this.AddRightCurveToIntersection(junction, startXCurve1, startYCurve1, hdgCurve1,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 },
                    leftLaneIds = new List<int> { },
                    rightLaneIds = new List<int> { -1 }
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId3 },
                    leftLaneIds = null,
                    rightLaneIds = new List<int> { -1 }
                });
            var curve1Left = this.AddLeftCurveToIntersection(junction, startXCurve1, startYCurve1, hdgCurve1,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId3 },
                    leftLaneIds = new List<int> { 1 },
                    rightLaneIds = null
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 },
                    leftLaneIds = new List<int> { 1 },
                    rightLaneIds = null
                });

            float startXCurve2 = startX + 0.5f * (float)Math.Cos(hdg);
            float startYCurve2 = startY + 0.5f * (float)Math.Sin(hdg);
            var curve2Right = this.AddRightCurveToIntersection(junction, startXCurve2, startYCurve2, hdg,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId1 },
                    leftLaneIds = null,
                    rightLaneIds = new List<int> { -1 }
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 },
                    leftLaneIds = null,
                    rightLaneIds = new List<int> { 1 }
                });
            var curve2Left = this.AddLeftCurveToIntersection(junction, startXCurve2, startYCurve2, hdg,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 },
                    leftLaneIds = new List<int> { -1 },
                    rightLaneIds = null
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId1 },
                    leftLaneIds = new List<int> { 1 },
                    rightLaneIds = null
                });

            float startX5 = startX + 0.5f * (float)Math.Cos(hdg);
            float startY5 = startY + 0.5f * (float)Math.Sin(hdg);
            var connectionRoad1 = this.AddRightStraight(junction, startX5, startY5, hdg, false,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId1 }, leftLaneIds = null, rightLaneIds = new List<int> { -1 }
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 }, leftLaneIds = null, rightLaneIds = new List<int> { -1 }
                });
            var connectionRoad2 = this.AddLeftStraight(junction, startX5, startY5, hdg, true,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = null
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId1 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = null
                });

            float startX6 = startX + 9f * (float)Math.Cos(hdg) - 8.5f * (float)Math.Sin(hdg);
            float startY6 = startY + 9f * (float)Math.Sin(hdg) + 8.5f * (float)Math.Cos(hdg);
            float hdg6 = hdg - 1.5707963f;
            var connectionRoad3 = this.AddRightStraight(junction, startX6, startY6, hdg6, false,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId3 }, leftLaneIds = null, rightLaneIds = new List<int> { 1 }
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId4 }, leftLaneIds = null, rightLaneIds = new List<int> { -1 }
                });
            var connectionRoad4 = this.AddLeftStraight(junction, startX6, startY6, hdg6, false,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId4 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = null
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId3 }, leftLaneIds = new List<int> { -1 }, rightLaneIds = null
                });

            float startXCurve3 = startX + 9f * (float)Math.Cos(hdg) - 8.5f * (float)Math.Sin(hdg);
            float startYCurve3 = startY + 9f * (float)Math.Sin(hdg) + 8.5f * (float)Math.Cos(hdg);
            float hdgCurve3 = hdg - 1.5707963f;
            var curve3Left = this.AddRightCurveToIntersection(junction, startXCurve3, startYCurve3, hdgCurve3,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 }, leftLaneIds = null, rightLaneIds = new List<int> { 1 }
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId4 }, leftLaneIds = null, rightLaneIds = new List<int> { -1 }
                });
            var curve3Right = this.AddLeftCurveToIntersection(junction, startXCurve3, startYCurve3, hdgCurve3,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId4 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = null
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId2 }, leftLaneIds = new List<int> { -1 }, rightLaneIds = null
                });

            float startXCurve4 = startX + 17.5f * (float)Math.Cos(hdg);
            float startYCurve4 = startY + 17.5f * (float)Math.Sin(hdg);
            float hdgCurve4 = hdg + 3.1415926f;
            var curve4Left = this.AddRightCurveToIntersection(junction: junction, startXCurve4, startYCurve4, hdgCurve4,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId3 }, leftLaneIds = null, rightLaneIds = new List<int> { 1 }
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId4 }, leftLaneIds = null, rightLaneIds = new List<int> { 1 }
                });
            var curve4Right = this.AddLeftCurveToIntersection(junction: junction, startXCurve4, startYCurve4, hdgCurve4,
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId4 }, leftLaneIds = new List<int> { -1 }, rightLaneIds = null
                },
                new SequenceInfo
                {
                    IsJunction = false,
                    Ids = new List<int> { incomingRoadId3 }, leftLaneIds = new List<int> { -1 }, rightLaneIds = null
                });

            connectionId = 0;

        }

        private XODRRoad AddRoundaboutEntry(XODRJunction junction, float startX = 0, float startY = 0, float hdg = 0,
            SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            if (predecessorInfo != null)
            {
                var connection = junction.AddConnectionElement(
                    id: connectionId.ToString(),
                    incomingRoadId: predecessorInfo.Ids.ElementAt(0).ToString(),
                    connectingRoadId: id.ToString());
                connectionId++;


            }

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "7.7704795232679942",
                id: id.ToString(),
                junction: junctionId.ToString());

            id++;

            var link = road.AddLinkElement();
            if (predecessorInfo != null)
            {
                if (predecessorInfo.IsJunction == true)
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("road", id.ToString());
                    }
                }
            }

            if (successorInfo != null)
            {
                if (successorInfo.IsJunction == true)
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("road", id.ToString());

                    }
                }
            }


            var plainView = road.AddPlainViewElement();
            var geometry4 = plainView.AddGeometryElement(
                s: "0",
                x: (startX).ToString(),
                y: (startY).ToString(),
                hdg: (hdg).ToString(),
                length: "2.8722833994166841e-01");
            var geometry3 = plainView.AddGeometryElement(
                s: "2.8706087295301153e-01",
                x: (startX + 0.287228339941668 * (float)Math.Cos(hdg)).ToString(),
                y: (startY + 0.287228339941668 * (float)Math.Sin(hdg)).ToString(),
                hdg: (hdg).ToString(),
                length: "3.7278138864132848",
                curvature: "-7.0850666500999637e-02");
            var geometry2 = plainView.AddGeometryElement(
                s: "4.0150422263549537e+0",
                x: (startX + 3.9718521088374819 * (float)Math.Cos(hdg) + 4.8943641564075668e-01 * (float)Math.Sin(hdg))
                .ToString(),
                y: (startY + 3.9718521088374819 * (float)Math.Sin(hdg) - 4.8943641564075668e-01 * (float)Math.Cos(hdg))
                .ToString(),
                hdg: (hdg - 2.6411809844406342e-01).ToString(),
                length: "3.4683764239600299e+00",
                curvature: "-2.7150009545203924e-01");
            var geometry1 = plainView.AddGeometryElement(
                s: "7.4832511833263258e+00",
                x: (startX + 6.45089662102553073 * (float)Math.Cos(hdg) + 2.7301760090773617e+00 * (float)Math.Sin(hdg))
                .ToString(),
                y: (startY + 6.45089662102553073 * (float)Math.Sin(hdg) - 2.7301760090773617e+00 * (float)Math.Cos(hdg))
                .ToString(),
                hdg: (hdg - 1.2057826286128144e+00).ToString(),
                length: "2.8706087295301153e-01");

            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Left);
            var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
            var laneRightLink = laneRight.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int rightLaneId in predecessorInfo.leftLaneIds)
                {
                    laneRightLink.AddLanePredecessor(rightLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int rightLaneId in successorInfo.leftLaneIds)
                {
                    laneRightLink.AddLaneSuccessor(rightLaneId.ToString());
                }
            }

            laneRight.AddWidthElement();
            var laneRight2 = right.AddLaneElement(id: "-2", type: "driving", level: "false");
            laneRight2.AddLinkElement();
            laneRight2.AddWidthElement(a: "1.5");

            return road;
        }

        private XODRRoad AddRoundaboutExit(XODRJunction junction, float startX = 0, float startY = 0, float hdg = 0,
            SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            if (predecessorInfo != null)
            {
                var connection = junction.AddConnectionElement(
                    id: connectionId.ToString(),
                    incomingRoadId: predecessorInfo.Ids.ElementAt(0).ToString(),
                    connectingRoadId: id.ToString());
                connectionId++;


            }

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "7.7704795232679942",
                id: id.ToString(),
                junction: junctionId.ToString());

            id++;


            var link = road.AddLinkElement();
            if (predecessorInfo != null)
            {
                if (predecessorInfo.IsJunction == true)
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("road", id.ToString());
                    }
                }
            }

            if (successorInfo != null)
            {
                if (successorInfo.IsJunction == true)
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("road", id.ToString());

                    }
                }
            }

            var plainView = road.AddPlainViewElement();
            var geometry4 = plainView.AddGeometryElement(
                s: "0",
                x: (startX).ToString(),
                y: (startY).ToString(),
                hdg: (hdg).ToString(),
                length: "2.8722833994166841e-01");
            var geometry3 = plainView.AddGeometryElement(
                s: "2.8706087295301153e-01",
                x: (startX + 0.287228339941668 * (float)Math.Cos(hdg)).ToString(),
                y: (startY + 0.287228339941668 * (float)Math.Sin(hdg)).ToString(),
                hdg: (hdg).ToString(),
                length: "3.7278138864132848",
                curvature: "7.0850666500999637e-02");
            var geometry2 = plainView.AddGeometryElement(
                s: "4.0150422263549537e+0",
                x: (startX + 3.9718521088374819 * (float)Math.Cos(hdg) - 4.8943641564075668e-01 * (float)Math.Sin(hdg))
                .ToString(),
                y: (startY + 3.9718521088374819 * (float)Math.Sin(hdg) + 4.8943641564075668e-01 * (float)Math.Cos(hdg))
                .ToString(),
                hdg: (hdg + 2.6411809844406342e-01).ToString(),
                length: "3.4683764239600299e+00",
                curvature: "2.7150009545203924e-01");
            var geometry1 = plainView.AddGeometryElement(
                s: "7.4832511833263258e+00",
                x: (startX + 6.45089662102553073 * (float)Math.Cos(hdg) - 2.7301760090773617e+00 * (float)Math.Sin(hdg))
                .ToString(),
                y: (startY + 6.45089662102553073 * (float)Math.Sin(hdg) + 2.7301760090773617e+00 * (float)Math.Cos(hdg))
                .ToString(),
                hdg: (hdg + 1.2057826286128144e+00).ToString(),
                length: "2.8706087295301153e-01");


            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var left = laneSection.AddDirectionElement(Direction.Left);
            var laneLeft = left.AddLaneElement(id: "1", type: "driving", level: "false");
            var laneLeftLink = laneLeft.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int leftLaneId in predecessorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLanePredecessor(leftLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int leftLaneId in successorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLaneSuccessor(leftLaneId.ToString());
                }
            }

            laneLeft.AddWidthElement();
            laneLeft.AddWidthElement();
            var laneLeft2 = left.AddLaneElement(id: "2", type: "driving", level: "false");
            laneLeft2.AddLinkElement();
            laneLeft2.AddWidthElement(a: "1.5");

            return road;
        }

        //The custom road has no left lane or left sidewalk
        public XODRRoad AddRightLaneCurve(XODRJunction junction, float startX = 0, float startY = 0, float hdg = 0,
            float length = 0, string laneWidth = "3.5", string curvature = null, bool sidewalk = false,
            SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            if (predecessorInfo != null)
            {
                foreach (var predecessorId in predecessorInfo.Ids)
                {
                    var connection = junction.AddConnectionElement(
                        id: connectionId.ToString(),
                        incomingRoadId: predecessorId.ToString(),
                        connectingRoadId: id.ToString());
                    connectionId++;

                }
            }


            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: length.ToString(),
                id: id.ToString(),
                junction: junctionId.ToString());

            id++;


            var link = road.AddLinkElement();
            if (predecessorInfo != null)
            {
                if (predecessorInfo.IsJunction == true)
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("road", id.ToString());
                    }
                }
            }

            if (successorInfo != null)
            {
                if (successorInfo.IsJunction == true)
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("road", id.ToString());

                    }
                }
            }


            var plainView = road.AddPlainViewElement();
            var geometry1 = plainView.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(),
                y: startY.ToString(),
                hdg: hdg.ToString(),
                length: length.ToString(),
                curvature: curvature);

            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Right);
            var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
            var laneRightLink = laneRight.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int rightLaneId in predecessorInfo.leftLaneIds)
                {
                    laneRightLink.AddLanePredecessor(rightLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int rightLaneId in successorInfo.leftLaneIds)
                {
                    laneRightLink.AddLaneSuccessor(rightLaneId.ToString());
                }
            }
            laneRight.AddWidthElement(a: laneWidth);

                if (sidewalk == true)
                {
                    var laneLeft1 = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
                    laneLeft1.AddLinkElement();
                    laneLeft1.AddWidthElement(a: "1.5");
                }

            return road;
        }
    

        public void Add3WayRoundAbout(float startX = 0, float startY = 0, float hdg = 0.0f, SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            XODRJunction junction = RootElement.AddJunctionElement(
               name: "Junction " + junctionId.ToString(),
               id: junctionId.ToString());

            SequenceInfo predecessorRoad1 = predecessorInfo;
            predecessorRoad1.Ids = new List<int> { predecessorInfo.Ids.ElementAt(2), successorInfo.Ids.ElementAt(2) };
            SequenceInfo predecessorRoad2 = predecessorInfo;
            predecessorRoad2.Ids = new List<int> { predecessorInfo.Ids.ElementAt(0), successorInfo.Ids.ElementAt(0) };
            SequenceInfo predecessorRoad3 = predecessorInfo;
            predecessorRoad3.Ids = new List<int> { predecessorInfo.Ids.ElementAt(1), successorInfo.Ids.ElementAt(1) };
            var road0 = this.AddStraightRoad(startX: startX, startY: startY, hdg: hdg, length: 5.5999755859375000e+00, predecessorInfo: predecessorRoad1, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> {id + 12, id + 16 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = new List<int> { -1 }});
            var road1 = this.AddStraightRoad(startX: startX + 20 * (float)Math.Cos(hdg) + 20f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 20 * (float)Math.Cos(hdg), hdg: hdg + 1.5707963267948966e+00f, length: 5.5999755859375000e+00f, predecessorInfo: predecessorRoad2, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 12, id+16 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = new List<int> { -1 }});
            var road2 = this.AddStraightRoad(startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg, length: 5.5999755859375000e+00f, predecessorInfo: predecessorRoad3, successorInfo:  new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 12, id + 16 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = new List<int> { -1 } });

            var road3 = this.AddRightLaneCurve(junction: junction, startX: startX + 12.1533743648088794f * (float)Math.Cos(hdg) + 2.9984095338145988e+00f * (float)Math.Sin(hdg), startY: startY + 12.1533743648088794f * (float)Math.Sin(hdg) - 2.9984095338145988e+00f * (float)Math.Cos(hdg), hdg: hdg - 1.2057917902788586e+00f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int>{id + 4, id + 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> {id + 5, id + 13}, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1, 1 } });
            var road4 = this.AddRightLaneCurve(junction: junction, startX: startX + 22.9984095338145988f * (float)Math.Cos(hdg) + 7.8466256351911206f * (float)Math.Sin(hdg), startY: startY + 22.9984095338145988f * (float)Math.Sin(hdg) - 7.8466256351911206f * (float)Math.Cos(hdg), hdg: hdg - 1.2057917902788586e+00f + 1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 4, id + 8}, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 5, id + 13 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1, 1 } });
            var road5 = this.AddRightLaneCurve(junction: junction, startX: startX + 27.8466256351911206f * (float)Math.Cos(hdg) - 2.9984095338145988e+00f * (float)Math.Sin(hdg), startY: startY + 27.8466256351911206f * (float)Math.Sin(hdg) + 2.9984095338145988e+00f * (float)Math.Cos(hdg), hdg: hdg - 1.2057917902788586e+00f + 2 * 1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 4, id + 8}, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo:  new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 5 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road6 = this.AddRightLaneCurve(junction: junction, startX: startX + 17.0015905f * (float)Math.Cos(hdg) - 7.8466256351911206f * (float)Math.Sin(hdg), startY: startY + 17.0015905f * (float)Math.Sin(hdg) + 7.8466256351911206f * (float)Math.Cos(hdg), hdg: hdg - 1.2057917902788586e+00f + 3 * 1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 4}, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 1, id + 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1, 1 } });

            var road7 = this.AddRightLaneCurve(junction: junction, startX: startX + 12.1534925868695165f * (float)Math.Cos(hdg) - 2.9987188942942149f * (float)Math.Sin(hdg), startY: startY + 12.1534925868695165f * (float)Math.Sin(hdg) + 2.9987188942942149f * (float)Math.Cos(hdg), hdg: hdg - 3.6504396273878453e-01f + 3 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01", predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 1}, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 4 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road8 = this.AddRightLaneCurve(junction: junction, startX: startX + 17.0012811057057851f * (float)Math.Cos(hdg) + 7.8465074131304835f * (float)Math.Sin(hdg), startY: startY + 17.0012811057057851f * (float)Math.Sin(hdg) - 7.8465074131304835f * (float)Math.Cos(hdg), hdg: hdg - 3.6504396273878453e-01f, length: 6.1320759349930771e+00f, curvature: "1.1904762445393628e-01", predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 5 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 4 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road9 = this.AddRightLaneCurve(junction: junction, startX: startX + 27.8466256351911206f * (float)Math.Cos(hdg) + 2.9987188942942149f * (float)Math.Sin(hdg), startY: startY + 27.8466256351911206f * (float)Math.Sin(hdg) - 2.9987188942942149f * (float)Math.Cos(hdg), hdg: hdg - 3.6504396273878453e-01f + 1 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01", predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 5 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 4 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road10 = this.AddRightLaneCurve(junction: junction, startX: startX + 22.9987188942942149f * (float)Math.Cos(hdg) - 7.8465074131304835f * (float)Math.Sin(hdg), startY: startY + 22.9987188942942149f * (float)Math.Sin(hdg) + 7.8465074131304835f * (float)Math.Cos(hdg), hdg: hdg - 3.6504396273878453e-01f + 2 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01", predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 5 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 4 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });

            var road11 = this.AddRoundaboutEntry(junction: junction, startX: startX + 5.599975585937500f * (float)Math.Cos(hdg), startY: startY + 5.5999755859375000f * (float)Math.Sin(hdg), hdg: hdg, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 11 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road12 = this.AddRoundaboutEntry(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) + 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 1.57f, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 11 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road13 = this.AddRoundaboutEntry(junction: junction, startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg + 3.1415f, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 11 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });

            var road14 = this.AddRoundaboutExit(junction: junction, startX: startX + 5.599975585937500f * (float)Math.Cos(hdg), startY: startY + 5.5999755859375000f * (float)Math.Sin(hdg), hdg: hdg, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 14 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { 1 } });
            var road15 = this.AddRoundaboutExit(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) + 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 1.57f, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 12 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 14 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { 1 } });
            var road16 = this.AddRoundaboutExit(junction: junction, startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg + 3.1415f, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 12 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 14 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { 1 } });

            junctionId++;
            connectionId = 0;

        }

        public void Add4WayRoundAbout(float startX = 0, float startY = 0, float hdg = 0.0f, SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            XODRJunction junction = RootElement.AddJunctionElement(
               name: "Junction " + junctionId.ToString(),
               id: junctionId.ToString());

            SequenceInfo predecessorRoad1 = predecessorInfo;
            predecessorRoad1.Ids = new List<int> { predecessorInfo.Ids.ElementAt(2), successorInfo.Ids.ElementAt(3) };
            SequenceInfo predecessorRoad2 = predecessorInfo;
            predecessorRoad2.Ids = new List<int> { predecessorInfo.Ids.ElementAt(0), successorInfo.Ids.ElementAt(0) };
            SequenceInfo predecessorRoad3 = predecessorInfo;
            predecessorRoad3.Ids = new List<int> { predecessorInfo.Ids.ElementAt(1), successorInfo.Ids.ElementAt(1) };
            SequenceInfo predecessorRoad4 = predecessorInfo;
            predecessorRoad4.Ids = new List<int> { predecessorInfo.Ids.ElementAt(1), successorInfo.Ids.ElementAt(2) };
            var road0 = this.AddStraightRoad(startX: startX, startY: startY, hdg: hdg, length: 5.5999755859375000e+00, predecessorInfo: predecessorRoad1, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 12, id + 16 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = new List<int> { -1 } });
            var road1 = this.AddStraightRoad(startX: startX + 20 * (float)Math.Cos(hdg) + 20f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 20 * (float)Math.Cos(hdg), hdg: hdg + 1.5707963267948966e+00f, length: 5.5999755859375000e+00f, predecessorInfo: predecessorRoad2, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 12, id + 16}, leftLaneIds = new List<int> { 1 }, rightLaneIds = new List<int> { -1 } });
            var road2 = this.AddStraightRoad(startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg, length: 5.5999755859375000e+00f, predecessorInfo: predecessorRoad3, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 12, id + 16 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = new List<int> { -1 } });
            var road3 = this.AddStraightRoad(startX: startX + 20f * (float)Math.Cos(hdg) - 20f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) + 20f * (float)Math.Cos(hdg), hdg: hdg - 1.5707963267948966e+00f, length: 5.5999755859375000e+00f, predecessorInfo: predecessorRoad4, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 12, id + 16 }, leftLaneIds = new List<int> { 1 }, rightLaneIds = new List<int> { -1 } });

            var road4 = this.AddRightLaneCurve(junction: junction, startX: startX + 12.1533743648088794f * (float)Math.Cos(hdg) + 2.9984095338145988e+00f * (float)Math.Sin(hdg), startY: startY + 12.1533743648088794f * (float)Math.Sin(hdg) - 2.9984095338145988e+00f * (float)Math.Cos(hdg), hdg: hdg - 1.2057917902788586e+00f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 4, id + 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 5, id + 13 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1, 1 } });
            var road5 = this.AddRightLaneCurve(junction: junction, startX: startX + 22.9984095338145988f * (float)Math.Cos(hdg) + 7.8466256351911206f * (float)Math.Sin(hdg), startY: startY + 22.9984095338145988f * (float)Math.Sin(hdg) - 7.8466256351911206f * (float)Math.Cos(hdg), hdg: hdg - 1.2057917902788586e+00f + 1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 4, id + 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 5, id + 13 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1, 1 } });
            var road6 = this.AddRightLaneCurve(junction: junction, startX: startX + 27.8466256351911206f * (float)Math.Cos(hdg) - 2.9984095338145988e+00f * (float)Math.Sin(hdg), startY: startY + 27.8466256351911206f * (float)Math.Sin(hdg) + 2.9984095338145988e+00f * (float)Math.Cos(hdg), hdg: hdg - 1.2057917902788586e+00f + 2 * 1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 4, id + 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 5, id + 13 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1, 1 } });
            var road7 = this.AddRightLaneCurve(junction: junction, startX: startX + 17.0015905f * (float)Math.Cos(hdg) - 7.8466256351911206f * (float)Math.Sin(hdg), startY: startY + 17.0015905f * (float)Math.Sin(hdg) + 7.8466256351911206f * (float)Math.Cos(hdg), hdg: hdg - 1.2057917902788586e+00f + 3 * 1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 4, id + 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id + 1, id + 9}, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1, 1 } });

            var road8 = this.AddRightLaneCurve(junction: junction, startX: startX + 12.1534925868695165f * (float)Math.Cos(hdg) - 2.9987188942942149f * (float)Math.Sin(hdg), startY: startY + 12.1534925868695165f * (float)Math.Sin(hdg) + 2.9987188942942149f * (float)Math.Cos(hdg), hdg: hdg - 3.6504396273878453e-01f + 3 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01", predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 1 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 4 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road9 = this.AddRightLaneCurve(junction: junction, startX: startX + 17.0012811057057851f * (float)Math.Cos(hdg) + 7.8465074131304835f * (float)Math.Sin(hdg), startY: startY + 17.0012811057057851f * (float)Math.Sin(hdg) - 7.8465074131304835f * (float)Math.Cos(hdg), hdg: hdg - 3.6504396273878453e-01f, length: 6.1320759349930771e+00f, curvature: "1.1904762445393628e-01", predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 5 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 4 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road10 = this.AddRightLaneCurve(junction: junction, startX: startX + 27.8466256351911206f * (float)Math.Cos(hdg) + 2.9987188942942149f * (float)Math.Sin(hdg), startY: startY + 27.8466256351911206f * (float)Math.Sin(hdg) - 2.9987188942942149f * (float)Math.Cos(hdg), hdg: hdg - 3.6504396273878453e-01f + 1 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01", predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 5 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 4 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road11 = this.AddRightLaneCurve(junction: junction, startX: startX + 22.9987188942942149f * (float)Math.Cos(hdg) - 7.8465074131304835f * (float)Math.Sin(hdg), startY: startY + 22.9987188942942149f * (float)Math.Sin(hdg) + 7.8465074131304835f * (float)Math.Cos(hdg), hdg: hdg - 3.6504396273878453e-01f + 2 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01", predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 5 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 4 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });

            var road12 = this.AddRoundaboutEntry(junction: junction, startX: startX + 5.599975585937500f * (float)Math.Cos(hdg), startY: startY + 5.5999755859375000f * (float)Math.Sin(hdg), hdg: hdg, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 12 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road13 = this.AddRoundaboutEntry(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) + 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 1.57f, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 12 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road14 = this.AddRoundaboutEntry(junction: junction, startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg + 3.1415f, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 12 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });
            var road15 = this.AddRoundaboutEntry(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) - 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) + 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 3 * 1.57f, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 12 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 8 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } });

            var road16 = this.AddRoundaboutExit(junction: junction, startX: startX + 5.599975585937500f * (float)Math.Cos(hdg), startY: startY + 5.5999755859375000f * (float)Math.Sin(hdg), hdg: hdg, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 9 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 16 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { 1 } });
            var road17 = this.AddRoundaboutExit(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) + 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 1.57f, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 13}, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo:  new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 16 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { 1 } });
            var road18 = this.AddRoundaboutExit(junction: junction, startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg + 3.1415f, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 13 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 16 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { 1 } });
            var road19 = this.AddRoundaboutExit(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) - 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) + 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 3 * 1.57f, predecessorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 13 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> { -1 } }, successorInfo: new SequenceInfo { IsJunction = false, Ids = new List<int> { id - 16 }, leftLaneIds = new List<int> { }, rightLaneIds = new List<int> {1 } });

            junctionId++;
            connectionId = 0;
        }

        public void AddParking(bool topParking, bool bottomParking, float startX = 0, float startY = 0, double hdg = 0, double length = 17, string laneWidth = "3.5", SequenceInfo predecessorInfo = null, SequenceInfo successorInfo = null)
        {
            //Add junction
            XODRJunction junction = RootElement.AddJunctionElement(
               name: "Junction " + junctionId.ToString(),
               id: junctionId.ToString());
            
            //Add straight road
            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: length.ToString(),
                id: id.ToString(),
                junction: junctionId.ToString());
            var mainRoadId = id;
            id++;

            var link = road.AddLinkElement();
            if (predecessorInfo != null)
            {
                if (predecessorInfo.IsJunction == true)
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in predecessorInfo.Ids)
                    {
                        var predecessor = link.AddPredecessor("road", id.ToString());
                    }
                }
            }

            if (successorInfo != null)
            {
                if (successorInfo.IsJunction == true)
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("junction", id.ToString());
                    }
                }
                else
                {
                    foreach (var id in successorInfo.Ids)
                    {
                        var successor = link.AddSuccessor("road", id.ToString());

                    }
                }
            }

            var plainView = road.AddPlainViewElement();
            var geometry = plainView.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(CultureInfo.InvariantCulture),
                y: startY.ToString(),
                hdg: hdg.ToString(),
                length: length.ToString(CultureInfo.InvariantCulture));

            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var left = laneSection.AddDirectionElement(Direction.Left);
            var laneLeft = left.AddLaneElement(id: "1", type: "driving", level: "false");
            var laneLeftLink = laneLeft.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int leftLaneId in predecessorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLanePredecessor(leftLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int leftLaneId in successorInfo.leftLaneIds)
                {
                    laneLeftLink.AddLaneSuccessor(leftLaneId.ToString());
                }
            }

            laneLeft.AddWidthElement(a: laneWidth);
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Right);
            var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
            var laneRightLink = laneRight.AddLinkElement();
            if (predecessorInfo != null)
            {
                foreach (int rightLaneId in predecessorInfo.rightLaneIds)
                {
                    laneRightLink.AddLanePredecessor(rightLaneId.ToString());
                }
            }

            if (successorInfo != null)
            {
                foreach (int rightLaneId in successorInfo.rightLaneIds)
                {
                    laneRightLink.AddLaneSuccessor(rightLaneId.ToString());
                }
            }
            laneRight.AddWidthElement(a: laneWidth);

            var objects = road.AddObjectsElement();

            //Add parking spots on the top (parallel to raod)
            if (topParking == true)
            {
                var parkingSpot1 = objects.AddObjectElement(zOffset: "0", s: "5", t: "5", hdg: (1.57).ToString(),
                    id: "0", length: "3.000375", name: "parkingspot", orientation: "none", pitch: "0.0",
                    roll: "0.0", type: "parkingSpace", width: "6.930416660");
                var parkingSpot2 = objects.AddObjectElement(zOffset: "0", s: "12", t: "5", hdg: (1.57).ToString(),
                    id: "1", length: "3.000375", name: "parkingspot", orientation: "none", pitch: "0.0",
                    roll: "0.0", type: "parkingSpace", width: "6.930416660");


                var roadParkingTop = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "14",
                id: id.ToString(),
                junction: junctionId.ToString());
                var roadParkingTopId = id;
                id++;
                var linkParkingTop = roadParkingTop.AddLinkElement();
                var plainViewParkingTop = roadParkingTop.AddPlainViewElement();
                var geometryParkingTop = plainViewParkingTop.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 1.5f * (float)Math.Cos(hdg) - 3.5f * (float)Math.Sin(hdg)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 1.5 * (float)Math.Sin(hdg) + 3.5f * (float)Math.Cos(hdg)).ToString(CultureInfo.InvariantCulture),
                    hdg: hdg.ToString(),
                    length: "14");
                var lanesParkingTop = roadParkingTop.AddLanesElement();
                var laneSectionParkingTop = lanesParkingTop.AddLaneSectionElement(s: "0");
                var leftParkingTop = laneSectionParkingTop.AddDirectionElement(Direction.Left);
                var laneLeftParkingTop = leftParkingTop.AddLaneElement(id: "1", type: "driving", level: "false");
                var laneLeftLinkParkingTop = laneLeftParkingTop.AddLinkElement();
                laneLeftParkingTop.AddWidthElement(a: "3");
                var centerParkingTop = laneSectionParkingTop.AddDirectionElement(Direction.Center);
                var laneCenterParkingTop = centerParkingTop.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenterParkingTop.AddLinkElement();
                laneCenterParkingTop.AddWidthElement(a: "0");

                //Add connection
                var connection = junction.AddConnectionElement(
                    id: "0",
                    incomingRoadId: roadParkingTopId.ToString(),
                    connectingRoadId: mainRoadId.ToString());
                connectionId++;

                //Add sidewalks
                var topSidewalk = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length:  "14",
                id: id.ToString(),
                junction: "-1");
                id++;
                
                var plainView1 = topSidewalk.AddPlainViewElement();
                var geometry1 = plainView1.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 1.5f * (float)Math.Cos(hdg) - 6.5f * (float)Math.Sin(hdg)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 1.5f * (float)Math.Sin(hdg) + 6.5f * (float)Math.Cos(hdg)).ToString(CultureInfo.InvariantCulture),
                    hdg: hdg.ToString(),
                    length: "14");

                var lanes1 = topSidewalk.AddLanesElement();
                var laneSection1 = lanes1.AddLaneSectionElement(s: "0");
                var left1 = laneSection1.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk1 = left1.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk1.AddLinkElement();
                laneLeftSidewalk1.AddWidthElement(a: "1.5");
                var center1 = laneSection1.AddDirectionElement(Direction.Center);
                var laneCenter1 = center1.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter1.AddLinkElement();
                laneCenter1.AddWidthElement(a: "0");


                var topLeftSidewalk = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "1.5",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView2 = topLeftSidewalk.AddPlainViewElement();
                var geometry2 = plainView2.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 5f * (float)Math.Cos(hdg + 1.5707963267949) + 1.5f * (float)Math.Sin(hdg + 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 5f * (float)Math.Sin(hdg + 1.5707963267949) - 1.5f * (float)Math.Cos(hdg + 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg + 1.5707963267949).ToString(),
                    length: "1.5");

                var lanes2 = topLeftSidewalk.AddLanesElement();
                var laneSection2 = lanes2.AddLaneSectionElement(s: "0");
                var left2 = laneSection2.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk2 = left2.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk2.AddLinkElement();
                laneLeftSidewalk2.AddWidthElement(a: "1.5");
                var center2 = laneSection2.AddDirectionElement(Direction.Center);
                var laneCenter2 = center2.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter2.AddLinkElement();
                laneCenter2.AddWidthElement(a: "0");


                var topRightSidewalk = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "1.5",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView3 = topRightSidewalk.AddPlainViewElement();
                var geometry3 = plainView3.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 5f * (float)Math.Cos(hdg + 1.5707963267949) + 17f * (float)Math.Sin(hdg + 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 5f * (float)Math.Sin(hdg + 1.5707963267949) - 17f * (float)Math.Cos(hdg + 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg + 1.5707963267949).ToString(),
                    length: "1.5");

                var lanes3 = topRightSidewalk.AddLanesElement();
                var laneSection3 = lanes3.AddLaneSectionElement(s: "0");
                var left3 = laneSection3.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk3 = left3.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk3.AddLinkElement();
                laneLeftSidewalk3.AddWidthElement(a: "1.5");
                var center3 = laneSection3.AddDirectionElement(Direction.Center);
                var laneCenter3 = center3.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter3.AddLinkElement();
                laneCenter3.AddWidthElement(a: "0");


                var topLeftSidewalkCurve = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "2.3561944901923",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView4 = topLeftSidewalkCurve.AddPlainViewElement();
                var geometry4 = plainView4.AddGeometryElement(
                    s: "0.0",
                    x: (startX - 1.5f * (float)Math.Cos(hdg - 3.1415926535898) + 8f * (float)Math.Sin(hdg - 3.1415926535898)).ToString(CultureInfo.InvariantCulture),
                    y: (startY - 1.5f * (float)Math.Sin(hdg - 3.1415926535898) - 8f * (float)Math.Cos(hdg - 3.1415926535898)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg - 3.1415926535898).ToString(),
                    length: "2.3561944901923",
                    curvature: "0.66666666667");

                var lanes4 = topLeftSidewalkCurve.AddLanesElement();
                var laneSection4 = lanes4.AddLaneSectionElement(s: "0");
                var left4 = laneSection4.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk4 = left4.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk4.AddLinkElement();
                laneLeftSidewalk4.AddWidthElement(a: "1.5");
                var center4 = laneSection4.AddDirectionElement(Direction.Center);
                var laneCenter4 = center4.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter4.AddLinkElement();
                laneCenter4.AddWidthElement(a: "0");


                var topRightSidewalkCurve = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "2.3561944901923",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView5 = topRightSidewalkCurve.AddPlainViewElement();
                var geometry5 = plainView5.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 6.5f * (float)Math.Cos(hdg + 1.5707963267949) + 17f * (float)Math.Sin(hdg + 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 6.5f * (float)Math.Sin(hdg + 1.5707963267949) - 17f * (float)Math.Cos(hdg + 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg + 1.570796).ToString(),
                    length: "2.3561944901923",
                    curvature: "0.66666666667");

                var lanes5 = topRightSidewalkCurve.AddLanesElement();
                var laneSection5 = lanes5.AddLaneSectionElement(s: "0");
                var left5 = laneSection5.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk5 = left5.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk5.AddLinkElement();
                laneLeftSidewalk5.AddWidthElement(a: "1.5");
                var center5 = laneSection5.AddDirectionElement(Direction.Center);
                var laneCenter5 = center5.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter5.AddLinkElement();
                laneCenter5.AddWidthElement(a: "0");


                var topCurveContRight = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "2.3561944901923",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView6 = topCurveContRight.AddPlainViewElement();
                var geometry6 = plainView6.AddGeometryElement(
                    s: "0.0",
                    x: (startX - 5f * (float)Math.Cos(hdg - 1.5707963267949) - 15.5f * (float)Math.Sin(hdg - 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    y: (startY - 5f * (float)Math.Sin(hdg - 1.5707963267949) + 15.5f * (float)Math.Cos(hdg - 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg - 1.570796).ToString(),
                    length: "2.3561944901923",
                    curvature: "0.66666666667");

                var lanes6 = topCurveContRight.AddLanesElement();
                var laneSection6 = lanes6.AddLaneSectionElement(s: "0");
                var left6 = laneSection6.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk6 = left6.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk6.AddLinkElement();
                laneLeftSidewalk6.AddWidthElement(a: "1.5");
                var center6 = laneSection6.AddDirectionElement(Direction.Center);
                var laneCenter6 = center6.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter6.AddLinkElement();
                laneCenter6.AddWidthElement(a: "0");


                var topCurveContLeft = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "2.3561944901923",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView7 = topCurveContLeft.AddPlainViewElement();
                var geometry7 = plainView7.AddGeometryElement(
                    s: "0.0",
                    x: (startX - 3.5f * (float)Math.Sin(hdg)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 3.5f * (float)Math.Cos(hdg)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg).ToString(),
                    length: "2.3561944901923",
                    curvature: "0.66666666667");

                var lanes7 = topCurveContLeft.AddLanesElement();
                var laneSection7 = lanes7.AddLaneSectionElement(s: "0");
                var left7 = laneSection7.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk7 = left7.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk7.AddLinkElement();
                laneLeftSidewalk7.AddWidthElement(a: "1.5");
                var center7 = laneSection7.AddDirectionElement(Direction.Center);
                var laneCenter7 = center7.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter7.AddLinkElement();
                laneCenter7.AddWidthElement(a: "0");

            }

            //Add parking spots on the bottom (90 degrees to road)
            if (bottomParking == true)
            {
                var parkingSpot3 = objects.AddObjectElement(zOffset: "0", s: "6", t: "-7", hdg: (1.57).ToString(),
                    id: "2", length: "7.00087", name: "parkingspot", orientation: "none", pitch: "0.0",
                    roll: "0.0", type: "parkingSpace", width: "4.902729166");
                var parkingSpot4 = objects.AddObjectElement(zOffset: "0", s: "11", t: "-7", hdg: (1.57 ).ToString(),
                    id : "3", length: "7.00087", name: "parkingspot", orientation: "none", pitch: "0.0", 
                    roll: "0.0", type: "parkingSpace", width: "4.9027291660");

                var roadParkingTop = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "7.000875",
                id: id.ToString(),
                junction: junctionId.ToString());
                var roadParkingBottomId = id;
                id++;
                var linkParkingTop = roadParkingTop.AddLinkElement();
                var plainViewParkingTop = roadParkingTop.AddPlainViewElement();
                var geometryParkingTop = plainViewParkingTop.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 8.5f * (float)Math.Cos(hdg) + 3.5f * (float)Math.Sin(hdg)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 8.5 * (float)Math.Sin(hdg) - 3.5f * (float)Math.Cos(hdg)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg - 1.57).ToString(),
                    length: "7.000875");
                var lanesParkingBottom = roadParkingTop.AddLanesElement();
                var laneSectionParkingBottom = lanesParkingBottom.AddLaneSectionElement(s: "0");
                var leftParkingBottom = laneSectionParkingBottom.AddDirectionElement(Direction.Left);
                var laneLeftParkingBottom = leftParkingBottom.AddLaneElement(id: "1", type: "driving", level: "false");
                var laneLeftLinkParkingBottom = laneLeftParkingBottom.AddLinkElement();
                laneLeftParkingBottom.AddWidthElement(a: "5");
                var centerParkingBottom = laneSectionParkingBottom.AddDirectionElement(Direction.Center);
                var laneCenterParkingBottom = centerParkingBottom.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenterParkingBottom.AddLinkElement();
                laneCenterParkingBottom.AddWidthElement(a: "0");
                var rightParkingBottom = laneSectionParkingBottom.AddDirectionElement(Direction.Right);
                var laneRightParkingBottom = rightParkingBottom.AddLaneElement(id: "-1", type: "driving", level: "false");
                var laneRightLinkParkingBottom = laneRightParkingBottom.AddLinkElement();
                laneRightParkingBottom.AddWidthElement(a: "5");

                //Add connection
                var connection = junction.AddConnectionElement(
                    id: "1",
                    incomingRoadId: roadParkingBottomId.ToString(),
                    connectingRoadId: mainRoadId.ToString());
                connectionId++;

                //Add sidewalks
                var bottomSidewalk = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "10",
                id: id.ToString(),
                junction: "-1");
                id++;

                var plainView8 = bottomSidewalk.AddPlainViewElement();
                var geometry8 = plainView8.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 3.5f * (float)Math.Cos(hdg) + 12f * (float)Math.Sin(hdg)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 3.5f * (float)Math.Sin(hdg) - 12f * (float)Math.Cos(hdg)).ToString(CultureInfo.InvariantCulture),
                    hdg: hdg.ToString(),
                    length: "10");

                var lanes8 = bottomSidewalk.AddLanesElement();
                var laneSection8 = lanes8.AddLaneSectionElement(s: "0");
                var left8 = laneSection8.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk8 = left8.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk8.AddLinkElement();
                laneLeftSidewalk8.AddWidthElement(a: "1.5");
                var center8 = laneSection8.AddDirectionElement(Direction.Center);
                var laneCenter8 = center8.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter8.AddLinkElement();
                laneCenter8.AddWidthElement(a: "0");


                var bottomLeftSidewalk = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "5.5",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView9 = bottomLeftSidewalk.AddPlainViewElement();
                var geometry9 = plainView9.AddGeometryElement(
                    s: "0.0",
                    x: (startX - 10.5f * (float)Math.Cos(hdg + 1.5707963267949) + 3.5f * (float)Math.Sin(hdg + 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    y: (startY - 10.5f * (float)Math.Sin(hdg + 1.5707963267949) - 3.5f * (float)Math.Cos(hdg + 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg + 1.570796).ToString(),
                    length: "5.5");

                var lanes9 = bottomLeftSidewalk.AddLanesElement();
                var laneSection9 = lanes9.AddLaneSectionElement(s: "0");
                var left9 = laneSection9.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk9 = left9.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk9.AddLinkElement();
                laneLeftSidewalk9.AddWidthElement(a: "1.5");
                var center9 = laneSection9.AddDirectionElement(Direction.Center);
                var laneCenter9 = center9.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter9.AddLinkElement();
                laneCenter9.AddWidthElement(a: "0");


                var bottomRightSidewalk = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "5.5",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView10 = bottomRightSidewalk.AddPlainViewElement();
                var geometry10 = plainView10.AddGeometryElement(
                    s: "0.0",
                    x: (startX - 10.5f * (float)Math.Cos(hdg + 1.5707963267949) + 15f * (float)Math.Sin(hdg + 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    y: (startY - 10.5f * (float)Math.Sin(hdg + 1.5707963267949) - 15f * (float)Math.Cos(hdg + 1.5707963267949)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg + 1.5707963267949).ToString(),
                    length: "5.5");

                var lanes10 = bottomRightSidewalk.AddLanesElement();
                var laneSection10 = lanes10.AddLaneSectionElement(s: "0");
                var left10 = laneSection10.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk10 = left10.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk10.AddLinkElement();
                laneLeftSidewalk10.AddWidthElement(a: "1.5");
                var center10 = laneSection10.AddDirectionElement(Direction.Center);
                var laneCenter10 = center10.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter10.AddLinkElement();
                laneCenter10.AddWidthElement(a: "0");


                var bottomLeftSidewalkCurve = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "2.3561944901923",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView11 = bottomLeftSidewalkCurve.AddPlainViewElement();
                var geometry11 = plainView11.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 10.5f * (float)Math.Cos(hdg - 1.570796) - 2f * (float)Math.Sin(hdg - 1.570796)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 10.5f * (float)Math.Sin(hdg - 1.570796) + 2f * (float)Math.Cos(hdg - 1.570796)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg - 1.570796).ToString(),
                    length: "2.3561944901923",
                    curvature: "0.66666666667");

                var lanes11 = bottomLeftSidewalkCurve.AddLanesElement();
                var laneSection11 = lanes11.AddLaneSectionElement(s: "0");
                var left11 = laneSection11.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk11 = left11.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk11.AddLinkElement();
                laneLeftSidewalk11.AddWidthElement(a: "1.5");
                var center11 = laneSection11.AddDirectionElement(Direction.Center);
                var laneCenter11 = center11.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter11.AddLinkElement();
                laneCenter11.AddWidthElement(a: "0");


                var bottomRightSidewalkCurve = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "2.3561944901923",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView12 = bottomRightSidewalkCurve.AddPlainViewElement();
                var geometry12 = plainView12.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 13.5f * (float)Math.Cos(hdg) + 12f * (float)Math.Sin(hdg)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 13.5f * (float)Math.Sin(hdg) - 12f * (float)Math.Cos(hdg)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg).ToString(),
                    length: "2.3561944901923",
                    curvature: "0.66666666667");

                var lanes12 = bottomRightSidewalkCurve.AddLanesElement();
                var laneSection12 = lanes12.AddLaneSectionElement(s: "0");
                var left12 = laneSection12.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk12 = left12.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk12.AddLinkElement();
                laneLeftSidewalk12.AddWidthElement(a: "1.5");
                var center12 = laneSection12.AddDirectionElement(Direction.Center);
                var laneCenter12 = center12.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter12.AddLinkElement();
                laneCenter12.AddWidthElement(a: "0");


                var bottomCurveContRight = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "2.3561944901923",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView13 = bottomCurveContRight.AddPlainViewElement();
                var geometry13 = plainView13.AddGeometryElement(
                    s: "0.0",
                    x: (startX - 15f * (float)Math.Cos(hdg - 3.141592) - 3.5f * (float)Math.Sin(hdg - 3.141592)).ToString(CultureInfo.InvariantCulture),
                    y: (startY - 15f * (float)Math.Sin(hdg - 3.141592) + 3.5f * (float)Math.Cos(hdg - 3.141592)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg - 3.141592).ToString(),
                    length: "2.3561944901923",
                    curvature: "0.66666666667");

                var lanes13 = bottomCurveContRight.AddLanesElement();
                var laneSection13 = lanes13.AddLaneSectionElement(s: "0");
                var left13 = laneSection13.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk13 = left13.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk13.AddLinkElement();
                laneLeftSidewalk13.AddWidthElement(a: "1.5");
                var center13 = laneSection13.AddDirectionElement(Direction.Center);
                var laneCenter13 = center13.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter13.AddLinkElement();
                laneCenter13.AddWidthElement(a: "0");


                var bottomCurveContLeft = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "2.3561944901923",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView14 = bottomCurveContLeft.AddPlainViewElement();
                var geometry14 = plainView14.AddGeometryElement(
                    s: "0.0",
                    x: (startX - 5f * (float)Math.Cos(hdg + 1.57) + 3.5f * (float)Math.Sin(hdg + 1.57)).ToString(CultureInfo.InvariantCulture),
                    y: (startY - 5 * (float)Math.Sin(hdg + 1.57) - 3.5f * (float)Math.Cos(hdg + 1.57)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg + 1.57).ToString(),
                    length: "2.3561944901923",
                    curvature: "0.66666666667");

                var lanes14 = bottomCurveContLeft.AddLanesElement();
                var laneSection14 = lanes14.AddLaneSectionElement(s: "0");
                var left14 = laneSection14.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk14 = left14.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk14.AddLinkElement();
                laneLeftSidewalk14.AddWidthElement(a: "1.5");
                var center14 = laneSection14.AddDirectionElement(Direction.Center);
                var laneCenter14 = center14.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter14.AddLinkElement();
                laneCenter14.AddWidthElement(a: "0");


                var bottomLeftStraight = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "2",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView15 = bottomLeftStraight.AddPlainViewElement();
                var geometry15 = plainView15.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 5f * (float)Math.Sin(hdg)).ToString(CultureInfo.InvariantCulture),
                    y: (startY - 5  * (float)Math.Cos(hdg)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg).ToString(),
                    length: "2");

                var lanes15 = bottomLeftStraight.AddLanesElement();
                var laneSection15 = lanes15.AddLaneSectionElement(s: "0");
                var left15 = laneSection15.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk15 = left15.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk15.AddLinkElement();
                laneLeftSidewalk15.AddWidthElement(a: "1.5");
                var center15 = laneSection15.AddDirectionElement(Direction.Center);
                var laneCenter15 = center15.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter15.AddLinkElement();
                laneCenter15.AddWidthElement(a: "0");


                var bottomRightStraight = RootElement.AddRoadElement(
                    name: "Road " + id.ToString(),
                    length: "2",
                    id: id.ToString(),
                    junction: "-1");
                id++;

                var plainView16 = bottomRightStraight.AddPlainViewElement();
                var geometry16 = plainView16.AddGeometryElement(
                    s: "0.0",
                    x: (startX + 15f * (float)Math.Cos(hdg) + 5f * (float)Math.Sin(hdg)).ToString(CultureInfo.InvariantCulture),
                    y: (startY + 15 * (float)Math.Sin(hdg) - 5f * (float)Math.Cos(hdg)).ToString(CultureInfo.InvariantCulture),
                    hdg: (hdg).ToString(),
                    length: "2");

                var lanes16 = bottomRightStraight.AddLanesElement();
                var laneSection16 = lanes16.AddLaneSectionElement(s: "0");
                var left16 = laneSection16.AddDirectionElement(Direction.Left);
                var laneLeftSidewalk16 = left16.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                laneLeftSidewalk16.AddLinkElement();
                laneLeftSidewalk16.AddWidthElement(a: "1.5");
                var center16 = laneSection16.AddDirectionElement(Direction.Center);
                var laneCenter16 = center16.AddLaneElement(id: "0", type: "none", level: "false");
                laneCenter16.AddLinkElement();
                laneCenter16.AddWidthElement(a: "0");
            }
            junctionId++;
        }
    }
}
    
