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

        public XODRRoad AddStraightRoad(float startX = 0 , float startY = 0, float hdg = 0.0f, float length = 0.0f, bool crossing = false, float crossingLength = 0.0f, float crossingWidth = 0.0f){      

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

        public XODRRoad AddCurve(float startX = 0 , float startY = 0, float hdg = 0.0f, float length = 0.0f, float curvature = 0.0)
        {
            var road = RootElement.AddRoadElement(
                name: "Road " + id.ToString(), 
                length: length.ToString(),
                id: id.ToString(),
                junction: "-1");

            id++;
                var plainView = road.AddPlainViewElement();
                    var geometry1 = plainView.AddGeometryElement(
                        s: "0.0",
                        x: startX.ToString(),
                        y: startY.ToString(),
                        hdg: hdg.ToString(),
                        length: length.ToString());

                        var arc = road.AddArcElement(road, curvature.ToString());
            
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
        }

        public XODRRoad AddCurveToIntersection(float startX = 0, float startY = 0, float arc = 0, float hdg, bool isIntersection = false, XODRRoad incomingRoad1, int incomingRoad2)
        {
            var curve1 = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "7.8539816339744830e+00",
                id: id.ToString(),
                junction: "1");
            id++;

            var link1 = AddLinkElement(curve1);
                var predecessor1 = AddPredecessor(link1, "road", incomingRoad1.toString());
                var successor1 = AddSuccessor(link1, "road", incomingRoad2.toString());

            var plainView1 = curve1.AddPlainViewElement();
            var geometry1 = plainView1.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(),
                y: startY.ToString(),
                hdg: hdg.toString(),
                length: length.ToString());
                var arc1 = geometry1.AddArcElement(geometry1, arc.toString());

            var lanes1 = curve1.AddLanesElement();
                var laneSection1 = lanes1.AddLaneSectionElement(s: "0");
                    var left1 = laneSection1.AddDirectionElement(Direction.Left);
                        var laneLeft1 = left1.AddLaneElement(id: "1", type: "driving", level: "false");
                            laneLeft1.AddLinkElement();
                            laneLeft1.AddWidthElement();
                    var center1 = laneSection1.AddDirectionElement(Direction.Center);
                        var laneCenter1 = center1.AddLaneElement(id: "0", type: "none", level: "false");
                            laneCenter1.AddLinkElement();
                            laneCenter1.AddWidthElement(a: "0");
                    var right1 = laneSection2.AddDirectionElement(Direction.Right);


            var curve2 = RootElement.AddRoadElement(
                name: "Road " + id.ToString(),
                length: "7.8539816339744830e+00",
                id: id.ToString(),
                junction: "1");
            id++;

            var link2 = AddLinkElement(curve2);
                var predecessor2 = AddPredecessor(link2, "road", incomingRoad2.toString());
                var successor2 = AddSuccessor(link2, "road", incomingRoad1.toString());

            var plainView2 = curve2.AddPlainViewElement();
            var geometry2 = plainView2.AddGeometryElement(
                s: "0.0",
                x: startX.ToString(),
                y: startY.ToString(),
                hdg: hdg.toString(),
                length: length.ToString());
                var arc2 = geometry1.AddArcElement(geometry1, arc.toString());

            var lanes2 = curve2.AddLanesElement();
                var laneSection2 = lanes2.AddLaneSectionElement(s: "0");
                    var left2 = laneSection2.AddDirectionElement(Direction.Left);
                    var center2 = laneSection2.AddDirectionElement(Direction.Center);
                        var laneCenter2 = center2.AddLaneElement(id: "0", type: "none", level: "false");
                            laneCenter2.AddLinkElement();
                            laneCenter2.AddWidthElement(a: "0");
                    var right2 = laneSection2.AddDirectionElement(Direction.Right);
                        var laneRight2 = right2.AddLaneElement(id: "-1", type: "driving", level: "false");
                            laneRight2.AddLinkElement();
                            laneRight2.AddWidthElement();
            
        }

        public XODRJunction Add3wayIntersection(float startX = 0, float startY = 0)
        {
            float length = 30;

            var incomingRoad1 = this.AddStraightRoad(startX: startX, startY: startY, hdg: 0, lenght: length, false);

            var incomingRoad2 = this.AddStraightRoad(startX + 35, startY - 35, 1.5707963267949, length, false);

            var incomingRoad3 = this.AddStraightRoad(startX + 35, startY + 5, 1.5707963267949, length, false);

            var curve1 = this.AddCurveToIntersection(startX + 30, startY, -0.2, 0.0, incomingRoad1, incomingRoad2);

            var curve2 = this.AddCurveToIntersection(startX + 30, startY, 0.2, 0.0, incomingRoad2, incomingRoad3);

            var connectionRoad1 = this.AddStraightRoad(startX + 35, startY - 5, 1.5707963267949, 10, false);

            return junction;
        }

        public XODRJunction Add4wayIntersection(float startX = 0, float startY = 0)
        {
            float length = 30;

            var incomingRoad1 = this.AddStraightRoad(startX, startY, 0, length, false);

            var incomingRoad2 = this.AddStraightRoad(startX + 35, startY - 35, 1.5707963267949, length, false);

            var incomingRoad3 = this.AddStraightRoad(startX + 35, startY + 5, 1.5707963267949, length, false);

            var incomingRoad4 = this.AddStraightRoad(startX + 40, startY, 0, length, false);

            var curve1 = this.AddCurveToIntersection(startX + 30, startY, -0.2, 0.0, incomingRoad1.id, incomingRoad2.id);

            var curve2 = this.AddCurveToIntersection(startX + 30, startY, 0.2, 0.0, incomingRoad2.id, incomingRoad3.id);

            var curve3 = this.AddCurveToIntersection(startX + 30, startY, -0.2, 3.1415926535898, incomingRoad2.id, incomingRoad3.id);

            var curve4 = this.AddCurveToIntersection(startX + 30, startY, 0.2, 3.1415926535898, incomingRoad2.id, incomingRoad3.id);

            var connectionRoad1 = this.AddStraightRoad(startX + 35, startY - 5, 0, 10, false);

            var connectionRoad2 = this.AddStraightRoad(startX + 30, startY, 1.5707963267949, 10, false);

            return junction;
        }
    }
}