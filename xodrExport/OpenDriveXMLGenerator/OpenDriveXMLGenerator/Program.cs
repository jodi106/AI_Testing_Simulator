using System;
using System.Xml;
using System.Linq;
using System.Xml.Linq;

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

            var road64 = builder.AddStraightRoad(startX:0 ,length:5, crossing:true);
            var curve1 = builder.Add15DegreeTurn(startX: 5);
            builder.Add4wayIntersection(-18, 0);
            builder.Add3wayIntersection(-36, 0);
            var curve90 = builder.Add90DegreeTurn(-14, -14);

            builder.Document.Save("OpenDrive.xml");

            Console.WriteLine("OpenDrive.xml generated");

        }
    }
}
