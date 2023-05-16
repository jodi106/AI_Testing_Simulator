﻿using System;
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

            //var road64 = builder.AddStraightRoad(startX:9 ,length:50, crossing:true);
            //var curve1 = builder.Add15DegreeTurn(startX: 59);
            builder.Add3wayIntersection(5, 5);
            

            builder.Document.Save("OpenDrive.xml");

            Console.WriteLine("OpenDrive.xml generated");

        }
    }
}
