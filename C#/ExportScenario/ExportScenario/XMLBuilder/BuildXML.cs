using ExportScenario.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;

namespace ExportScenario.XMLBuilder
{
    public class BuildXML
    {

        private XmlDocument root;
        private XmlNode openScenario;
        private ScenarioInfo scenarioInfo;
        private XmlNode storyBoard;

        public BuildXML(ScenarioInfo scenarioInfo)
        /// Constructor to initializes BuildXML object with head section
        {
            Console.WriteLine("Hello");
            this.scenarioInfo = scenarioInfo;

            root = new XmlDocument();
            XmlNode xmlVersion = root.CreateXmlDeclaration("1.0", null, null);
            root.AppendChild(xmlVersion);
            openScenario = root.CreateElement("OpenSCENARIO");
            root.AppendChild(openScenario);


        }

        public void CombineXML()
        /// Combines all xml blocks 
        {
            BuildFirstOpenScenarioElements(scenarioInfo.Name, scenarioInfo.MapURL);

            BuildEntities entities = new BuildEntities(scenarioInfo, root, openScenario);
            entities.CombineEntities();
            BuildStoryBoard();

            ExportXML(scenarioInfo.Name);
        }

        public void ExportXML(string scenario_name = "MyScenario")
        /// Exports the finished OpenScenario file to defined path
        {
            root.Save("..\\..\\..\\" + scenario_name + ".xosc");
            root.Save(Console.Out);
        }

        private void BuildFirstOpenScenarioElements(string scenario_name = "MyScenario", string map = "Town04") // you can rename this method
        /// Creates first ScenarioElements
        {
            // TODO Variables that need to be inside ScenarioInfo class TODO
            string dateTime = "2022-09-24T12:00:00"; // TODO create datetime string of current time

            // add elements
            XmlNode file_header = root.CreateElement("FileHeader");
            SetAttribute("revMajor", "1", file_header);
            SetAttribute("revMinor", "0", file_header);
            SetAttribute("date", dateTime, file_header);
            SetAttribute("description", "CARLA:" + scenario_name, file_header);
            SetAttribute("author", "ScenarioBuilderTM", file_header);
            XmlNode parameter_declarations = root.CreateElement("ParameterDeclarations");
            XmlNode catalog_locations = root.CreateElement("CatalogLocations");
            XmlNode road_network = root.CreateElement("RoadNetwork");
            XmlNode logic_file = root.CreateElement("LogicFile");
            SetAttribute("filepath", map, logic_file);
            XmlNode scene_graph_file = root.CreateElement("SceneGraphFile");
            SetAttribute("filepath", "", scene_graph_file);

            // hierarchy
            openScenario.AppendChild(file_header);
            openScenario.AppendChild(parameter_declarations);
            openScenario.AppendChild(catalog_locations);
            openScenario.AppendChild(road_network);
            road_network.AppendChild(logic_file);
            road_network.AppendChild(scene_graph_file);
        }

        public void BuildStoryBoard()
        /// Combines Init block and Story blocks
        {
            storyBoard = root.CreateElement("StoryBoard");
            openScenario.AppendChild(storyBoard);
            BuildInit init = new BuildInit(scenarioInfo, root, storyBoard);
            init.CombineInit();

            for (int i = 0; i < scenarioInfo.Vehicles.Count; i++)
            {
                BuildStories(scenarioInfo.Vehicles[i]);
            }


        }

        public void BuildStories(Vehicle vehicle)//, Pedestrian pedestrian)
        /// Creates Stories from story head and Events
        {


            // ToDo implement either new BuildStories function for pedestrians or create variable code below

            XmlNode story = root.CreateElement("Story");
            SetAttribute("name", "Adversary" + vehicle.Id + "_Story", story);
            XmlNode act = root.CreateElement("Act");
            SetAttribute("name", "Adversary" + vehicle.Id + "_Act", act);
            XmlNode maneuverGroup = root.CreateElement("ManeuverGroup");
            SetAttribute("maximumExecutionCount", "1", maneuverGroup);
            SetAttribute("name", "Adversary" + vehicle.Id + "Sequence", maneuverGroup);
            XmlNode actors = root.CreateElement("Actors");
            SetAttribute("selectTriggeringEntities", "false", actors);
            XmlNode entityRef = root.CreateElement("EntityRef");
            SetAttribute("entityRef", "adversary" + vehicle.Id, entityRef);
            XmlNode maneuver = root.CreateElement("Maneuver");
            SetAttribute("name", "Adversary" + vehicle.Id + "_Maneuver", maneuverGroup);


            // ToDo add EventList entries to ScenarioInfoExample

            /*
            for (int i = 0; i < waypoint.ActionTypeInfo.Positions.Count; i++)
            for (int i = 0; i < vehicle.Path.EventList.Count; i++)
            {
                BuildEvents(vehicle.Path.EventList[i]);
            }
            */

            // hierarchy
            storyBoard.AppendChild(story);
            story.AppendChild(act);
            act.AppendChild(maneuverGroup);
            maneuverGroup.AppendChild(actors);
            actors.AppendChild(entityRef);
            maneuverGroup.AppendChild(maneuver);


            // ToDo implement using StopTrigger from BuildTrigger.
            XmlNode stopTrigger = root.CreateElement("StopTrigger");
            storyBoard.AppendChild(stopTrigger);
        }

        public void BuildEvents(Waypoint waypoint)
        /// Creates Events by combining Actions and Triggers and combines them to one XML Block
        {
            XmlNode new_event = root.CreateElement("Event");
            SetAttribute("name", waypoint.ActionTypeInfo.Name + waypoint.Id, new_event);
            SetAttribute("priority", waypoint.Priority, new_event);
            XmlNode action = root.CreateElement("Action");
            SetAttribute("name", waypoint.ActionTypeInfo.Name + waypoint.Id, action);
            

            // ToDo implement Event and Trigger building
            BuildAction buildAction = new BuildAction(root, "buildAction");

            //Get the method information using the method info class
            MethodInfo mi = this.GetType().GetMethod(waypoint.ActionTypeInfo.Name);
            
            mi.Invoke(buildAction, new object[] { action, waypoint });


            new_event.AppendChild(action);

            // ToDo implement Trigger building
            BuildTrigger buildTrigger = new BuildTrigger(root, scenarioInfo);
            //buildTrigger.CombineTrigger(true, new_event, waypoint.Trigger_Info.TriggerType);
            
            
            

        }

        

        // Helper
        private void SetAttribute(string name, string value, XmlNode element)
        {
            XmlAttribute attribute = root.CreateAttribute(name);
            attribute.Value = value;
            element.Attributes.Append(attribute);
        }
    }
}


