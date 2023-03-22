using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using Entity;

namespace ExportScenario.XMLBuilder
{
    internal class BuildInit
    /// <summary>Class to init Carla World Environment and Entities</summary>
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
        /// Combines GlobalAction and Private xml blocks .
        {
            BuildGlobalAction(scenarioInfo.WorldOptions);

            // Spawn ego vehicle at requested coordinates and speed
            double initialSpeedMS_ego = (scenarioInfo.EgoVehicle.InitialSpeedKMH / 3.6); // convert km/h in m/s for Carla
            if (scenarioInfo.EgoVehicle.InitialSpeedKMH <= 0) initialSpeedMS_ego = 0.1; 
            BuildPrivate(scenarioInfo.EgoVehicle, scenarioInfo.EgoVehicle.getCarlaLocation(), initialSpeedMS_ego, true);

            // Spawn simulation vehicles at requested coordinates and speed
            for (int n = 0; n < scenarioInfo.Vehicles.Count; n++)
            {
                double initialSpeedMS = scenarioInfo.Vehicles[n].InitialSpeedKMH / 3.6;
                if (scenarioInfo.Vehicles[n].StartRouteInfo != null) initialSpeedMS = 0;
                BuildPrivate(scenarioInfo.Vehicles[n], scenarioInfo.Vehicles[n].getCarlaLocation(), initialSpeedMS);
            }

            // Spawn pedestrians at requested coordinates and speed
            for (int n = 0; n < scenarioInfo.Pedestrians.Count; n++)
            {
                BuildPrivate(scenarioInfo.Pedestrians[n], scenarioInfo.Pedestrians[n].getCarlaLocation(), scenarioInfo.Pedestrians[n].InitialSpeedKMH / 3.6);
            }
        }

        public void BuildGlobalAction(WorldOptions worldOptions)
        /// Creates GlobalAction EnvironmentAction xml block (only EnvironmentAction implemented).
        {
            XmlNode global_action = root.CreateElement("GlobalAction");
            BuildAction buildPublicAction = new BuildAction(root, "BuildPublic");
            buildPublicAction.EnvironmentAction(global_action, worldOptions);
            // Hierarchy
            actions.AppendChild(global_action);

        }

        public void BuildPrivate(BaseEntity entity, Location spawnPoint, double initialSpeed, bool isEgoVehicle = false)
        /// Builds Private xml block. Specifies Spawnpostition and speed for scenario entities.
        {
            string entityRef = entity.Id;
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
                Ego ego = (Ego)entity;
                XmlNode private_action0 = root.CreateElement("PrivateAction");
                buildPrivateAction.ControllerAction(private_action0, ego.Agent);
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
