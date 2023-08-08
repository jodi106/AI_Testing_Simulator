using System.Xml;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// This class contains methods to create beads of level 1 of an Opendrive file of the version 1.4.
    /// </summary>
    public static class XMLElementExtentions
    {

        /// <summary>
        /// This method creates the XML code of an element.
        /// </summary>
        /// <param name="parent">The parent element of the XML elements of which the XML element is a child.</param>
        /// <param name="name">The name of the XML element.</param>
        /// <returns>The created XML element.</returns>
        public static XmlElement AddElement(this XmlElement parent, string name)
        {
            var element = parent.OwnerDocument.CreateElement(name);
            parent.AppendChild(element);
            return element;
        }

        /// <summary>
        /// This method creates the XML code the header of an Opendrive file.
        /// </summary>
        /// <param name="parent">The parent element of the header element.</param>
        /// <param name="revMajor">The major revision number of Opendrive format.</param>
        /// <param name="revMinor">The minor revision number of Opendrive format.</param>
        /// <param name="name">The name of the header element.</param>
        /// <param name="version">The version of the header element.</param>
        /// <param name="north">The maximum inertial y value.</param>
        /// <param name="south">The minimum inertial y value.</param>
        /// <param name="east">The maximum inertial x value.</param>
        /// <param name="west">The minimum inertial x value.</param>
        /// <param name="vendor">The vendor name.</param>
        /// <returns>The created header element.</returns>
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

        /// <summary>
        /// This method creates the XML code of the geo reference of the header of an Opendrive file.
        /// </summary>
        /// <param name="parent">The parent element of the geo reference element.</param>
        /// <param name="geoRef">The geographic location of the Opendrive map.</param>
        /// <returns>The created geo reference element.</returns>
        public static XmlElement AddGeoReferenceElement(this XmlElement parent, string geoRef)
        {
            var geoReference = parent.OwnerDocument.CreateElement("geoReference");
            geoReference.AppendChild(parent.OwnerDocument.CreateCDataSection(geoRef));
            parent.AppendChild(geoReference);

            return geoReference;
        }

        /// <summary>
        /// This method creates the XML code the user data of an XML element. A user data element can exist on any level.
        /// </summary>
        /// <param name="parent">The parent element of the user data element.</param>
        /// <returns>The created user data element.</returns>
        public static XmlElement AddUserDataElement(this XmlElement parent)
        {
            var userData = parent.OwnerDocument.CreateElement("userData");
            parent.AppendChild(userData);

            return userData;
        }

        /// <summary>
        /// This method creates the XML code the vector scene of and OpenDrive file.
        /// </summary>
        /// <param name="parent">The parent element of the vector scene element.</param>
        /// <param name="program">The program which was used to create the XML file.</param>
        /// <param name="version">THe version of the program.</param>
        /// <returns>The created vector scene element.</returns>
        public static XmlElement AddVectorSceneElement(this XmlElement parent, string program, string version)
        {
            var vectorScene = parent.OwnerDocument.CreateElement("vectorScene");
            vectorScene.SetAttribute("program", program);
            vectorScene.SetAttribute("version", version);
            parent.AppendChild(vectorScene);

            return vectorScene;
        }

        /// <summary>
        /// This method creates the XML code of a road element in XML.
        /// </summary>
        /// <param name="parent">The parent element of the road element.</param>
        /// <param name="name">The name of the road.</param>
        /// <param name="length">The length of the road.</param>
        /// <param name="id">The id of the road.</param>
        /// <param name="junction">The id of the junction the road is a part of. If the road is not part of any junction, this
        /// value is "-1".</param>
        /// <returns>The created vector scene element.</returns>
        public static XODRRoad AddRoadElement(this XmlElement parent, string name, string length, string id, string junction)
        {
            var road = new XODRRoad(parent.OwnerDocument.CreateElement("road")); 
            
            road.SetAttribute("name", name);
            road.SetAttribute("length", length);
            road.SetAttribute("id", id);
            road.SetAttribute("junction", junction);
            parent.AppendChild(road.XmlElement);

            return road;
        }

        /// <summary>
        /// This method creates the XML code for a junction element in XML.
        /// </summary>
        /// <param name="parent">The parent element of the juction.</param>
        /// <param name="name">The name of the junction.</param>
        /// <param name="id">The id of the junction element.</param>
        /// <returns>The created junction element.</returns>
        public static XODRJunction AddJunctionElement(this XmlElement parent, string name, string id)
        {
            var junction = new XODRJunction(parent.OwnerDocument.CreateElement("junction"));

            junction.SetAttribute("name", name);
            junction.SetAttribute("id", id);
            parent.AppendChild(junction.XmlElement);

            return junction;
        }

        //public static XmlElement AddTypeElement(this XmlElement parent, string s, string type)
        //{
        //    var typeElem = parent.OwnerDocument.CreateElement("type");
        //    typeElem.SetAttribute("s", s);
        //    typeElem.SetAttribute("type", type);
        //    parent.AppendChild(typeElem);

        //    return typeElem;
        //}

        //public static XmlElement AddSpeedElement(this XmlElement parent, string max, string unit)
        //{
        //    var speed = parent.OwnerDocument.CreateElement("speed");
        //    speed.SetAttribute("max", max);
        //    speed.SetAttribute("unit", unit);
        //    parent.AppendChild(speed);

        //    return speed;
        //}

        //public static XmlElement AddElevationElement(this XmlElement parent, string s, string a, string b, string c)
        //{
        //    var elevationProfile = parent.OwnerDocument.CreateElement("elevationProfile");
        //    parent.AppendChild(elevationProfile);

        //    var elevation = parent.OwnerDocument.CreateElement("elevation");
        //    elevationProfile.AppendChild(elevation);
        //    elevation.SetAttribute("s", s);

        //    var shape = parent.OwnerDocument.CreateElement("shape");
        //    elevation.AppendChild(shape);

        //    var aNode = parent.OwnerDocument.CreateElement("a");
        //    aNode.InnerText = a;
        //    shape.AppendChild(aNode);

        //    var bNode = parent.OwnerDocument.CreateElement("b");
        //    bNode.InnerText = b;
        //    shape.AppendChild(bNode);

        //    var cNode = parent.OwnerDocument.CreateElement("c");
        //    cNode.InnerText = c;
        //    shape.AppendChild(cNode);

        //    return elevation;
        //}
    }
}