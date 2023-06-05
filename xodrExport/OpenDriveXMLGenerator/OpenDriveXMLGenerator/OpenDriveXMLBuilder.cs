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
        public XODRRoad AddStraightRoad(float startX = 0, float startY = 0, float hdg = 0, double length = 0, bool crossing = false, float crossingLength = 0.0f, float crossingWidth = 0.0f, string laneWidth = "3", int overId = -1) {

            int idr;
            
            if( overId != -1 ) { idr = overId; 
            } else {
                idr = this.id;
            }
            
            var road = RootElement.AddRoadElement(
                name: "Road " + idr.ToString(),
                length: length.ToString(),
                id: idr.ToString(),
                junction: "-1");

            if(idr!=overId)
                id++;
            
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

        public XODRRoad AddCustomRoad(int overId = -1, float startX = 0, float startY = 0, float hdg = 0, float length = 0, string laneWidth = "3", string curvature = null){

            int idr;

            if (overId != -1)
            {
                idr = overId;
            }
            else
            {
                idr = this.id;
            }

            var road = RootElement.AddRoadElement(
                name: "Road " + idr.ToString(),
                length: length.ToString(),
                id: idr.ToString(),
                junction: "-1");

            if (idr != overId)
                id++;

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
                    var laneRightSidewalk = right.AddLaneElement(id: "-2", type: "sidewalk", level: "false");
                            laneRightSidewalk.AddLinkElement();
                            laneRightSidewalk.AddWidthElement(a: "1.5");


            return road;
        }

        public XODRRoad AddRightCurveToIntersection(XODRJunction junction, float startX = 0, float startY = 0, double hdg = 0, int predecessorId = 0, int successorId = 0)
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

        public XODRRoad AddLeftCurveToIntersection(XODRJunction junction, float startX = 0, float startY = 0,double hdg = 0, int predecessorId = 0, int successorId = 0)
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

       
        public XODRRoad AddRightStraight(XODRJunction junction, float startX = 0, float startY = 0, double hdg = 0, bool sidewalk = false, int predecessorId = 0, int successorId = 0)
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
                laneRight.AddWidthElement();
                var laneLeft1 = right.AddLaneElement(id: "-2", type: "driving", level: "false");
                laneLeft1.AddLinkElement();
                laneLeft1.AddWidthElement(a: "1.5");
            }

            return road;
        }

        public XODRRoad AddLeftStraight(XODRJunction junction, float startX = 0, float startY = 0, double hdg = 0, bool sidewalk = false, int predecessorId = 0, int successorId = 0)
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
                laneLeft.AddWidthElement();
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
            var curve4Right = this.AddLeftCurveToIntersection(junction, startXCurve4, startYCurve4, hdgCurve4, incomingRoadId2, incomingRoadId4);

            connectionId = 0;

        }
    
        public XODRRoad AddRoundAbout(float startX = 0, float startY = 0, double hdg = 0.0){

            var road0 = this.AddStraightRoad(overId: 0, startX: -20, startY: 0, hdg: 0, length: 5.5999755859375000e+00, laneWidth: "3.5");
            var road1 = this.AddStraightRoad(overId: 1, startX: 1.2858939716150483e-16f, startY: -1.4400024414062500e+01f, hdg: -1.5707963267948966e+00f, length: 5.5999755859375000e+00f, laneWidth: "3.5");
            var road2 = this.AddStraightRoad(overId: 2, startX: 1.4400024414062500e+01f, startY: 0, hdg: 0, length: 5.5999755859375000e+00f, laneWidth: "3.5");
            var road3 = this.AddStraightRoad(overId: 3, startX: 1.2981407199059296e-16f, startY: 1.4400024414062500e+01f, hdg: 1.5707963267948966e+00f, length: 5.5999755859375000e+00f, laneWidth: "3.5");
            var road4 = this.AddCustomRoad(overId: 5, startX: -7.8466256351911206e+00f, startY: -2.9984095338145988e+00f, hdg: -1.2057917902788586e+00f, length: 7.0622814306167738e+00f, laneWidth: "3.5", curvature: "1.1904762445393628e-01");
            
            return null;
        
        }
    }
}