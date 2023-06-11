using System.Globalization;
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

            rootElement.AddGeoReferenceElement(geoRef: "+proj=tmerc +lat_0=0 +lon_0=0 +k=1 +x_0=0 +y_0=0 +datum=WGS84 +units=m " +
                                                        "+geoidgrids=egm96_15.gtx +vunits=m +no_defs");

            var userDataElement = rootElement.AddUserDataElement();

            userDataElement.AddVectorSceneElement(program: "RoadRunner", version: "2019.2.12 (build 5161c15)");
        }

        /**
         * Generates a straight piece of road
         */
        public XODRRoad AddStraightRoad(float startX = 0, float startY = 0, float hdg = 0, double length = 0, bool crossing = false, float crossingLength = 0.0f, float crossingWidth = 0.0f, string laneWidth = "3.5", int[] predecessorId = null, int[] successorId = null) {
            
            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: length.ToString(),
                id: id.ToString(),
                junction: "-1");

                id++;

            var link = road.AddLinkElement();
            if (predecessorId != null)
            {
                foreach (var id in predecessorId)
                {
                    var predecessor = link.AddPredecessor("road", id.ToString());

                }
            }
            if (successorId != null)
            {
                foreach (var id in successorId)
                {
                    var successor = link.AddSuccessor("road", id.ToString());
                }
            }

            var plainView = road.AddPlainViewElement();
            //TODO modify x and y based on previous road
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
                            laneLeftSidewalk.AddWidthElement(a:"1.5");
                        var laneLeft = left.AddLaneElement(id: "1", type: "driving", level: "false");
                            laneLeft.AddLinkElement();
                            laneLeft.AddWidthElement(a:laneWidth);
                    var center = laneSection.AddDirectionElement(Direction.Center);
                        var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
                            laneCenter.AddLinkElement();
                            laneCenter.AddWidthElement(a: "0");
                    var right = laneSection.AddDirectionElement(Direction.Right);
                        var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
                            laneRight.AddLinkElement();
                            laneRight.AddWidthElement(a:laneWidth);
                        var laneRightSidewalk = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
                            laneRightSidewalk.AddLinkElement();
                            laneRightSidewalk.AddWidthElement(a: "1.5");


            if (crossing) {

                if (crossingLength > 6) {
                    throw new ArgumentOutOfRangeException(nameof(crossingLength), "Crosswalk length must pe smaller then road width");
                }


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
                junction: "-1");

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
                    var left = laneSection.AddDirectionElement(Direction.Left);
                        var laneLeftSidewalk = left.AddLaneElement(id: "2", type: "sidewalk", level: "false");
                            laneLeftSidewalk.AddLinkElement();
                            laneLeftSidewalk.AddWidthElement(a: "1.5");
                        var laneLeft = left.AddLaneElement(id: "1", type: "driving", level: "false");
                            laneLeft.AddLinkElement();
                            laneLeft.AddWidthElement();
                    var center = laneSection.AddDirectionElement(Direction.Center);
                        var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
                            laneCenter.AddLinkElement();
                            laneCenter.AddWidthElement(a: "0");
                    var right = laneSection.AddDirectionElement(Direction.Right);
                        var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
                            laneRight.AddLinkElement();
                            laneRight.AddWidthElement();
                        var laneRightSidewalk = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
                            laneRightSidewalk.AddLinkElement();
                            laneRightSidewalk.AddWidthElement(a:"1.5");

            return road;
        }

        public XODRRoad Add90DegreeTurn(float startX = 0, float startY = 0, double hdg = 0.0)
        {

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "7.8538002104133504",
                id: id.ToString(),
                junction: "-1");

            id++;
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
                        laneLeft.AddLinkElement();
                        laneLeft.AddWidthElement();
                var center = laneSection.AddDirectionElement(Direction.Center);
                    var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
                        laneCenter.AddLinkElement();
                        laneCenter.AddWidthElement(a: "0");
                var right = laneSection.AddDirectionElement(Direction.Right);
                    var laneRight = right.AddLaneElement(id: "-1", type: "driving", level: "false");
                        laneRight.AddLinkElement();
                        laneRight.AddWidthElement();
                    var laneRightSidewalk = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
                        laneRightSidewalk.AddLinkElement();
                        laneRightSidewalk.AddWidthElement(a: "1.5");

            return road;
        }


        //The custom road has no left lane or left sidewalk
        public XODRRoad AddLeftLaneCurve(XODRJunction junction, float startX = 0, float startY = 0, float hdg = 0, float length = 0, string laneWidth = "3.5", string curvature = null, bool sidewalk = false, int[] predecessorId = null, int[] successorId = null)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorId.ToString(),
                connectingRoadId: id.ToString());
            connectionId++;

            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: length.ToString(),
                id: id.ToString(),
                junction: junctionId.ToString());

            id++;


            var link = road.AddLinkElement();
            if (predecessorId != null)
            {
                foreach (var id in predecessorId)
                {
                    var predecessor = link.AddPredecessor("road", id.ToString());

                }
            }
            if (successorId != null)
            {
                foreach (var id in successorId)
                {
                    var successor = link.AddSuccessor("road", id.ToString());
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
                            laneRight.AddLinkElement();
                            laneRight.AddWidthElement(a: laneWidth);

            if(sidewalk == true)
            {
                var laneLeft1 = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
                laneLeft1.AddLinkElement();
                laneLeft1.AddWidthElement(a: "1.5");
            }

            return road;
        }

        public XODRRoad AddRightCurveToIntersection(XODRJunction junction = null, float startX = 0, float startY = 0, double hdg = 0, int predecessorId = 0, int successorId = 0)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorId.ToString(),
                connectingRoadId: id.ToString());
            connectionId++;

            var curve = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "13.41",
                id: id.ToString(),
                junction: junctionId.ToString());
            id++;

            var link = curve.AddLinkElement();
            var predecessor = link.AddPredecessor("road", predecessorId.ToString());
            var successor = link.AddSuccessor("road", successorId.ToString());

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
                y: (startY + 0.1458369439657385f * (float) Math.Sin(hdg)).ToString(),
                hdg: hdg.ToString(),
                length: "13.122688641864244",
                curvature: "-0.11970079986381091");
            var geometry3 = plainView.AddGeometryElement(
                s: "13.268525585829982",
                x: (startX + 8.35416305603426f * (float)Math.Cos(hdg - 1.5707963267948966) - 8.5f * (float)Math.Sin(hdg - 1.5707963267948966)).ToString(),
                y: (startY + 8.35416305603426f * (float)Math.Sin(hdg - 1.5707963267948966) + 8.5f * (float)Math.Cos(hdg - 1.5707963267948966)).ToString(),
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
                            laneRight.AddLinkElement();
                            laneRight.AddWidthElement();
                        var laneRightSidewalk = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
                            laneRightSidewalk.AddLinkElement();
                            laneRightSidewalk.AddWidthElement(a: "1.5");

            return curve;
        }

        public XODRRoad AddLeftCurveToIntersection(XODRJunction junction = null, float startX = 0, float startY = 0,double hdg = 0, int predecessorId = 0, int successorId = 0)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorId.ToString(),
                connectingRoadId: id.ToString());
            connectionId++;

            var curve = RootElement.AddRoadElement(
               name: "Road " + id.ToString(),
               length: "14.679668513160259",
               id: id.ToString(),
               junction: junctionId.ToString());
            id++;

            var link = curve.AddLinkElement();
            var predecessor = link.AddPredecessor("road", predecessorId.ToString());
            var successor = link.AddSuccessor("road", successorId.ToString());

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
                y: (startY + 3.0002131237663852 * (float) Math.Sin(hdg)).ToString(),
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
                x: (startX + 5.4056349186104047f * (float)Math.Cos(hdg - 1.5707963267948966) - 8.5f * (float)Math.Sin(hdg - 1.5707963267948966)).ToString(),
                y: (startY + 5.4056349186104047f * (float)Math.Sin(hdg - 1.5707963267948966) + 8.5f * (float)Math.Cos(hdg - 1.5707963267948966)).ToString(),
                hdg: (hdg - 1.5707963267948970).ToString(),
                length: "3.0943650813895953");

            var lanes = curve.AddLanesElement();
                var laneSection = lanes.AddLaneSectionElement(s: "0");
                    var center = laneSection.AddDirectionElement(Direction.Center);
                        var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
                            laneCenter.AddLinkElement();
                            laneCenter.AddWidthElement(a: "0");
                var left = laneSection.AddDirectionElement(Direction.Left);
                    var laneRight = left.AddLaneElement(id: "1", type: "driving", level: "false");
                        laneRight.AddLinkElement();
                        laneRight.AddWidthElement();

            return curve;

        }

       
        public XODRRoad AddRightStraight(XODRJunction junction = null, float startX = 0, float startY = 0, double hdg = 0, bool sidewalk = false, int predecessorId = 0, int successorId = 0)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorId.ToString(),
                connectingRoadId: id.ToString());
            connectionId++;

            var road = RootElement.AddRoadElement(
               name: "Road " + id.ToString(),
               length: "17",
               id: id.ToString(),
               junction: junctionId.ToString());
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

            if (sidewalk == true)
            {
                var laneLeft1 = right.AddLaneElement(id: "-2", type: "driving", level: "false");
                laneLeft1.AddLinkElement();
                laneLeft1.AddWidthElement(a: "1.5");
            }

            return road;
        }

        public XODRRoad AddLeftStraight(XODRJunction junction = null, float startX = 0, float startY = 0, double hdg = 0, bool sidewalk = false, int predecessorId = 0, int successorId = 0)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorId.ToString(),
                connectingRoadId: id.ToString());
            connectionId++; 
            
            var road = RootElement.AddRoadElement(
               name: "Road " + id.ToString(),
               length: "17",
               id: id.ToString(),
               junction: junctionId.ToString());
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

            if (sidewalk == true)
            {
                var laneLeft1 = left.AddLaneElement(id: "2", type: "driving", level: "false");
                laneLeft1.AddLinkElement();
                laneLeft1.AddWidthElement(a: "1.5");
            }
           

            return road;
        }


        public void Add3wayIntersection(float startX = 0, float startY = 0, float hdg = 0)
        {
            var junction = RootElement.AddJunctionElement(
               name: "Junction " + junctionId.ToString(),
               id: junctionId.ToString());
            junctionId++;

            var incomingRoadId1 = id;
            float startX1 = startX;
            float startY1 = startY;
            var incomingRoad1 = this.AddStraightRoad(startX1, startY1, hdg, 0.5, false);


            float startX2 = startX + 17.5f * (float) Math.Cos(hdg);
            float startY2 = startY + 17.5f * (float) Math.Sin(hdg);
            var incomingRoadId2 = id;
            var incomingRoad2 = this.AddStraightRoad(startX2, startY2, hdg, 0.5, false);

            float startX3 = startX + 9f * (float)Math.Cos(hdg) + 9f * (float)Math.Sin(hdg);
            float startY3 = startY + 9f * (float)Math.Sin(hdg) - 9f * (float)Math.Cos(hdg);
            float hdg3 = hdg + 1.5707963267949f;
            var incomingRoadId3 = id;
            var incomingRoad3 = this.AddStraightRoad(startX3, startY3, hdg3, 0.5, false);

            float startXCurve1 = startX + 9f * (float)Math.Cos(hdg) + 8.5f * (float)Math.Sin(hdg);
            float startYCurve1 = startY + 9f * (float)Math.Sin(hdg) - 8.5f * (float)Math.Cos(hdg);
            float hdgCurve1 = hdg + 1.5707963267949f;
            var curve1Right = this.AddRightCurveToIntersection(junction, startXCurve1, startYCurve1, hdgCurve1, incomingRoadId3, incomingRoadId2);
            var curve1Left = this.AddLeftCurveToIntersection(junction, startXCurve1, startYCurve1, hdgCurve1, incomingRoadId2, incomingRoadId3);

            float startXCurve2 = startX + 0.5f * (float)Math.Cos(hdg);
            float startYCurve2 = startY + 0.5f * (float)Math.Sin(hdg);
            var curve2Right = this.AddRightCurveToIntersection(junction, startXCurve2, startYCurve2, hdg, incomingRoadId1, incomingRoadId3);
            var curve2Left = this.AddLeftCurveToIntersection(junction, startXCurve2, startYCurve2, hdg, incomingRoadId3, incomingRoadId1);

            float startX4 = startX + 0.5f * (float)Math.Cos(hdg);
            float startY4 = startY + 0.5f * (float)Math.Sin(hdg) ;
            var connectionRoad1 = this.AddRightStraight(junction, startX4, startY4, hdg, false, incomingRoadId1, incomingRoadId2);
            var connectionRoad2 = this.AddLeftStraight(junction, startX4, startY4, hdg, true, incomingRoadId2, incomingRoadId1);

            connectionId = 0;

        }

        
        public void Add4wayIntersection(float startX = 0, float startY = 0, float hdg = 0)
        {
            XODRJunction junction = RootElement.AddJunctionElement(
               name: "Junction " + junctionId.ToString(),
               id: junctionId.ToString());
            junctionId++;

            var incomingRoadId1 = id;
            float startX1 = startX;
            float startY1 = startY;
            var incomingRoad1 = this.AddStraightRoad(startX1, startY1, hdg, 0.5f, false);

            float startX2 = startX + 17.5f * (float)Math.Cos(hdg);
            float startY2 = startY + 17.5f * (float)Math.Sin(hdg);
            var incomingRoadId2 = id;
            var incomingRoad2 = this.AddStraightRoad(startX2, startY2, hdg, 0.5f, false);

            float startX3 = startX + 9f * (float)Math.Cos(hdg) + 9f * (float)Math.Sin(hdg);
            float startY3 = startY + 9f * (float)Math.Sin(hdg) - 9f * (float)Math.Cos(hdg);
            float hdg3 = hdg + 1.5707963267949f;
            var incomingRoadId3 = id;
            var incomingRoad3 = this.AddStraightRoad(startX3, startY3, hdg3, 0.5f, false);

            float startX4 = startX + 9f * (float)Math.Cos(hdg) - 8.5f * (float)Math.Sin(hdg);
            float startY4 = startY + 9f * (float)Math.Sin(hdg) + 8.5f * (float)Math.Cos(hdg);
            float hdg4 = hdg + 1.5707963267949f;
            var incomingRoadId4 = id;
            var incomingRoad4 = this.AddStraightRoad(startX4, startY4, hdg4, 0.5f, false);

            float startXCurve1 = startX + 9f * (float)Math.Cos(hdg) + 8.5f * (float)Math.Sin(hdg);
            float startYCurve1 = startY + 9f * (float)Math.Sin(hdg) - 8.5f * (float)Math.Cos(hdg);
            float hdgCurve1 = hdg + 1.5707963267949f;
            var curve1Right = this.AddRightCurveToIntersection(junction, startXCurve1, startYCurve1, hdgCurve1, incomingRoadId3, incomingRoadId2);
            var curve1Left = this.AddLeftCurveToIntersection(junction, startXCurve1, startYCurve1, hdgCurve1, incomingRoadId2, incomingRoadId3);

            float startXCurve2 = startX + 0.5f * (float)Math.Cos(hdg);
            float startYCurve2 = startY + 0.5f * (float)Math.Sin(hdg);
            var curve2Right = this.AddRightCurveToIntersection(junction, startXCurve2, startYCurve2, hdg, incomingRoadId1, incomingRoadId3);
            var curve2Left = this.AddLeftCurveToIntersection(junction, startXCurve2, startYCurve2, hdg, incomingRoadId3, incomingRoadId1);

            float startX5 = startX + 0.5f * (float)Math.Cos(hdg);
            float startY5 = startY + 0.5f * (float)Math.Sin(hdg);
            var connectionRoad1 = this.AddRightStraight(junction, startX5, startY5, hdg, false, incomingRoadId1, incomingRoadId2);
            var connectionRoad2 = this.AddLeftStraight(junction, startX5, startY5, hdg, false, incomingRoadId2, incomingRoadId1);

            float startX6 = startX + 9f * (float)Math.Cos(hdg) - 8.5f * (float)Math.Sin(hdg);
            float startY6 = startY + 9f * (float)Math.Sin(hdg) + 8.5f * (float)Math.Cos(hdg);
            float hdg6 = hdg - 1.5707963f;
            var connectionRoad3 = this.AddRightStraight(junction, startX6, startY6, hdg6, false, incomingRoadId3, incomingRoadId4);
            var connectionRoad4 = this.AddLeftStraight(junction, startX6, startY6, hdg6, false, incomingRoadId4, incomingRoadId3);

            float startXCurve3 = startX + 9f * (float)Math.Cos(hdg) - 8.5f * (float)Math.Sin(hdg);
            float startYCurve3 = startY + 9f * (float)Math.Sin(hdg) + 8.5f * (float)Math.Cos(hdg);
            float hdgCurve3 = hdg - 1.5707963f;
            var curve3Left = this.AddRightCurveToIntersection(junction, startXCurve3, startYCurve3, hdgCurve3, incomingRoadId4, incomingRoadId3);
            var curve3Right = this.AddLeftCurveToIntersection(junction, startXCurve3, startYCurve3, hdgCurve3, incomingRoadId3, incomingRoadId4);

            float startXCurve4 = startX + 17.5f * (float)Math.Cos(hdg);
            float startYCurve4 = startY + 17.5f * (float)Math.Sin(hdg);
            float hdgCurve4 = hdg + 3.1415926f;
            var curve4Left = this.AddRightCurveToIntersection(junction, startXCurve4, startYCurve4, hdgCurve4, incomingRoadId4, incomingRoadId2);
            var curve4Right = this.AddLeftCurveToIntersection(junction: junction, startXCurve4, startYCurve4, hdgCurve4, incomingRoadId2, incomingRoadId4);

            connectionId = 0;

        }

        private XODRRoad AddRoundaboutEntry(XODRJunction junction, float startX = 0, float startY = 0, float hdg = 0, int[] predecessorId = null, int[] successorId = null)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorId.ToString(),
                connectingRoadId: id.ToString());
            connectionId++;

            var road = RootElement.AddRoadElement(
               name: "Road " + id.ToString(),
               length: "7.7704795232679942",
               id: id.ToString(),
               junction: junctionId.ToString());

            id++;

            var link = road.AddLinkElement();
            if (predecessorId != null)
            {
                foreach (var id in predecessorId)
                {
                    var predecessor = link.AddPredecessor("road", id.ToString());

                }
            }
            if (successorId != null)
            {
                foreach (var id in successorId)
                {
                    var successor = link.AddSuccessor("road", id.ToString());
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
                x: (startX + 3.9718521088374819 * (float)Math.Cos(hdg) + 4.8943641564075668e-01 * (float)Math.Sin(hdg)).ToString(),
                y: (startY + 3.9718521088374819 * (float)Math.Sin(hdg) - 4.8943641564075668e-01 * (float)Math.Cos(hdg)).ToString(),
                hdg: (hdg -2.6411809844406342e-01).ToString(),
                length: "3.4683764239600299e+00",
                curvature: "-2.7150009545203924e-01");
            var geometry1 = plainView.AddGeometryElement(
                s: "7.4832511833263258e+00",
                x: (startX + 6.45089662102553073 * (float)Math.Cos(hdg) + 2.7301760090773617e+00 * (float)Math.Sin(hdg)).ToString(),
                y: (startY + 6.45089662102553073 * (float)Math.Sin(hdg) - 2.7301760090773617e+00 * (float)Math.Cos(hdg)).ToString(),
                hdg: (hdg -1.2057826286128144e+00).ToString(),
                length: "2.8706087295301153e-01");

            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var left = laneSection.AddDirectionElement(Direction.Left);
            var laneLeft = left.AddLaneElement(id: "-1", type: "driving", level: "false");
            laneLeft.AddLinkElement();
            laneLeft.AddWidthElement();
            var laneLeft1 = left.AddLaneElement(id: "-2", type: "driving", level: "false");
            laneLeft1.AddLinkElement();
            laneLeft1.AddWidthElement(a: "1.5");

            return road;
        }
        private XODRRoad AddRoundaboutExit(XODRJunction junction, float startX = 0, float startY = 0, float hdg = 0, int[] predecessorId = null, int[] successorId = null)
        {
            var connection = junction.AddConnectionElement(
                id: connectionId.ToString(),
                incomingRoadId: predecessorId.ToString(),
                connectingRoadId: id.ToString());
            connectionId++;

            var road = RootElement.AddRoadElement(
               name: "Road " + id.ToString(),
               length: "7.7704795232679942",
               id: id.ToString(),
               junction: junctionId.ToString());

            id++;


            var link = road.AddLinkElement();
            if (predecessorId != null)
            {
                foreach (var id in predecessorId)
                {
                    var predecessor = link.AddPredecessor("road", id.ToString());

                }
            }
            if (successorId != null)
            {
                foreach (var id in successorId)
                {
                    var successor = link.AddSuccessor("road", id.ToString());
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
                x: (startX + 3.9718521088374819 * (float)Math.Cos(hdg) - 4.8943641564075668e-01 * (float)Math.Sin(hdg)).ToString(),
                y: (startY + 3.9718521088374819 * (float)Math.Sin(hdg) + 4.8943641564075668e-01 * (float)Math.Cos(hdg)).ToString(),
                hdg: (hdg + 2.6411809844406342e-01).ToString(),
                length: "3.4683764239600299e+00",
                curvature: "2.7150009545203924e-01");
            var geometry1 = plainView.AddGeometryElement(
                s: "7.4832511833263258e+00",
                x: (startX + 6.45089662102553073 * (float)Math.Cos(hdg) - 2.7301760090773617e+00 * (float)Math.Sin(hdg)).ToString(),
                y: (startY + 6.45089662102553073 * (float)Math.Sin(hdg) + 2.7301760090773617e+00 * (float)Math.Cos(hdg)).ToString(),
                hdg: (hdg + 1.2057826286128144e+00).ToString(),
                length: "2.8706087295301153e-01");


            var lanes = road.AddLanesElement();
            var laneSection = lanes.AddLaneSectionElement(s: "0");
            var center = laneSection.AddDirectionElement(Direction.Center);
            var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
            laneCenter.AddLinkElement();
            laneCenter.AddWidthElement(a: "0");
            var right = laneSection.AddDirectionElement(Direction.Left);
            var laneRight = right.AddLaneElement(id: "1", type: "driving", level: "false");
            laneRight.AddLinkElement();
            laneRight.AddWidthElement();
            var laneRight1 = right.AddLaneElement(id: "2", type: "driving", level: "false");
            laneRight1.AddLinkElement();
            laneRight1.AddWidthElement(a: "1.5");

            return road;
        }

        public void Add3WayRoundAbout(float startX = 0, float startY = 0, float hdg = 0.0f)
        {
            XODRJunction junction = RootElement.AddJunctionElement(
               name: "Junction " + junctionId.ToString(),
               id: junctionId.ToString());
            
            var road0 = this.AddStraightRoad(startX: startX, startY: startY, hdg: hdg, length: 5.5999755859375000e+00);
            var road2 = this.AddStraightRoad(startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg, length: 5.5999755859375000e+00f);
            var road3 = this.AddStraightRoad(startX: startX + 20 * (float)Math.Cos(hdg) + 20f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 20 * (float)Math.Cos(hdg), hdg:  hdg + 1.5707963267948966e+00f, length: 5.5999755859375000e+00f);

            var road4 = this.AddLeftLaneCurve(junction: junction, startX: startX + 12.1533743648088794f * (float)Math.Cos(hdg) + 2.9984095338145988e+00f * (float)Math.Sin(hdg), startY: startY + 12.1533743648088794f * (float)Math.Sin(hdg) - 2.9984095338145988e+00f * (float)Math.Cos(hdg), hdg: hdg -1.2057917902788586e+00f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true);
            var road5 = this.AddLeftLaneCurve(junction: junction, startX: startX + 22.9984095338145988f * (float)Math.Cos(hdg) + 7.8466256351911206f * (float)Math.Sin(hdg), startY: startY + 22.9984095338145988f * (float)Math.Sin(hdg) - 7.8466256351911206f * (float)Math.Cos(hdg), hdg: hdg -1.2057917902788586e+00f + 1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true);
            var road6 = this.AddLeftLaneCurve(junction: junction, startX: startX + 27.8466256351911206f * (float)Math.Cos(hdg) - 2.9984095338145988e+00f * (float)Math.Sin(hdg), startY: startY + 27.8466256351911206f * (float)Math.Sin(hdg) + 2.9984095338145988e+00f * (float)Math.Cos(hdg), hdg: hdg -1.2057917902788586e+00f + 2*1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true);
            var road7 = this.AddLeftLaneCurve(junction: junction, startX: startX + 17.0015905f * (float)Math.Cos(hdg) - 7.8466256351911206f * (float)Math.Sin(hdg), startY: startY + 17.0015905f * (float)Math.Sin(hdg) + 7.8466256351911206f * (float)Math.Cos(hdg), hdg: hdg -1.2057917902788586e+00f + 3*1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true);

            var road8 = this.AddLeftLaneCurve(junction: junction, startX: startX + 12.1534925868695165f * (float)Math.Cos(hdg) - 2.9987188942942149f * (float)Math.Sin(hdg), startY: startY + 12.1534925868695165f * (float)Math.Sin(hdg) + 2.9987188942942149f * (float)Math.Cos(hdg), hdg: hdg -3.6504396273878453e-01f + 3 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01");
            var road9 = this.AddLeftLaneCurve(junction: junction, startX: startX + 17.0012811057057851f * (float)Math.Cos(hdg) + 7.8465074131304835f * (float)Math.Sin(hdg), startY: startY + 17.0012811057057851f * (float)Math.Sin(hdg) - 7.8465074131304835f * (float)Math.Cos(hdg), hdg: hdg -3.6504396273878453e-01f, length: 6.1320759349930771e+00f, curvature: "1.1904762445393628e-01");
            var road10 = this.AddLeftLaneCurve(junction: junction, startX: startX + 27.8466256351911206f * (float)Math.Cos(hdg) + 2.9987188942942149f * (float)Math.Sin(hdg), startY: startY + 27.8466256351911206f * (float)Math.Sin(hdg) - 2.9987188942942149f * (float)Math.Cos(hdg), hdg: hdg -3.6504396273878453e-01f + 1 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01");
            var road11 = this.AddLeftLaneCurve(junction: junction, startX: startX + 22.9987188942942149f * (float)Math.Cos(hdg) - 7.8465074131304835f * (float)Math.Sin(hdg), startY: startY + 22.9987188942942149f * (float)Math.Sin(hdg) + 7.8465074131304835f * (float)Math.Cos(hdg), hdg: hdg -3.6504396273878453e-01f + 2 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01", sidewalk: true);

            var road12 = this.AddRoundaboutEntry(junction: junction, startX: startX + 5.599975585937500f * (float)Math.Cos(hdg), startY: startY + 5.5999755859375000f * (float)Math.Sin(hdg), hdg: hdg);
            var road13 = this.AddRoundaboutEntry(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) + 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 1.57f);
            var road14 = this.AddRoundaboutEntry(junction: junction, startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg + 3.1415f);

            var road16 = this.AddRoundaboutExit(junction: junction, startX: startX + 5.599975585937500f * (float)Math.Cos(hdg), startY: startY + 5.5999755859375000f * (float)Math.Sin(hdg), hdg: hdg);
            var road17 = this.AddRoundaboutExit(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) + 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 1.57f);
            var road18 = this.AddRoundaboutExit(junction: junction, startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg + 3.1415f);

            junctionId++;
            connectionId = 0;

        }

        public void Add4WayRoundAbout(float startX = 0, float startY = 0, float hdg = 0.0f)
        {
            XODRJunction junction = RootElement.AddJunctionElement(
               name: "Junction " + junctionId.ToString(),
               id: junctionId.ToString());

            var road0 = this.AddStraightRoad(startX: startX, startY: startY, hdg: hdg, length: 5.5999755859375000e+00);
            var road1 = this.AddStraightRoad(startX: startX + 20f * (float)Math.Cos(hdg) - 20f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) + 20f * (float)Math.Cos(hdg), hdg: hdg - 1.5707963267948966e+00f, length: 5.5999755859375000e+00f);
            var road2 = this.AddStraightRoad(startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg, length: 5.5999755859375000e+00f);
            var road3 = this.AddStraightRoad(startX: startX + 20 * (float)Math.Cos(hdg) + 20f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 20 * (float)Math.Cos(hdg), hdg:  hdg + 1.5707963267948966e+00f, length: 5.5999755859375000e+00f);

            var road4 = this.AddLeftLaneCurve(junction: junction, startX: startX + 12.1533743648088794f * (float)Math.Cos(hdg) + 2.9984095338145988e+00f * (float)Math.Sin(hdg), startY: startY + 12.1533743648088794f * (float)Math.Sin(hdg) - 2.9984095338145988e+00f * (float)Math.Cos(hdg), hdg: hdg -1.2057917902788586e+00f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true);
            var road5 = this.AddLeftLaneCurve(junction: junction, startX: startX + 22.9984095338145988f * (float)Math.Cos(hdg) + 7.8466256351911206f * (float)Math.Sin(hdg), startY: startY + 22.9984095338145988f * (float)Math.Sin(hdg) - 7.8466256351911206f * (float)Math.Cos(hdg), hdg: hdg -1.2057917902788586e+00f + 1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true);
            var road6 = this.AddLeftLaneCurve(junction: junction, startX: startX + 27.8466256351911206f * (float)Math.Cos(hdg) - 2.9984095338145988e+00f * (float)Math.Sin(hdg), startY: startY + 27.8466256351911206f * (float)Math.Sin(hdg) + 2.9984095338145988e+00f * (float)Math.Cos(hdg), hdg: hdg -1.2057917902788586e+00f + 2*1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true);
            var road7 = this.AddLeftLaneCurve(junction: junction, startX: startX + 17.0015905f * (float)Math.Cos(hdg) - 7.8466256351911206f * (float)Math.Sin(hdg), startY: startY + 17.0015905f * (float)Math.Sin(hdg) + 7.8466256351911206f * (float)Math.Cos(hdg), hdg: hdg -1.2057917902788586e+00f + 3*1.57f, length: 7.0622814306167738e+00f, curvature: "1.1904762445393628e-01", sidewalk: true);

            var road8 = this.AddLeftLaneCurve(junction: junction, startX: startX + 12.1534925868695165f * (float)Math.Cos(hdg) - 2.9987188942942149f * (float)Math.Sin(hdg), startY: startY + 12.1534925868695165f * (float)Math.Sin(hdg) + 2.9987188942942149f * (float)Math.Cos(hdg), hdg: hdg -3.6504396273878453e-01f + 3 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01");
            var road9 = this.AddLeftLaneCurve(junction: junction, startX: startX + 17.0012811057057851f * (float)Math.Cos(hdg) + 7.8465074131304835f * (float)Math.Sin(hdg), startY: startY + 17.0012811057057851f * (float)Math.Sin(hdg) - 7.8465074131304835f * (float)Math.Cos(hdg), hdg: hdg -3.6504396273878453e-01f, length: 6.1320759349930771e+00f, curvature: "1.1904762445393628e-01");
            var road10 = this.AddLeftLaneCurve(junction: junction, startX: startX + 27.8466256351911206f * (float)Math.Cos(hdg) + 2.9987188942942149f * (float)Math.Sin(hdg), startY: startY + 27.8466256351911206f * (float)Math.Sin(hdg) - 2.9987188942942149f * (float)Math.Cos(hdg), hdg: hdg -3.6504396273878453e-01f + 1 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01");
            var road11 = this.AddLeftLaneCurve(junction: junction, startX: startX + 22.9987188942942149f * (float)Math.Cos(hdg) - 7.8465074131304835f * (float)Math.Sin(hdg), startY: startY + 22.9987188942942149f * (float)Math.Sin(hdg) + 7.8465074131304835f * (float)Math.Cos(hdg), hdg: hdg -3.6504396273878453e-01f + 2 * 1.57f, length: 6.1320759349930780e+00f, curvature: "1.1904762445393628e-01");

            var road12 = this.AddRoundaboutEntry(junction: junction, startX: startX + 5.599975585937500f * (float)Math.Cos(hdg), startY: startY + 5.5999755859375000f * (float)Math.Sin(hdg), hdg: hdg);
            var road13 = this.AddRoundaboutEntry(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) + 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 1.57f);
            var road14 = this.AddRoundaboutEntry(junction: junction, startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg + 3.1415f);
            var road15 = this.AddRoundaboutEntry(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) - 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) + 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 3 * 1.57f);

            var road16 = this.AddRoundaboutExit(junction: junction, startX: startX + 5.599975585937500f * (float)Math.Cos(hdg), startY: startY + 5.5999755859375000f * (float)Math.Sin(hdg), hdg: hdg);
            var road17 = this.AddRoundaboutExit(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) + 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) - 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 1.57f);
            var road18 = this.AddRoundaboutExit(junction: junction, startX: startX + 34.4000244140625f * (float)Math.Cos(hdg), startY: startY + 34.4000244140625f * (float)Math.Sin(hdg), hdg: hdg + 3.1415f);
            var road19 = this.AddRoundaboutExit(junction: junction, startX: startX + 20f * (float)Math.Cos(hdg) - 14.4000244140625f * (float)Math.Sin(hdg), startY: startY + 20f * (float)Math.Sin(hdg) + 14.4000244140625f * (float)Math.Cos(hdg), hdg: hdg + 3 * 1.57f);

            junctionId++;
            connectionId = 0;
        }
    }
}
