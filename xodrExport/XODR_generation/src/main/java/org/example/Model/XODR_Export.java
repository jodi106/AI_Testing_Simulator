package org.example.Model;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

public class XODR_Export {
    public static void main(String[] args) throws Exception {
        // Step 1
        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();

        // Step 2
        DocumentBuilder db = dbf.newDocumentBuilder();

        // Step 3
        Document doc = db.newDocument();

        // create the root element
        Element rootElement = doc.createElement("OpenDRIVE");
        doc.appendChild(rootElement);

        // create the header element
        Element header = doc.createElement("header");
        header.setAttribute("revMajor", "1");
        header.setAttribute("revMinor", "4");
        header.setAttribute("name", "");
        header.setAttribute("version", "1");
        header.setAttribute("north", "3.4040445389524979e+2");
        header.setAttribute("south", "-2.8943958368362757e+2");
        header.setAttribute("east", "3.3085867381572922e+2");
        header.setAttribute("west", "-3.1262489222340042e+2");
        header.setAttribute("vendor", "VectorZero");
        rootElement.appendChild(header);

        // create the geoReference element
        Element geoReference = doc.createElement("geoReference");
        geoReference.appendChild(doc.createCDATASection("+proj=tmerc +lat_0=0 +lon_0=0 +k=1 +x_0=0 +y_0=0 +datum=WGS84 +units=m +geoidgrids=egm96_15.gtx +vunits=m +no_defs"));
        header.appendChild(geoReference);

        // create the userData element
        Element userData = doc.createElement("userData");
        header.appendChild(userData);

        // create the vectorScene element
        Element vectorScene = doc.createElement("vectorScene");
        vectorScene.setAttribute("program", "RoadRunner");
        vectorScene.setAttribute("version", "2019.2.12 (build 5161c15)");
        userData.appendChild(vectorScene);

        // create the road element
        Element road = doc.createElement("road");
        road.setAttribute("name", "Road 64");
        road.setAttribute("length", "1.0234747062906101e+2");
        road.setAttribute("id", "64");
        road.setAttribute("junction", "-1");
        rootElement.appendChild(road);

        // create the link element
        Element link = doc.createElement("link");
        road.appendChild(link);

        // create the predecessor element
        Element predecessor = doc.createElement("predecessor");
        predecessor.setAttribute("elementType", "junction");
        predecessor.setAttribute("elementId", "455");
        link.appendChild(predecessor);

        // create the type element
        Element type = doc.createElement("type");
        type.setAttribute("s", "0.0000000000000000e+0");
        type.setAttribute("type", "town");
        road.appendChild(type);

        // create the speed element
        Element speed = doc.createElement("speed");
        speed.setAttribute("max", "55");
        speed.setAttribute("unit", "mph");
        type.appendChild(speed);

        // create the planView element
        Element planView = doc.createElement("planView");
        road.appendChild(planView);


        // create the geometry elements
        //TODO variabil in functie de drum
        Element geometry1 = doc.createElement("geometry");
        geometry1.setAttribute("s","0.0000000000000000e+0");
        geometry1.setAttribute("x","-2.3697531961010279e+2");
        geometry1.setAttribute("y","3.7372531134669975e+0");
        geometry1.setAttribute("hdg","2.9484610789761891e+0");
        geometry1.setAttribute("length","1.4798839982417746e+1");
        planView.appendChild(geometry1);

        Element arc1 = doc.createElement("arc");
        arc1.setAttribute("curvature", "-1.0668617530566759e-2");
        geometry1.appendChild(arc1);

        Element geometry2 = doc.createElement("geometry");
        geometry2.setAttribute("s","1.4798839982417746e+1");
        geometry2.setAttribute("x","-2.5121499633789063e+2");
        geometry2.setAttribute("y","7.7100000381469727e+0");
        geometry2.setAttribute("hdg","-3.4926073918718714e+0");
        geometry2.setAttribute("length","3.0246354415086046e+1");
        planView.appendChild(geometry2);

        Element arc2 = doc.createElement("arc");
        arc2.setAttribute("curvature", "-3.1406976782155697e-2");
        geometry2.appendChild(arc2);

        Element geometry3 = doc.createElement("geometry");
        geometry3.setAttribute("s","4.5045194397503792e+1");
        geometry3.setAttribute("x","-2.7095468793544137e+2");
        geometry3.setAttribute("y","2.9120906132501553e+1");
        geometry3.setAttribute("hdg","1.8406313644482548e+0");
        geometry3.setAttribute("length","5.7302276231557215e+1");
        planView.appendChild(geometry3);


        //create elevationProfile element
        Element elevationProfile = doc.createElement("elevationProfile");
        road.appendChild(elevationProfile);

        //create elevation element
        Element elevation = doc.createElement("elevation");
        elevation.setAttribute("s", "0.0000000000000000e+0");
        elevation.setAttribute("a", "0.0000000000000000e+0");
        elevation.setAttribute("b", "0.0000000000000000e+0");
        elevation.setAttribute("c", "0.0000000000000000e+0");
        elevation.setAttribute("d", "0.0000000000000000e+0");
        elevationProfile.appendChild(elevation);


        Element lateralProfile = doc.createElement("lateralProfile");
        road.appendChild(lateralProfile);

        Element superElevation = doc.createElement("superelevation");
        superElevation.setAttribute("s", "0.0000000000000000e+0");
        superElevation.setAttribute("a", "0.0000000000000000e+0");
        superElevation.setAttribute("b", "0.0000000000000000e+0");
        superElevation.setAttribute("c", "0.0000000000000000e+0");
        superElevation.setAttribute("d", "0.0000000000000000e+0");
        lateralProfile.appendChild(superElevation);

        Element lanes = doc.createElement("lanes");
        road.appendChild(lanes);

        Element laneOffset = doc.createElement("laneOffset");
        laneOffset.setAttribute("s", "0.0000000000000000e+0");
        laneOffset.setAttribute("a", "-6.6349999999999998e+0");
        laneOffset.setAttribute("b", "0.0000000000000000e+0");
        laneOffset.setAttribute("c", "0.0000000000000000e+0");
        laneOffset.setAttribute("d", "0.0000000000000000e+0");
        lanes.appendChild(laneOffset);


        Element laneSection = doc.createElement("laneSection");
        laneSection.setAttribute("s", "0.0000000000000000e+0");
        lanes.appendChild(laneSection);

        Element left = doc.createElement("left");
        laneSection.appendChild(left);

        //Lane 8
        Element lane8 = doc.createElement("lane");
        lane8.setAttribute("id", "6");
        lane8.setAttribute("type", "sidewalk");
        lane8.setAttribute("level", "false");
        left.appendChild(lane8);

        Element width8 = doc.createElement("width");
        width8.setAttribute("sOffset", "0.0000000000000000e+0");
        width8.setAttribute("a", "2.0000000000000000e+0");
        width8.setAttribute("b", "0.0000000000000000e+0");
        width8.setAttribute("c", "0.0000000000000000e+0");
        width8.setAttribute("d", "0.0000000000000000e+0");
        lane8.appendChild(width8);

        Element roadMark8 = doc.createElement("roadMark");
        roadMark8.setAttribute("sOffset", "0.0000000000000000e+0");
        roadMark8.setAttribute("type", "none");
        roadMark8.setAttribute("material", "standard");
        roadMark8.setAttribute("color", "white");
        roadMark8.setAttribute("laneChange", "none");
        lane8.appendChild(roadMark8);

        Element userData8 = doc.createElement("userData");
        lane8.appendChild(userData8);

        Element vectorLane8 = doc.createElement("vectorLane");
        vectorLane8.setAttribute("sOffset", "0.0000000000000000e+0");
        vectorLane8.setAttribute("laneId", "{96760f1b-54ba-41ff-b639-fe5dcfcb1f99}");
        vectorLane8.setAttribute("travelDir", "undirected");
        userData8.appendChild(vectorLane8);


        //Lane7
        Element lane7 = doc.createElement("lane");
        lane7.setAttribute("id", "5");
        lane7.setAttribute("type", "shoulder");
        lane7.setAttribute("level", "false");
        left.appendChild(lane7);

        Element width7 = doc.createElement("width");
        width7.setAttribute("sOffset", "0.0000000000000000e+0");
        width7.setAttribute("a", "6.3499999999999979e-1");
        width7.setAttribute("b", "0.0000000000000000e+0");
        width7.setAttribute("c", "0.0000000000000000e+0");
        width7.setAttribute("d", "0.0000000000000000e+0");
        lane7.appendChild(width7);

        Element roadMark7 = doc.createElement("roadMark");
        roadMark7.setAttribute("sOffset", "0.0000000000000000e+0");
        roadMark7.setAttribute("type", "none");
        roadMark7.setAttribute("material", "standard");
        roadMark7.setAttribute("color", "white");
        roadMark7.setAttribute("laneChange", "none");
        lane7.appendChild(roadMark7);

        Element userData7 = doc.createElement("userData");
        lane7.appendChild(userData7);

        Element vectorLane7 = doc.createElement("vectorLane");
        vectorLane7.setAttribute("sOffset", "0.0000000000000000e+0");
        vectorLane7.setAttribute("laneId", "{0ee4d9bb-303d-4aa8-88d1-f95cbbdd4141}");
        vectorLane7.setAttribute("travelDir", "undirected");
        userData7.appendChild(vectorLane7);

        //Lane 5
        Element lane5 = doc.createElement("lane");
        lane5.setAttribute("id", "4");
        lane5.setAttribute("type", "none");
        lane5.setAttribute("level", "false");
        left.appendChild(lane5);

        Element width5 = doc.createElement("width");
        width5.setAttribute("sOffset", "0.0000000000000000e+0");
        width5.setAttribute("a", "3.5000000000000000e+0");
        width5.setAttribute("b", "0.0000000000000000e+0");
        width5.setAttribute("c", "0.0000000000000000e+0");
        width5.setAttribute("d", "0.0000000000000000e+0");
        lane5.appendChild(width5);

        Element roadMark5 = doc.createElement("roadMark");
        roadMark5.setAttribute("sOffset", "0.0000000000000000e+0");
        roadMark5.setAttribute("type", "solid");
        roadMark5.setAttribute("material", "standard");
        roadMark5.setAttribute("color", "white");
        roadMark5.setAttribute("width", "1.2500000000000000e-1");
        roadMark5.setAttribute("laneChange", "none");
        lane5.appendChild(roadMark5);

        Element userData5 = doc.createElement("userData");
        lane5.appendChild(userData5);

        Element vectorLane5 = doc. createElement("vertorLane");
        vectorLane5.setAttribute("sOffset", "0.0000000000000000e+0");
        vectorLane5.setAttribute("laneId", "{758941ca-5a3a-47bd-9936-765d67ac4a00}");
        vectorLane5.setAttribute("travelDir", "undirected");
        userData5.appendChild(vectorLane5);


        //Lane4
        Element lane4 = doc.createElement("lane");
        lane4.setAttribute("id", "3");
        lane4.setAttribute("type", "none");
        lane4.setAttribute("level", "false");
        left.appendChild(lane4);

        Element width4 = doc.createElement("width");
        width4.setAttribute("sOffset", "0.0000000000000000e+0");
        width4.setAttribute("a", "3.5000000000000000e+0");
        width4.setAttribute("b", "0.0000000000000000e+0");
        width4.setAttribute("c", "0.0000000000000000e+0");
        width4.setAttribute("d", "0.0000000000000000e+0");
        lane4.appendChild(width4);

        Element roadMark4 = doc.createElement("roadMark");
        roadMark4.setAttribute("sOffset", "0.0000000000000000e+0");
        roadMark4.setAttribute("type", "solid");
        roadMark4.setAttribute("material", "standard");
        roadMark4.setAttribute("color", "white");
        roadMark4.setAttribute("laneChange", "none");
        lane4.appendChild(roadMark4);

        Element userData4 = doc.createElement("userData");
        lane4.appendChild(userData4);

        Element vectorLane4 = doc. createElement("vertorLane");
        vectorLane4.setAttribute("sOffset", "0.0000000000000000e+0");
        vectorLane4.setAttribute("laneId", "{f8886eb6-dcce-45f7-a4d2-30e0a61041cf}");
        vectorLane4.setAttribute("travelDir", "undirected");
        userData4.appendChild(vectorLane4);


        //Lane 2
        Element lane2 = doc.createElement("lane");
        lane2.setAttribute("id", "2");
        lane2.setAttribute("type", "shoulder");
        lane2.setAttribute("level", "false");
        left.appendChild(lane2);

        Element width2 = doc.createElement("width");
        width2.setAttribute("sOffset", "0.0000000000000000e+0");
        width2.setAttribute("a", "6.3499999999999979e-1");
        width2.setAttribute("b", "0.0000000000000000e+0");
        width2.setAttribute("c", "0.0000000000000000e+0");
        width2.setAttribute("d", "0.0000000000000000e+0");
        lane2.appendChild(width2);

        Element roadMark2 = doc.createElement("roadMark");
        roadMark2.setAttribute("sOffset", "0.0000000000000000e+0");
        roadMark2.setAttribute("type", "none");
        roadMark2.setAttribute("material", "standard");
        roadMark2.setAttribute("color", "white");
        roadMark2.setAttribute("laneChange", "none");
        lane2.appendChild(roadMark2);

        Element userData2 = doc.createElement("userData");
        lane2.appendChild(userData2);

        Element vectorLane2 = doc.createElement("vectorLane");
        vectorLane2.setAttribute("sOffset", "0.0000000000000000e+0");
        vectorLane2.setAttribute("laneId", "{743bf693-77d2-47fb-ae6d-0eff49204339}");
        vectorLane2.setAttribute("travelDir", "undirected");
        userData2.appendChild(vectorLane2);

        //Lane 1
        Element lane1 = doc.createElement("lane");
        lane1.setAttribute("id", "1");
        lane1.setAttribute("type", "sidewalk");
        lane1.setAttribute("level", "false");
        left.appendChild(lane1);

        Element width1 = doc.createElement("width");
        width1.setAttribute("sOffset", "0.0000000000000000e+0");
        width1.setAttribute("a", "2.0000000000000000e+0");
        width1.setAttribute("b", "0.0000000000000000e+0");
        width1.setAttribute("c", "0.0000000000000000e+0");
        width1.setAttribute("d", "0.0000000000000000e+0");
        lane1.appendChild(width1);

        Element roadMark1 = doc.createElement("roadMark");
        roadMark1.setAttribute("s", "0.0000000000000000e+0");
        roadMark1.setAttribute("type", "curb");
        roadMark1.setAttribute("material", "standard");
        roadMark1.setAttribute("color", "white");
        roadMark1.setAttribute("width", "1.5239999999999998e-1");
        roadMark1.setAttribute("laneChange", "none");
        lane1.appendChild(roadMark1);

        Element userData1 = doc.createElement("userData");
        lane1.appendChild(userData1);

        Element vectorLane1 = doc. createElement("vertorLane");
        vectorLane1.setAttribute("sOffset", "0.0000000000000000e+0");
        vectorLane1.setAttribute("laneId", "{280e09ee-dc26-4199-973f-8eca847a8ad4}");
        vectorLane1.setAttribute("travelDir", "undirected");
        userData1.appendChild(vectorLane1);


        //Center
        Element center = doc.createElement("center");
        laneSection.appendChild(center);

        Element lane = doc.createElement("lane");
        lane.setAttribute("id", "0");
        lane.setAttribute("type", "none");
        lane.setAttribute("level", "false");
        center.appendChild(lane);

        Element roadMark = doc.createElement("roadMark");
        roadMark.setAttribute("sOffset", "0.0000000000000000e+0");
        roadMark.setAttribute("type", "none");
        roadMark.setAttribute("material", "standard");
        roadMark.setAttribute("color", "white");
        roadMark.setAttribute("laneChange", "none");
        lane.appendChild(roadMark);


        // Step 6
        TransformerFactory tf = TransformerFactory.newInstance();
        Transformer transformer = tf.newTransformer();
        transformer.setOutputProperty(OutputKeys.INDENT, "yes");

        // Step 7
        DOMSource source = new DOMSource(doc);
        StreamResult result = new StreamResult("road.xml");
        transformer.transform(source, result);


    }
}