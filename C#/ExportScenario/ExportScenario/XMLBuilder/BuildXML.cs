using ExportScenario.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace ExportScenario.XMLBuilder
{
    public class BuildXML
    {

        private XmlDocument root;
        private XmlNode openScenario;
        private ScenarioInfo scenarioInfo; // Currently initialised in BuildXML

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

            BuildInit init = new BuildInit(scenarioInfo, root, openScenario);
            init.CombineInit();

            ExportXML(scenarioInfo.Name);
        }

        public void ExportXML(string scenario_name = "MyScenario")
        /// Exports the finished OpenScenario file to defined path
        {
            root.Save("..\\..\\..\\" + scenario_name + ".xosc");
            root.Save(Console.Out);
        }

        private void BuildFirstOpenScenarioElements(string scenario_name = "MyScenario", string map = "Town04") // you can rename this method
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

        }

        public void BuildStories()
        /// Creates Stories from story head and Events
        {

        }

        public void BuildEvents()
        /// Creates Events by combining Actions and Triggers and combines them to one XML Block
        {

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


