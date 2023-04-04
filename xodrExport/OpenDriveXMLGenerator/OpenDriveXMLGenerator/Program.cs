using System;
using System.Xml;
using System.Linq;
using System.Xml.Linq;

namespace OpenDriveXMLGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new OpenDriveXMLBuilder();

            var document = builder.Document;

            var rootElement = builder.RootElement;

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

            var road64 = rootElement.AddRoadElement(
                name: "Road 64", 
                length: "1.0234747062906101e+2",
                id: "64",
                junction: "-1");

            var link = road64.AddLinkElement(elementType: "junction", elementId: "455");

            var type = road64.AddTypeElement(s: "0.0", type: "town");

            var speed = type.AddSpeedElement(max: "55", unit: "mph");

            var plainView = road64.AddPlainViewElement();

            var geometry1 = plainView.AddGeometryElement(
                s: "0.0",
                x: "-2.3697531961010279e+2",
                y: "3.7372531134669975e+0",
                hdg: "2.9484610789761891e+0",
                length: "1.4798839982417746e+1",
                curvature: "-1.0668617530566759e-2");




            document.Save("OpenDrive.xml");

            Console.WriteLine("OpenDrive.xml generated");
        }
    }
}
