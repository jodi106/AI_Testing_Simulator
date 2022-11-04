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
        private XmlNode init;
        private XmlNode actions;

        public BuildInit(ScenarioInfo scenarioInfo, XmlDocument root, XmlNode openScenario)
        /// Constructor
        {

            this.scenarioInfo = scenarioInfo;
            this.root = root;
            this.openScenario = openScenario;

            init = root.CreateElement("Init");
            openScenario.AppendChild(init);
            actions = root.CreateElement("Actions");
            init.AppendChild(actions);

        }

        public void CombineInit()
        /// Combines GlobalAction and Private xml blocks 
        {
            /*
            int number_of_simulation_cars = 3;
            int number_of_pedestrians = 2;
            string[] x = { "255.7", "290", "255", "255" }; // index 0 is ego, index 1+ is simulation vehicle position
            string[] y = { "-145.7", "-172", "-190", "-210" };
            string[] z = { "0.3", "0.3", "0.3", "0.3" };
            string[] h = { "200", "180", "90", "90" };
            string[] x_ped = { "265", "266" };
            string[] y_ped = { "-165.1", "-164" };
            string[] z_ped = { "0.3", "0.3" };
            string[] h_ped = { "200", "90" };
            */

            double rotation = 180.0; // TODO either expand SpawnPoint to 4 values or add rotation attribute to vehicles
            string control_mode = "carla_auto_pilot_control"; // other value: external_control 
            BuildGlobalAction(scenarioInfo.WorldOptions);

            // Spawn ego vehicle at requested coordinates
            BuildPrivate("hero", scenarioInfo.EgoVehicle.SpawnPoint, rotation, true, control_mode);

            // Spawn simulation vehicles at requested coordinates
            for (int n = 0; n < scenarioInfo.Vehicles.Count; n++)
            {
                BuildPrivate("adversary" + n.ToString(), scenarioInfo.Vehicles[n].SpawnPoint, rotation);
            }

            // Spawn pedestrians at requested coordinates
            for (int n = 0; n < scenarioInfo.Pedestrians.Count; n++)
            {
                BuildPrivate("adversary_pedestrian" + n.ToString(), scenarioInfo.Pedestrians[n].SpawnPoint, rotation);
            }

        }

        public void BuildGlobalAction(WorldOptions worldOptions)
        /// Creates GlobalAction EnvironmentAction xml block (only Environment Action implemented)
        {
            // TODO Directly use worldOptions attributes in XML Nodes
            string cloudState = worldOptions.CloudState; // possible values: cloudy, free, overcast, rainy
            double sunIntensity = worldOptions.SunIntensity; // Illuminance of the sun, direct sunlight is around 100,00 lx. Unit: lux; Range: [0..inf[.
            string precipitationType = worldOptions.PrecipitationTypes; // possible values: dry, rain, snow
            double precipitationIntensity = worldOptions.PrecipitationIntensity; // 0.0 no rain
            string dateTime = worldOptions.Date_Time; // Format: "2019-06-25T12:00:00"
            double sunAzimuth = worldOptions.SunAzimuth; // Azimuth of the sun, counted counterclockwise, 0=north, PI/2 = east, PI=south, 3/2 PI=west. Unit: radian; Range: [0..2PI].
            double sunElevation = worldOptions.SunElevation; // Solar elevation angle, 0=x/y plane, PI/2=zenith. Unit: rad; Range: [-PI..PI].
            double fogVisualRange = worldOptions.FogVisualRange; // Unit: m; Range: [0..inf[.
            double frictionScaleFactor = worldOptions.FrictionScaleFactor; // Friction scale factor. Range: [0..inf[

            XmlNode global_action = root.CreateElement("GlobalAction");
            XmlNode environment_action = root.CreateElement("EnvironmentAction");
            XmlNode environment = root.CreateElement("Environment");
            SetAttribute("name", "Environment1", environment);
            XmlNode time_of_day = root.CreateElement("TimeOfDay");
            SetAttribute("animation", "false", time_of_day);
            SetAttribute("dateTime", dateTime, time_of_day);
            XmlNode weather = root.CreateElement("Weather");
            SetAttribute("cloudState", cloudState, weather);
            XmlNode sun = root.CreateElement("Sun");
            SetAttribute("intensity", sunIntensity.ToString(), sun);
            SetAttribute("azimuth", sunAzimuth.ToString(), sun);
            SetAttribute("elevation", sunElevation.ToString(), sun);
            XmlNode fog = root.CreateElement("Fog");
            SetAttribute("visualRange", fogVisualRange.ToString(), fog);
            XmlNode precipitation = root.CreateElement("Precipitation");
            SetAttribute("precipitationType", precipitationType, precipitation);
            SetAttribute("intensity", precipitationIntensity.ToString(), precipitation);
            XmlNode road_condition = root.CreateElement("RoadCondition");
            SetAttribute("frictionScaleFactor", frictionScaleFactor.ToString(), road_condition);

            // Hierarchy
            actions.AppendChild(global_action);
            global_action.AppendChild(environment_action);
            environment_action.AppendChild(environment);
            environment.AppendChild(time_of_day);
            environment.AppendChild(weather);
            weather.AppendChild(sun);
            weather.AppendChild(fog);
            weather.AppendChild(precipitation);
            environment.AppendChild(road_condition);
        }

        public void BuildPrivate(string entityRef, Coord3D spawnPoint, double rotation, bool isEgoVehicle = false, string controlMode = "simulation")
        /// Builds Private xml block
        {
            // initial position
            XmlNode _private = root.CreateElement("Private");
            SetAttribute("entityRef", entityRef, _private);
            XmlNode private_action1 = root.CreateElement("PrivateAction");
            XmlNode teleport_action = root.CreateElement("TeleportAction");
            XmlNode position = root.CreateElement("Position");
            XmlNode world_position = root.CreateElement("WorldPosition");

            SetAttribute("x", spawnPoint.X.ToString(), world_position);
            SetAttribute("y", spawnPoint.Y.ToString(), world_position);
            SetAttribute("z", spawnPoint.Z.ToString(), world_position);
            SetAttribute("h", rotation.ToString(), world_position);
            /*
            SetAttribute("x", x, world_position);
            SetAttribute("y", y, world_position);
            SetAttribute("z", z, world_position);
            SetAttribute("h", h, world_position);
            */

            // initial speed
            // TODO implement speed action method instead of hard coding
            XmlNode private_action2 = root.CreateElement("PrivateAction");
            XmlNode speed_action = root.CreateElement("SpeedAction");
            XmlNode SpeedActionDynamics = root.CreateElement("SpeedActionDynamics");
            XmlNode SpeedActionTarget = root.CreateElement("SpeedActionTarget");
            XmlNode AbsoluteTargetSpeed = root.CreateElement("AbsoluteTargetSpeed");

            SetAttribute("dynamicsShape", "step", SpeedActionDynamics);
            SetAttribute("value", "0", SpeedActionDynamics);
            SetAttribute("dynamicsDimension", "time", SpeedActionDynamics);

            SetAttribute("value", "20.0", AbsoluteTargetSpeed);

            if (isEgoVehicle)
            {
                XmlNode private_action0 = root.CreateElement("PrivateAction");
                XmlNode controller_action = root.CreateElement("ControllerAction");
                XmlNode assign_controller_action = root.CreateElement("AssignControllerAction");
                XmlNode controller = root.CreateElement("Controller");
                SetAttribute("name", "HeroAgent", controller);
                XmlNode properties = root.CreateElement("Properties");
                XmlNode property = root.CreateElement("Property");
                SetAttribute("name", "module", property);
                SetAttribute("value", controlMode, property);
                XmlNode override_controller_value_action = root.CreateElement("OverrideControllerValueAction");
                XmlNode throttle = root.CreateElement("Throttle");
                SetAttribute("value", "0", throttle);
                SetAttribute("active", "false", throttle);
                XmlNode brake = root.CreateElement("Brake");
                SetAttribute("value", "0", brake);
                SetAttribute("active", "false", brake);
                XmlNode clutch = root.CreateElement("Clutch");
                SetAttribute("value", "0", clutch);
                SetAttribute("active", "false", clutch);
                XmlNode parking_brake = root.CreateElement("ParkingBrake");
                SetAttribute("value", "0", parking_brake);
                SetAttribute("active", "false", parking_brake);
                XmlNode steering_wheel = root.CreateElement("SteeringWheel");
                SetAttribute("value", "0", steering_wheel);
                SetAttribute("active", "false", steering_wheel);
                XmlNode gear = root.CreateElement("Gear");
                SetAttribute("number", "0", gear);
                SetAttribute("active", "false", gear);

                // Hierarchy
                _private.AppendChild(private_action0);
                private_action0.AppendChild(controller_action);
                controller_action.AppendChild(assign_controller_action);
                assign_controller_action.AppendChild(controller);
                controller.AppendChild(properties);
                properties.AppendChild(property);
                controller_action.AppendChild(override_controller_value_action);
                override_controller_value_action.AppendChild(throttle);
                override_controller_value_action.AppendChild(brake);
                override_controller_value_action.AppendChild(clutch);
                override_controller_value_action.AppendChild(parking_brake);
                override_controller_value_action.AppendChild(steering_wheel);
                override_controller_value_action.AppendChild(gear);
            }

            // Hierarchy
            actions.AppendChild(_private);
            _private.AppendChild(private_action1);
            private_action1.AppendChild(teleport_action);
            teleport_action.AppendChild(position);
            position.AppendChild(world_position);

            _private.AppendChild(private_action2);
            private_action2.AppendChild(speed_action);
            speed_action.AppendChild(SpeedActionDynamics);
            speed_action.AppendChild(SpeedActionTarget);
            SpeedActionTarget.AppendChild(AbsoluteTargetSpeed);

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
