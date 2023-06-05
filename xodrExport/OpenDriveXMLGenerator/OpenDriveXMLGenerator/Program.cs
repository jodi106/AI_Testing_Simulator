using System.Xml;

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

            builder.AddRoundAbout(); 

            builder.Document.Save("OpenDrive.xodr");

            Console.WriteLine("OpenDrive.xodr generated");

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
            var road19 = builder.AddStraightRoad(startX: 129, startY: -205, hdg: 1.5707963267949f, length: 91);
            var road20 = builder.AddStraightRoad(startX: 24, startY: -109, hdg: 0, length: 96);
            var road10 = builder.AddStraightRoad(startX: 19, startY: -44, hdg: -1.5707963267949f, length: 60);
            var road11 = builder.Add90DegreeTurn(startX: 19, startY: -104, hdg: -1.5707963267949);
        }
    }
}
