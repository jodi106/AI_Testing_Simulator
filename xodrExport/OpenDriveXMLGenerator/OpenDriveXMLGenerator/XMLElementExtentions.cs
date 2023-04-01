using System.Xml;

namespace OpenDriveXMLGenerator
{
    public static class XMLElementExtentions
    {
        public static XmlElement AddElement(this XmlElement parent, string name)
        {
            var element = parent.OwnerDocument.CreateElement(name);
            parent.AppendChild(element);
            return element;
        }

        public static XmlElement AddHeaderElement(this XmlElement parent, string revMajor, string revMinor, string name, string version,
           string north, string south, string east, string west, string vendor)
        {
            var header = parent.OwnerDocument.CreateElement("header");
            header.SetAttribute("revMajor", revMajor);
            header.SetAttribute("revMinor", revMinor);
            header.SetAttribute("name", name);
            header.SetAttribute("version", version);
            header.SetAttribute("north", north);
            header.SetAttribute("south", south);
            header.SetAttribute("east", east);
            header.SetAttribute("west", west);
            header.SetAttribute("vendor", vendor);

            parent.AppendChild(header);

            return header;
        }

        public static XmlElement AddGeoReferenceElement(this XmlElement parent, string geoRef)
        {
            var geoReference = parent.OwnerDocument.CreateElement("geoReference");
            geoReference.AppendChild(parent.OwnerDocument.CreateCDataSection(geoRef));
            parent.AppendChild(geoReference);

            return geoReference;
        }

        public static XmlElement AddUserDataElement(this XmlElement parent)
        {
            var userData = parent.OwnerDocument.CreateElement("userData");
            parent.AppendChild(userData);

            return userData;
        }

        public static XmlElement AddVectorSceneElement(this XmlElement parent, string program, string version)
        {
            var vectorScene = parent.OwnerDocument.CreateElement("vectorScene");
            vectorScene.SetAttribute("program", program);
            vectorScene.SetAttribute("version", version);
            parent.AppendChild(vectorScene);

            return vectorScene;
        }

        public static XmlElement AddRoadElement(this XmlElement parent, string name, string length, string id, string junction)
        {
            var road = parent.OwnerDocument.CreateElement("road");
            road.SetAttribute("name", name);
            road.SetAttribute("length", length);
            road.SetAttribute("id", id);
            road.SetAttribute("junction", junction);
            parent.AppendChild(road);

            return road;
        }

        public static XmlElement AddLinkElement(this XmlElement parent, string elementType, string elementId)
        {
            var link = parent.OwnerDocument.CreateElement("link");
            var predecessor = parent.OwnerDocument.CreateElement("predecessor");
            predecessor.SetAttribute("elementType", elementType);
            predecessor.SetAttribute("elementId", elementId);
            link.AppendChild(predecessor);
            parent.AppendChild(link);

            return link;
        }

        public static XmlElement AddTypeElement(this XmlElement parent, string s, string type)
        {
            var typeElem = parent.OwnerDocument.CreateElement("type");
            typeElem.SetAttribute("s", s);
            typeElem.SetAttribute("type", type);
            parent.AppendChild(typeElem);

            return typeElem;
        }

        public static XmlElement AddSpeedElement(this XmlElement parent, string max, string unit)
        {
            var speed = parent.OwnerDocument.CreateElement("speed");
            speed.SetAttribute("max", max);
            speed.SetAttribute("unit", unit);
            parent.AppendChild(speed);

            return speed;
        }

        public static XmlElement AddPlainViewElement(this XmlElement parent)
        {
            var planView = parent.OwnerDocument.CreateElement("planView");
            parent.AppendChild(planView);

            return planView;
        }

        public static XmlElement AddGeometryElement(this XmlElement parent, string s, string x, string y, string hdg, string length, string curvature)
        {
            var geometry = parent.OwnerDocument.CreateElement("geometry");
            geometry.SetAttribute("s", s);
            geometry.SetAttribute("x", x);
            geometry.SetAttribute("y", y);
            geometry.SetAttribute("hdg", hdg);
            geometry.SetAttribute("length", length);
            parent.AppendChild(geometry);

            if (curvature != null)
            {
                var arc = parent.OwnerDocument.CreateElement("arc");
                arc.SetAttribute("curvature", curvature);
                geometry.AppendChild(arc);
            }

            return geometry;
        }

        public static XmlElement AddElevationElement(this XmlElement parent, string s, string a, string b, string c)
        {
            var elevationProfile = parent.OwnerDocument.CreateElement("elevationProfile");
            parent.AppendChild(elevationProfile);

            var elevation = parent.OwnerDocument.CreateElement("elevation");
            elevationProfile.AppendChild(elevation);
            elevation.SetAttribute("s", s);

            var shape = parent.OwnerDocument.CreateElement("shape");
            elevation.AppendChild(shape);

            var aNode = parent.OwnerDocument.CreateElement("a");
            aNode.InnerText = a;
            shape.AppendChild(aNode);

            var bNode = parent.OwnerDocument.CreateElement("b");
            bNode.InnerText = b;
            shape.AppendChild(bNode);

            var cNode = parent.OwnerDocument.CreateElement("c");
            cNode.InnerText = c;
            shape.AppendChild(cNode);

            return elevation;
        }
    }
}