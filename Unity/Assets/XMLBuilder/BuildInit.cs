using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using Entity;

namespace ExportScenario.XMLBuilder
{

    /// <summary>
    /// Class to init Carla World Environment and Entities
    /// </summary>
    internal class BuildInit
    {
        private ScenarioInfo scenarioInfo;
        private XmlDocument root;
        private XmlNode openScenario;
        private XmlNode storyBoard;
        private XmlNode init;
        private XmlNode actions;

        /// <summary>
        /// Initializes a new instance of the BuildInit class to set up the Carla World Environment and Entities.
        /// </summary>
        /// <param name="scenarioInfo">The ScenarioInfo containing the necessary attributes for initializing the entities.</param>
        /// <param name="root">The XmlDocument for creating the XML structure.</param>
        /// <param name="storyBoard">The XmlNode for the StoryBoard element.</param>
        public BuildInit(ScenarioInfo scenarioInfo, XmlDocument root, XmlNode storyBoard)
        {

            this.scenarioInfo = scenarioInfo;
            this.root = root;
            this.storyBoard = storyBoard;

            init = root.CreateElement("Init");
            storyBoard.AppendChild(init);
            actions = root.CreateElement("Actions");
            init.AppendChild(actions);

        }

        /// <summary>
        /// Combines GlobalAction and Private XML blocks for the initialization of the scenario.
        /// </summary>
        public void CombineInit()
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
                if (scenarioInfo.Vehicles[n].StartPathInfo != null) initialSpeedMS = 0;
                BuildPrivate(scenarioInfo.Vehicles[n], scenarioInfo.Vehicles[n].getCarlaLocation(), initialSpeedMS);
            }

            // Spawn pedestrians at requested coordinates and speed
            for (int n = 0; n < scenarioInfo.Pedestrians.Count; n++)
            {
                double initialSpeedMS = scenarioInfo.Pedestrians[n].InitialSpeedKMH / 3.6;
                if (scenarioInfo.Pedestrians[n].StartPathInfo != null) initialSpeedMS = 0;
                BuildPrivate(scenarioInfo.Pedestrians[n], scenarioInfo.Pedestrians[n].getCarlaLocation(), initialSpeedMS);
            }
        }

        /// <summary>
        /// Creates a GlobalAction EnvironmentAction XML block (only EnvironmentAction implemented).
        /// </summary>
        /// <param name="worldOptions">The WorldOptions object containing the necessary attributes for setting the environment actions.</param>
        public void BuildGlobalAction(WorldOptions worldOptions)
        {
            XmlNode global_action = root.CreateElement("GlobalAction");
            BuildAction buildPublicAction = new BuildAction(root, "BuildPublic");
            buildPublicAction.EnvironmentAction(global_action, worldOptions);
            // Hierarchy
            actions.AppendChild(global_action);

        }
        
        /// <summary>
        /// Builds a Private XML block for a given entity, specifying its spawn position and speed.
        /// </summary>
        /// <param name="entity">The BaseEntity for which the Private XML block will be created.</param>
        /// <param name="spawnPoint">The Location object representing the spawn point of the entity.</param>
        /// <param name="initialSpeed">The initial speed of the entity in m/s.</param>
        /// <param name="isEgoVehicle">A flag to indicate if the entity is the ego vehicle. Default is false.</param>
        public void BuildPrivate(BaseEntity entity, Location spawnPoint, double initialSpeed, bool isEgoVehicle = false)
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

        /// <summary>
        /// Helper method to set an attribute for an XmlNode.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <param name="element">The XmlNode for which the attribute will be set.</param>        
        private void SetAttribute(string name, string value, XmlNode element)
        {
            XmlAttribute attribute = root.CreateAttribute(name);
            attribute.Value = value;
            element.Attributes.Append(attribute);
        }
    }
}
