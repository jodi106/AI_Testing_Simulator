using System.Globalization;
using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class OpenDriveXMLBuilder
    {
        public XmlDocument Document { get; set; }
        public XmlElement RootElement { get; set; }

        private int id = 0;
        private float x;
        private float y;

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

            rootElement.AddGeoReferenceElement(geoRef: "+proj=tmerc +lat_0=0 +lon_0=0 +k=1 +x_0=0 +y_0=0 +datum=WGS84 +units=m " +
                                                        "+geoidgrids=egm96_15.gtx +vunits=m +no_defs");

            var userDataElement = rootElement.AddUserDataElement();

            userDataElement.AddVectorSceneElement(program: "RoadRunner", version: "2019.2.12 (build 5161c15)");
        }

        public XODRRoad AddStraightRoad(float startX = 0, float startY = 0, double hdg = 0.0, double length = 0.0, bool crossing = false, float crossingLength = 0.0f, float crossingWidth = 0.0f) {

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: length.ToString(),
                id: id.ToString(),
                junction: "1");

            id++;
            var plainView = road.AddPlainViewElement();
            //TODO modify x and y based on previous road
            var geometry1 = plainView.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(),
                y: startY.ToString(),
                hdg: hdg.ToString(),
                length: length.ToString());

            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var leftSidewalk = laneSection.AddDirectionElement(Direction.Left);
            var laneLeftSidewalk = leftSidewalk.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
            laneLeftSidewalk.AddLinkElement();
            laneLeftSidewalk.AddWidthElement(a: "1.5");
            var left = laneSection.AddDirectionElement(Direction.Left);
            var laneLeft = left.AddLaneElement(id: "-1", type: "driving", level: "false");
            laneLeft.AddLinkElement();
            laneLeft.AddWidthElement();
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Right);
            var laneRight = right.AddLaneElement(id: "1", type: "driving", level: "false");
            laneRight.AddLinkElement();
            laneRight.AddWidthElement();
            var rightSidewalk = laneSection.AddDirectionElement(Direction.Left);
            var laneRightSidewalk = rightSidewalk.AddLaneElement(id: "2", type: "sidewalk", level: "false");
            laneRightSidewalk.AddLinkElement();
            laneRightSidewalk.AddWidthElement(a: "1.5");


            if (crossing) {

                if (crossingLength > 6) {
                    throw new ArgumentOutOfRangeException(nameof(crossingLength), "Crosswalk length must pe smaller then road width");
                }

                float u = 0.0f;
                float v = 0.0f;

                var objects = road.AddObjectsElement();
                var obj = objects.AddObjectElement(s: (length / 2).ToString(), t: "0", zOffset: "0");
                var outline = obj.AddOutlineElement();
                /*
                    The order of the corners is important, it draws the shape sequential.
                */
                if (crossingLength == 0.0f || crossingWidth == 0.0f) {
                    outline.AddCornerLocalElement((-3).ToString(), (-1.5).ToString());
                    outline.AddCornerLocalElement((-3).ToString(), 1.5.ToString());
                    outline.AddCornerLocalElement(3.ToString(), 1.5.ToString());
                    outline.AddCornerLocalElement(3.ToString(), (-1.5).ToString());

                } else {
                    outline.AddCornerLocalElement((-crossingLength / 2).ToString(), (-crossingWidth / 2).ToString());
                    outline.AddCornerLocalElement((-crossingLength / 2).ToString(), (crossingWidth / 2).ToString());
                    outline.AddCornerLocalElement((crossingLength / 2).ToString(), (crossingWidth / 2).ToString());
                    outline.AddCornerLocalElement((crossingLength / 2).ToString(), (-crossingWidth / 2).ToString());
                }


            }


            return road;
        }

        public XODRRoad Add15DegreeTurn(float startX = 0, float startY = 0, double hdg = 0.0)
        {

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "1.3089411479932687",
                id: id.ToString(),
                junction: "1");

            id++;
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
            var leftSidewalk = laneSection.AddDirectionElement(Direction.Left);
            var laneLeftSidewalk = leftSidewalk.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
            laneLeftSidewalk.AddLinkElement();
            laneLeftSidewalk.AddWidthElement(a: "1.5");
            var left = laneSection.AddDirectionElement(Direction.Left);
            var laneLeft = left.AddLaneElement(id: "-1", type: "driving", level: "false");
            laneLeft.AddLinkElement();
            laneLeft.AddWidthElement();
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Right);
            var laneRight = right.AddLaneElement(id: "1", type: "driving", level: "false");
            laneRight.AddLinkElement();
            laneRight.AddWidthElement();
            var rightSidewalk = laneSection.AddDirectionElement(Direction.Left);
            var laneRightSidewalk = rightSidewalk.AddLaneElement(id: "2", type: "sidewalk", level: "false");
            laneRightSidewalk.AddLinkElement();
            laneRightSidewalk.AddWidthElement(a:"1.5");

            return road;
        }


        public XODRRoad AddRightCurveToIntersection(float startX = 0, float startY = 0, double hdg = 0, int predecessorId = 0, int successorId = 0)
        {
            var arc = -0.1197;

            var curve = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "13.41",
                id: id.ToString(),
                junction: "-1");
            id++;

            var link = curve.AddLinkElement();
            var predecessor = link.AddPredecessor("road", predecessorId.ToString());
            var successor = link.AddSuccessor("road", successorId.ToString());

            var plainView = curve.AddPlainViewElement();
            var geometry1 = plainView.AddGeometryElement(
                s: "0",
                x: startX.ToString(),
                y: (startY + 0.5).ToString(),
                hdg: hdg.ToString(),
                length: "0.14583694396573854");
            var geometry2 = plainView.AddGeometryElement(
                s: "0.14583694396573854",
                x: startX.ToString(),
                y: (startY + 0.6458).ToString(),
                hdg: hdg.ToString(),
                length: "13.122688641864244",
                curvature: "-0.11970079986381091");
            var geometry3 = plainView.AddGeometryElement(
                s: (startX + 13.122688641864244).ToString(),
                x: (startX + 8.3541630560342615).ToString(),
                y: startY.ToString(),
                hdg: "0",
                length: "0.1455");

            var lanes = curve.AddLanesElement();
                var laneSection = lanes.AddLaneSectionElement(s: "0");
                    var right = laneSection.AddDirectionElement(Direction.Left);
                        var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
                            laneRight.AddLinkElement();
                            laneRight.AddWidthElement();
                    var center = laneSection.AddDirectionElement(Direction.Center);
                        var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
                            laneCenter.AddLinkElement();
                            laneCenter.AddWidthElement(a: "0");

            return curve;

        }

        public XODRRoad AddLeftCurveToIntersection(float startX = 0, float startY = 0,double hdg = 0, int predecessorId = 0, int successorId = 0)
        {
            var curve = RootElement.AddRoadElement(
               name: "Road " + id.ToString(),
               length: "14.679668513160259",
               id: id.ToString(),
               junction: "-1");
            id++;

            var link = curve.AddLinkElement();
            var predecessor = link.AddPredecessor("road", predecessorId.ToString());
            var successor = link.AddSuccessor("road", successorId.ToString());

            var plainView = curve.AddPlainViewElement();
            var geometry1 = plainView.AddGeometryElement(
                s: "0",
                x: (startX).ToString(),
                y: (startY + 0.5).ToString(),
                hdg: hdg.ToString(),
                length: "2.9999999999999991");
            var geometry2 = plainView.AddGeometryElement(
                s: "2.9999999999999991",
                x: (startX).ToString(),
                y: (startY + 3.5).ToString(),
                hdg: hdg.ToString(),
                length: "0.094151957623209270");
            var geometry3 = plainView.AddGeometryElement(
                s: "3.0941519576232084",
                x: (startX).ToString(),
                y: (startY + 3.5943650813895935).ToString(),
                hdg: hdg.ToString(),
                length: "8.49115147414745414",
                curvature: "-0.1849921452440712");
            var geometry4 = plainView.AddGeometryElement(
                s: "11.585303431770662",
                x: (startX + 3.5943650813895953).ToString(),
                y: (startY + 9).ToString(),
                hdg: "0",
                length: "3.0943650813895953");

            var lanes = curve.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Left);
            var laneRight = right.AddLaneElement(id: "1", type: "driving", level: "false");
            laneRight.AddLinkElement();
            laneRight.AddWidthElement();

            return curve;

        }

        public XODRRoad AddRightStraightToIntersection(float startX = 0, float startY = 0, double hdg = 0, int predecessorId = 0, int successorId = 0)
        {
            var road = RootElement.AddRoadElement(
               name: "Road " + id.ToString(),
               length: "17",
               id: id.ToString(),
               junction: "-1");
            id++;

            var link = road.AddLinkElement();
            var predecessor = link.AddPredecessor("road", predecessorId.ToString());
            var successor = link.AddSuccessor("road", successorId.ToString());

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
            laneRight.AddLinkElement();
            laneRight.AddWidthElement();

            return road;
        }

        public XODRRoad AddLeftStraightToIntersection(float startX = 0, float startY = 0, double hdg = 0, int predecessorId = 0, int successorId = 0)
        {
            var road = RootElement.AddRoadElement(
               name: "Road " + id.ToString(),
               length: "17",
               id: id.ToString(),
               junction: "-1");
            id++;

            var link = road.AddLinkElement();
            var predecessor = link.AddPredecessor("road", predecessorId.ToString());
            var successor = link.AddSuccessor("road", successorId.ToString());

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
                            laneLeft.AddLinkElement();
                            laneLeft.AddWidthElement();

            return road;
        }


        public void Add3wayIntersection(float startX = 0, float startY = 0)
        {
            var incomingRoadId1 = id;
            var incomingRoad1 = this.AddStraightRoad(startX, startY, 0, 0.5, false);

            float startX2 = startX + 17.5f;
            float startY2 = startY;
            var incomingRoadId2 = id;
            var incomingRoad2 = this.AddStraightRoad(startX2, startY2, 0, 0.5, false);

            float startX3 = startX + 9;
            float startY3 = startY - 9;
            var incomingRoadId3 = id;
            var incomingRoad3 = this.AddStraightRoad(startX3, startY3, 1.5707963267949, 0.5, false);

            float startXCurve1 = startX + 9;
            float startYCurve1 = startY - 9;
            var curve1Right = this.AddRightCurveToIntersection(startXCurve1, startYCurve1, 1.5707963267949, incomingRoadId3, incomingRoadId2);
            var curve1Left = this.AddLeftCurveToIntersection(startXCurve1, startYCurve1, 1.5707963267949, incomingRoadId2, incomingRoadId3);

            float startXCurve2 = startX + 0.5f;
            float startYCurve2 = startY - 0.6458f;
            var curve2Right = this.AddRightCurveToIntersection(startXCurve2, startYCurve2, 0, incomingRoadId1, incomingRoadId3);
            var curve2Left = this.AddLeftCurveToIntersection(startXCurve2, startYCurve2, 0, incomingRoadId3, incomingRoadId1);

            float startX4 = startX + 0.5f;
            float startY4 = startY;
            var connectionRoad1 = this.AddRightStraightToIntersection(startX4, startY4, 0);
            var connectionRoad2 = this.AddLeftStraightToIntersection(startX4, startY4, 0);
        }

        /**public void Add4wayIntersection(float startX = 0, float startY = 0)
        {
            float length = 3;

            var incomingRoadId1 = id;
            var incomingRoad1 = this.AddStraightRoad(startX, startY, 0, length, false);

            float startX2 = startX + 8;
            float startY2 = startX - 8;
            var incomingRoadId2 = id;
            var incomingRoad2 = this.AddStraightRoad(startX2, startY2, 1.5707963267949, length, false);

            float startX3 = startX;
            float startY3 = startY + 5;
            var incomingRoadId3 = id;
            var incomingRoad3 = this.AddStraightRoad(startX3, startY3, 1.5707963267949, length, false);

            float startX4 = startX + 13;
            float startY4 = startY;
            var incomingRoadId4 = id;
            var incomingRoad4 = this.AddStraightRoad(startX4, startY4, 0, length, false);

            float startXCurve1 = startX + 8;
            float startYCurve1 = startY - 5;
            var curve1Left = this.AddRightCurveToIntersection(startXCurve1, startYCurve1,1.5707963267949, incomingRoadId1, incomingRoadId2);
            var curve1Right = this.AddLeftCurveToIntersection(startXCurve1, startYCurve1, 1.5707963267949, incomingRoadId2, incomingRoadId1);

            float startXCurve2 = startX + 3;
            float startYCurve2 = startY;
            var curve2Left = this.AddRightCurveToIntersection(startXCurve2, startYCurve2, 0, incomingRoadId1, incomingRoadId2);
            var curve2Right = this.AddLeftCurveToIntersection(startXCurve2, startYCurve2, 0, incomingRoadId2, incomingRoadId1);

            float startXCurve3 = startX + 8;
            float startYCurve3 = startY + 5;
            var curve3Left = this.AddRightCurveToIntersection(startXCurve3, startYCurve3, 4.7123889803846899, incomingRoadId4, incomingRoadId3);
            var curve3Right = this.AddLeftCurveToIntersection(startXCurve3, startYCurve3, 4.7123889803846899, incomingRoadId3, incomingRoadId4);

            float startXCurve4 = startX + 13;
            float startYCurve4 = startY;
            var curve4Left = this.AddRightCurveToIntersection(startXCurve4, startYCurve4, 3.1415926535898, incomingRoadId4, incomingRoadId2);
            var curve4Right = this.AddLeftCurveToIntersection(startXCurve4, startYCurve4, 3.1415926535898, incomingRoadId2, incomingRoadId4);

            float startX5 = startX + 3;
            float startY5 = startY;
            var connectionRoad1 = this.AddStraightRoad(startX5, startY5, 0, 10, false);

            float startX6 = startX + 8;
            float startY6 = startY - 5;
            var connectionRoad2 = this.AddStraightRoad(startX6, startY6, 1.5707963267949, 10, false);
        }*/
    }
}