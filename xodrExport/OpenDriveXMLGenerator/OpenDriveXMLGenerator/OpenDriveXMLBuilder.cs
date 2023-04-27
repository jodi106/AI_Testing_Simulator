using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class OpenDriveXMLBuilder
    {
        public XmlDocument Document { get; set; }
        public XmlElement RootElement { get; set; }

        private int id = 0;
        private int junctionId = 0;
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
                junction: "-1");

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
    }
}