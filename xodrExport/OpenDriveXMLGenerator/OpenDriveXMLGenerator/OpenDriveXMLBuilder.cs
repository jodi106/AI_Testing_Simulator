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

            rootElement.AddGeoReferenceElement( geoRef: "+proj=tmerc +lat_0=0 +lon_0=0 +k=1 +x_0=0 +y_0=0 +datum=WGS84 +units=m " +
                                                        "+geoidgrids=egm96_15.gtx +vunits=m +no_defs");

            var userDataElement = rootElement.AddUserDataElement();

            userDataElement.AddVectorSceneElement(program: "RoadRunner", version: "2019.2.12 (build 5161c15)");
        }

        public XODRRoad AddStraightRoad(float startX = 0 , float startY = 0, double hdg = 0.0, float length = 0.0f, bool crossing = false, float crossingLength = 0.0f, float crossingWidth = 0.0f){      

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
                var laneSection = lanes.AddLaneSectionElement(s:"0");
                    var left = laneSection.AddDirectionElement(Direction.Left);
                        var lane = left.AddLaneElement(id:"1", type:"driving", level:"false");
                            lane.AddLinkElement();
                            lane.AddWidthElement();
                    var center = laneSection.AddDirectionElement(Direction.Center);
                        var laneCenter = center.AddLaneElement(id:"0", type:"none", level:"false");
                            laneCenter.AddLinkElement();
                            laneCenter.AddWidthElement(a:"0");
                    var right = laneSection.AddDirectionElement(Direction.Right);
                        var laneRight = right.AddLaneElement(id:"-1", type:"driving", level:"false");
                            laneRight.AddLinkElement();
                            laneRight.AddWidthElement();


            if(crossing){

                if(crossingLength > 6){
                    throw new ArgumentOutOfRangeException(nameof(crossingLength), "Crosswalk length must pe smaller then road width");
                }

            float u = 0.0f;
            float v = 0.0f;
            
            var objects = road.AddObjectsElement();
                var obj = objects.AddObjectElement(s:(length/2).ToString(),t:"0", zOffset:"0");
                    var outline = obj.AddOutlineElement();
                    /*
                        The order of the corners is important, it draws the shape sequential.
                    */
                        if(crossingLength == 0.0f || crossingWidth ==0.0f){
                            outline.AddCornerLocalElement((-3).ToString(),(-1.5).ToString());
                            outline.AddCornerLocalElement((-3).ToString(), 1.5.ToString());
                            outline.AddCornerLocalElement(3.ToString(), 1.5.ToString());
                            outline.AddCornerLocalElement(3.ToString(), (-1.5).ToString());

                        }else{
                            outline.AddCornerLocalElement((-crossingLength/2).ToString(), (-crossingWidth/2).ToString());
                            outline.AddCornerLocalElement((-crossingLength/2).ToString(), (crossingWidth/2).ToString());
                            outline.AddCornerLocalElement((crossingLength/2).ToString(), (crossingWidth/2).ToString());
                            outline.AddCornerLocalElement((crossingLength/2).ToString(), (-crossingWidth/2).ToString());    
                        }
                        
            
            }
            

            return road;
        }

        
        public XODRRoad AddLeftCurveToIntersection(float startX = 0, float startY = 0, double arc = 0, double hdg,  int predecessorId, int successorId)
        {
            var curve = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "7.8539816339744830e+00",
                id: id.ToString(),
                junction: "-1");
            id++;

            var link = AddLinkElement(curve);
                var predecessor = link.AddPredecessor(link, "road", predecessorId.ToString());
                var successor = link.AddSuccessor(link, "road", successorId.ToString());

            var plainView = curve.AddPlainViewElement();
            var geometry = plainView.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(),
                y: startY.ToString(),
                hdg: hdg.ToString(),
                length: "7.8539816339744830e+00");
                var arc = geometry.AddArcElement(geometry, arc.ToString());

            var lanes = curve.AddLanesElement();
                var laneSection = lanes.AddLaneSectionElement(s: "0");
                    var left = laneSection.AddDirectionElement(Direction.Left);
                        var laneLeft = left.AddLaneElement(id: "1", type: "driving", level: "false");
                            laneLeft.AddLinkElement();
                            laneLeft.AddWidthElement();
                    var center = laneSection.AddDirectionElement(Direction.Center);
                        var laneCenter = center.AddLaneElement(id: "0", type: "none", level: "false");
                            laneCenter.AddLinkElement();
                            laneCenter.AddWidthElement(a: "0");

            return curve;
            
        }

        public XODRRoad AddRightCurveToIntersection(float startX = 0, float startY = 0, double arc = 0, double hdg = 0, int predecessorId, int successorId)
        {
            var curve = RootElement.AddRoadElement(
               name: "Road " + id.ToString(),
               length: "7.8539816339744830e+00",
               id: id.ToString(),
               junction: "-1");
            id++;

            var link = AddLinkElement(curve);
            var predecessor = link.AddPredecessor(link, "road", predecessorId.toString());
            var successor = link.AddSuccessor(link, "road", successorId.toString());

            var plainView = curve.AddPlainViewElement();
            var geometry = plainView.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(),
                y: startY.ToString(),
                hdg: hdg.toString(),
                length: "7.8539816339744830e+00");
            var arc = geometry.AddArcElement(geometry, arc.ToString());

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

            return curve;

        }


        public void Add3wayIntersection(float startX = 0, float startY = 0)
        {
            float length = 30;

            var incomingRoadId1 = id;
            var incomingRoad1 = this.AddStraightRoad(startX, startY, 0, length, false);

            float startX2 = startX + 35;
            float startY2 = startX - 35;
            var incomingRoadId2 = id;
            var incomingRoad2 = this.AddStraightRoad(startX2, startY2, 1.5707963267949, length, false);

            float startX3 = startX;
            float startY3 = startY + 5;
            var incomingRoadId3 = id;
            var incomingRoad3 = this.AddStraightRoad(startX3, startY3, 1.5707963267949, length, false);

            float startXCurve1 = startX + 35;
            float startYCurve1 = startY - 5;
            var curve1Left = this.AddLeftCurveToIntersection(startXCurve1, startYCurve1, 0.2, 1.5707963267949, incomingRoadId1, incomingRoadId2);
            var curve1Right = this.AddRightCurveToIntersection(startXCurve1, startYCurve1, 0.2, 1.5707963267949, incomingRoadId2, incomingRoadId1);

            float startXCurve2 = startX + 30;
            float startYCurve2 = startY;
            var curve2Left = this.AddLeftCurveToIntersection(startXCurve2, startYCurve2, 0.2, 0.0, incomingRoadId1, incomingRoadId2);
            var curve2Right = this.AddRightCurveToIntersection(startXCurve2, startYCurve2, 0.2, 0, incomingRoadId2, incomingRoadId1);

            float startX4 = startX + 35;
            float startY4 = startX - 5;
            var connectionRoad1 = this.AddStraightRoad(startX4, startY4, 1.5707963267949, 10, false);
        }

        public void Add4wayIntersection(float startX = 0, float startY = 0)
        {
            float length = 30;

            var incomingRoadId1 = id;
            var incomingRoad1 = this.AddStraightRoad(startX, startY, 0, length, false);

            float startX2 = startX + 35;
            float startY2 = startX - 35;
            var incomingRoadId2 = id;
            var incomingRoad2 = this.AddStraightRoad(startX2, startY2, 1.5707963267949, length, false);

            float startX3 = startX;
            float startY3 = startY + 5;
            var incomingRoadId3 = id;
            var incomingRoad3 = this.AddStraightRoad(startX3, startY3, 1.5707963267949, length, false);

            float startX4 = startX + 40;
            float startY4 = startY;
            var incomingRoadId4 = id;
            var incomingRoad4 = this.AddStraightRoad(startX4, startY4, 0, length, false);

            float startXCurve1 = startX + 35;
            float startYCurve1 = startY - 5;
            var curve1Left = this.AddLeftCurveToIntersection(startXCurve1, startYCurve1, 0.2, 1.5707963267949, incomingRoadId1, incomingRoadId2);
            var curve1Right = this.AddRightCurveToIntersection(startXCurve1, startYCurve1, 0.2, 1.5707963267949, incomingRoadId2, incomingRoadId1);

            float startXCurve2 = startX + 30;
            float startYCurve2 = startY;
            var curve2Left = this.AddLeftCurveToIntersection(startXCurve2, startYCurve2, 0.2, 0, incomingRoadId1, incomingRoadId2);
            var curve2Right = this.AddRightCurveToIntersection(startXCurve2, startYCurve2, 0.2, 0, incomingRoadId2, incomingRoadId1);

            float startXCurve3 = startX + 35;
            float startYCurve3 = startY + 5;
            var curve3Left = this.AddLeftCurveToIntersection(startXCurve3, startYCurve3, 0.2, 4.7123889803846899, incomingRoadId4, incomingRoadId3);
            var curve3Right = this.AddRightCurveToIntersection(startXCurve3, startYCurve3, 0.2, 4.7123889803846899, incomingRoadId3, incomingRoadId4);

            float startXCurve4 = startX + 40;
            float startYCurve4 = startY;
            var curve4Left = this.AddLeftCurveToIntersection(startXCurve4, startYCurve4, 0.2, 3.1415926535898, incomingRoadId4, incomingRoadId2);
            var curve4Right = this.AddRightCurveToIntersection(startXCurve4, startYCurve4, 0.2, 3.1415926535898, incomingRoadId2, incomingRoadId4);

            float startX5 = startX + 30;
            float startY5 = startY;
            var connectionRoad1 = this.AddStraightRoad(startX5, startY5, 0, 10, false);

            float startX6 = startX + 35;
            float startY6 = startY - 5;
            var connectionRoad2 = this.AddStraightRoad(startX6, startY6, 1.5707963267949, 10, false);
        }
    }
}