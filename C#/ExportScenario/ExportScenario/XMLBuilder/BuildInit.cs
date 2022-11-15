using ExportScenario.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using ExportScenario.Entities;

namespace ExportScenario.XMLBuilder
{
    internal class BuildInit
    {
        private ScenarioInfo scenarioInfo;
        private XmlDocument root;
        private XmlNode openScenario;
        private XmlNode storyBoard;
        private XmlNode init;
        private XmlNode actions;

        public BuildInit(ScenarioInfo scenarioInfo, XmlDocument root, XmlNode storyBoard)
        /// Constructor
        {

            this.scenarioInfo = scenarioInfo;
            this.root = root;
            this.storyBoard = storyBoard;

            init = root.CreateElement("Init");
            storyBoard.AppendChild(init);
            actions = root.CreateElement("Actions");
            init.AppendChild(actions);

        }

        public void CombineInit()
        /// Combines GlobalAction and Private xml blocks 
        {

            // TODO implement InitialSpeed Attirbute for entities

            string control_mode = "carla_auto_pilot_control"; // other value: external_control 
            BuildGlobalAction(scenarioInfo.WorldOptions);

            // Spawn ego vehicle at requested coordinates
            BuildPrivate("hero", scenarioInfo.EgoVehicle.SpawnPoint, 0, true, control_mode);

            // Spawn simulation vehicles at requested coordinates
            for (int n = 0; n < scenarioInfo.Vehicles.Count; n++)
            {
                BuildPrivate("adversary" + scenarioInfo.Vehicles[n].Id, scenarioInfo.Vehicles[n].SpawnPoint, scenarioInfo.Vehicles[n].InitialSpeed);
            }

            // Spawn pedestrians at requested coordinates
            for (int n = 0; n < scenarioInfo.Pedestrians.Count; n++)
            {
                BuildPrivate("adversary_pedestrian" + scenarioInfo.Pedestrians[n].Id, scenarioInfo.Pedestrians[n].SpawnPoint, scenarioInfo.Pedestrians[n].InitialSpeed);
            }

        }

        public void BuildGlobalAction(WorldOptions worldOptions)
        /// Creates GlobalAction EnvironmentAction xml block (only Environment Action implemented)
        {
            XmlNode global_action = root.CreateElement("GlobalAction");
            BuildAction buildPublicAction = new BuildAction(root, "BuildPublic");
            buildPublicAction.EnvironmentAction(global_action, worldOptions);
            // Hierarchy
            actions.AppendChild(global_action);

        }

        public void BuildPrivate(string entityRef, Coord3D spawnPoint, double initialSpeed, bool isEgoVehicle = false, string controlMode = "simulation")
        /// Builds Private xml block
        {
            XmlNode _private = root.CreateElement("Private");
            SetAttribute("entityRef", entityRef, _private);
            XmlNode private_action1 = root.CreateElement("PrivateAction");
            XmlNode private_action2 = root.CreateElement("PrivateAction");

            BuildAction buildPrivateAction = new BuildAction(root, "BuildPrivate");
            // initial position
            buildPrivateAction.TeleportAction(private_action1, spawnPoint);
            // initial speed
            buildPrivateAction.InitialSpeedAction(private_action2, initialSpeed);

            if (isEgoVehicle)
            {
                XmlNode private_action0 = root.CreateElement("PrivateAction");
                buildPrivateAction.ControllerAction(private_action0);
                _private.AppendChild(private_action0);
            }
            // Hierarchy
            actions.AppendChild(_private);
            _private.AppendChild(private_action1);
            _private.AppendChild(private_action2);
        }

        /// helper
        private void SetAttribute(string name, string value, XmlNode element)
        {
            XmlAttribute attribute = root.CreateAttribute(name);
            attribute.Value = value;
            element.Attributes.Append(attribute);
        }
    }
}
