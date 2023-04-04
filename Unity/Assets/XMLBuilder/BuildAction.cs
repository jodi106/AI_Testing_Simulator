using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Entity;

namespace ExportScenario.XMLBuilder
{
    internal class BuildAction
    /// <summary>Class to create OpenScenario Actions for Stories and Init.</summary>
    {
        private XmlDocument root;
        public string name { get; set; }
        public BuildAction(XmlDocument root, string name)
        // Constructor
        {
            this.root = root;
            this.name = name;
        }
        public void CombineAction() { }
        /// ---Probably not necessary as Action only has one xmlBlock---

        //---------------------------------- StoryActions -----------------------------------------
        // Input always has to be XmlNode action, Waypoint waypoint
        public void AcquirePositionAction(XmlNode action, ActionType actionType)
        /// Creates AcquirePositionAction. Defines specific position to go to for a scenario entity (Ego vehicle).
        /// Invoked in BuildXML.cs BuildEvent()
        {
            XmlNode privateAction = root.CreateElement("PrivateAction");
            XmlNode routingAction = root.CreateElement("RoutingAction");
            XmlNode acquirePositionAction = root.CreateElement("AcquirePositionAction");
            XmlNode position = root.CreateElement("Position");
            XmlNode worldPosition = root.CreateElement("WorldPosition");
            SetAttribute("x", actionType.PositionsCarla[0].Vector3Ser.ToVector3().x.ToString(), worldPosition);
            SetAttribute("y", actionType.PositionsCarla[0].Vector3Ser.ToVector3().y.ToString(), worldPosition);
            SetAttribute("z", actionType.PositionsCarla[0].Vector3Ser.ToVector3().z.ToString(), worldPosition);
            SetAttribute("h", actionType.PositionsCarla[0].Rot.ToString(), worldPosition);

            // Hierarchy
            action.AppendChild(privateAction);
            privateAction.AppendChild(routingAction);
            routingAction.AppendChild(acquirePositionAction);
            acquirePositionAction.AppendChild(position);
            position.AppendChild(worldPosition);
        }

        // Invoked in BuildXML.cs Method BuildEvents
        public void AssignRouteAction(XmlNode action, ActionType actionType)
        /// Creates AssignRouteAction. Defines entire route with multiple postisions for a scenario entity.
        {
            // TODO routeStrategy is 'fastest' for vehicles and 'shortest' for pedestrians

            XmlNode privateAction = root.CreateElement("PrivateAction");
            XmlNode routingAction = root.CreateElement("RoutingAction");
            XmlNode assignRouteAction = root.CreateElement("AssignRouteAction");
            XmlNode route = root.CreateElement("Route");
            SetAttribute("name", "Route", route);
            SetAttribute("closed", "false", route);

            for (int i = 0; i < actionType.PositionsCarla.Count; i++)
            {
                string? routeStrategy = (i == 1) ? "shortest" : "fastest"; // Bugfix to avoid strange Carla behavior
                XmlNode _waypoint = root.CreateElement("Waypoint");
                SetAttribute("routeStrategy", routeStrategy, _waypoint);
                XmlNode position = root.CreateElement("Position");
                XmlNode worldPosition = root.CreateElement("WorldPosition");
                SetAttribute("x", actionType.PositionsCarla[i].Vector3Ser.ToVector3().x.ToString(), worldPosition);
                SetAttribute("y", actionType.PositionsCarla[i].Vector3Ser.ToVector3().y.ToString(), worldPosition);
                SetAttribute("z", actionType.PositionsCarla[i].Vector3Ser.ToVector3().z.ToString(), worldPosition);
                SetAttribute("h", actionType.PositionsCarla[i].Rot.ToString(), worldPosition);

                route.AppendChild(_waypoint);
                _waypoint.AppendChild(position);
                position.AppendChild(worldPosition);
            }
            action.AppendChild(privateAction);
            privateAction.AppendChild(routingAction);
            routingAction.AppendChild(assignRouteAction);
            assignRouteAction.AppendChild(route);
        }

        public void SpeedAction(XmlNode action, ActionType actionType)
        /// Creates SpeedAction. Defines speed for a scenario entity.
        {
            XmlNode privateAction = root.CreateElement("PrivateAction");
            XmlNode longitudinalAction = root.CreateElement("LongitudinalAction");
            XmlNode speedAction = root.CreateElement("SpeedAction");
            XmlNode SpeedActionDynamics = root.CreateElement("SpeedActionDynamics");
            XmlNode SpeedActionTarget = root.CreateElement("SpeedActionTarget");
            XmlNode AbsoluteTargetSpeed = root.CreateElement("AbsoluteTargetSpeed");

            SetAttribute("dynamicsShape", actionType.DynamicsShape, SpeedActionDynamics);
            SetAttribute("value", actionType.SpeedActionDynamicsValue.ToString(), SpeedActionDynamics);
            SetAttribute("dynamicsDimension", actionType.DynamicDimensions, SpeedActionDynamics);
            SetAttribute("value", ((double) actionType.AbsoluteTargetSpeedValueKMH / 3.6).ToString(), AbsoluteTargetSpeed);

            action.AppendChild(privateAction);
            privateAction.AppendChild(longitudinalAction);
            longitudinalAction.AppendChild(speedAction);
            speedAction.AppendChild(SpeedActionDynamics);
            speedAction.AppendChild(SpeedActionTarget);
            SpeedActionTarget.AppendChild(AbsoluteTargetSpeed);
        }

        public void StopAction(XmlNode action, ActionType actionType)
        /// Creates SpeedAction to speed 0. Then creates another SpeedAction to previous speed.
        {
            XmlNode privateAction = root.CreateElement("PrivateAction");
            XmlNode longitudinalAction = root.CreateElement("LongitudinalAction");
            XmlNode speedAction = root.CreateElement("SpeedAction");
            XmlNode SpeedActionDynamics = root.CreateElement("SpeedActionDynamics");
            XmlNode SpeedActionTarget = root.CreateElement("SpeedActionTarget");
            XmlNode AbsoluteTargetSpeed = root.CreateElement("AbsoluteTargetSpeed");

            SetAttribute("dynamicsShape", actionType.DynamicsShape, SpeedActionDynamics);
            SetAttribute("value", actionType.SpeedActionDynamicsValue.ToString(), SpeedActionDynamics);
            SetAttribute("dynamicsDimension", "time", SpeedActionDynamics);
            SetAttribute("value", "0", AbsoluteTargetSpeed);
            

            action.AppendChild(privateAction);
            privateAction.AppendChild(longitudinalAction);
            longitudinalAction.AppendChild(speedAction);
            speedAction.AppendChild(SpeedActionDynamics);
            speedAction.AppendChild(SpeedActionTarget);
            SpeedActionTarget.AppendChild(AbsoluteTargetSpeed);
        }
        public void LaneChangeAction(XmlNode action, ActionType actionType)
        /// Creates LaneChangeAction. Defines amount of lanes to change for a scenario entity relative to a specified entity.
        {
            XmlNode privateAction = root.CreateElement("PrivateAction");
            XmlNode lateralAction = root.CreateElement("LateralAction");
            XmlNode laneChangeAction = root.CreateElement("LaneChangeAction");
            XmlNode laneChangeActionDynamics = root.CreateElement("LaneChangeActionDynamics");
            SetAttribute("dynamicsShape", actionType.DynamicsShape, laneChangeActionDynamics);
            SetAttribute("value", actionType.LaneChangeActionDynamicsValue.ToString(), laneChangeActionDynamics);
            SetAttribute("dynamicsDimension", actionType.DynamicDimensions, laneChangeActionDynamics);
            XmlNode laneChangeTarget = root.CreateElement("LaneChangeTarget");
            XmlNode relativeTargetLane = root.CreateElement("RelativeTargetLane");
            SetAttribute("entityRef", actionType.EntityRef, relativeTargetLane);
            SetAttribute("value", actionType.RelativeTargetLaneValue.ToString(), relativeTargetLane);

            action.AppendChild(privateAction);
            privateAction.AppendChild(lateralAction);
            lateralAction.AppendChild(laneChangeAction);
            laneChangeAction.AppendChild(laneChangeActionDynamics);
            laneChangeAction.AppendChild(laneChangeTarget);
            laneChangeTarget.AppendChild(relativeTargetLane);
        }

        //---------------------------------- InitActions -----------------------------------------
        public void InitialSpeedAction(XmlNode privateAction, double absoluteTargetSpeedValue, string speedActionDynamics = "step", double speedActionDynamicsValue = 0, string dynamicsDimension = "time")
        /// Creates SpeedAction for BuildInit. Defines initial speed of scenario entities.
        {
            XmlNode longitudinalAction = root.CreateElement("LongitudinalAction");
            XmlNode speedAction = root.CreateElement("SpeedAction");
            XmlNode SpeedActionDynamics = root.CreateElement("SpeedActionDynamics");
            XmlNode SpeedActionTarget = root.CreateElement("SpeedActionTarget");
            XmlNode AbsoluteTargetSpeed = root.CreateElement("AbsoluteTargetSpeed");

            SetAttribute("dynamicsShape", speedActionDynamics, SpeedActionDynamics);
            SetAttribute("value", speedActionDynamicsValue.ToString(), SpeedActionDynamics);
            SetAttribute("dynamicsDimension", dynamicsDimension, SpeedActionDynamics);
            SetAttribute("value", absoluteTargetSpeedValue.ToString(), AbsoluteTargetSpeed);

            privateAction.AppendChild(longitudinalAction);
            longitudinalAction.AppendChild(speedAction);
            speedAction.AppendChild(SpeedActionDynamics);
            speedAction.AppendChild(SpeedActionTarget);
            SpeedActionTarget.AppendChild(AbsoluteTargetSpeed);
        }
        public void TeleportAction(XmlNode privateAction, Location spawnPoint)
        /// Creates TeleportAction. Defines start position for scenario entities.
        {
            XmlNode teleport_action = root.CreateElement("TeleportAction");
            XmlNode position = root.CreateElement("Position");
            XmlNode world_position = root.CreateElement("WorldPosition");

            SetAttribute("x", spawnPoint.Vector3Ser.ToVector3().x.ToString(), world_position);
            SetAttribute("y", spawnPoint.Vector3Ser.ToVector3().y.ToString(), world_position);
            SetAttribute("z", spawnPoint.Vector3Ser.ToVector3().z.ToString(), world_position);
            SetAttribute("h", spawnPoint.Rot.ToString(), world_position);

            privateAction.AppendChild(teleport_action);
            teleport_action.AppendChild(position);
            position.AppendChild(world_position);
        }

        public void ControllerAction(XmlNode privateAction, string controlMode, string controllerName = "HeroAgent")
        /// Creates ControllerAction for Ego Vehicle Init.
        // ToDo: validate whether this is necessary for Ego Intit
        {
            XmlNode controller_action = root.CreateElement("ControllerAction");
            XmlNode assign_controller_action = root.CreateElement("AssignControllerAction");
            XmlNode controller = root.CreateElement("Controller");
            SetAttribute("name", controllerName, controller);
            XmlNode properties = root.CreateElement("Properties");
            XmlNode property1 = root.CreateElement("Property");
            
            SetAttribute("name", "module", property1);
            SetAttribute("value", controlMode, property1); // "external_control", "simple_vehicle_control", ...
            if (controlMode == "simple_vehicle_control")
            {
                XmlNode property2 = root.CreateElement("Property");
                SetAttribute("name", "attach_camera", property2);
                SetAttribute("value", "true", property2);
                properties.AppendChild(property2);
            }
            
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
            privateAction.AppendChild(controller_action);
            controller_action.AppendChild(assign_controller_action);
            assign_controller_action.AppendChild(controller);
            controller.AppendChild(properties);
            properties.AppendChild(property1);
            controller_action.AppendChild(override_controller_value_action);
            override_controller_value_action.AppendChild(throttle);
            override_controller_value_action.AppendChild(brake);
            override_controller_value_action.AppendChild(clutch);
            override_controller_value_action.AppendChild(parking_brake);
            override_controller_value_action.AppendChild(steering_wheel);
            override_controller_value_action.AppendChild(gear);
        }

        // Public Actions
        public void EnvironmentAction(XmlNode globalAction, WorldOptions worldOptions) 
        {
            XmlNode environment_action = root.CreateElement("EnvironmentAction");
            XmlNode environment = root.CreateElement("Environment");
            SetAttribute("name", "Environment1", environment);
            XmlNode time_of_day = root.CreateElement("TimeOfDay");
            SetAttribute("animation", "false", time_of_day);
            SetAttribute("dateTime", worldOptions.Date_Time, time_of_day);
            XmlNode weather = root.CreateElement("Weather");
            SetAttribute("cloudState", worldOptions.CloudState.ToString().ToLower(), weather);
            XmlNode sun = root.CreateElement("Sun");
            SetAttribute("intensity", worldOptions.SunIntensity.ToString(), sun);
            SetAttribute("azimuth", worldOptions.SunAzimuth.ToString(), sun);
            SetAttribute("elevation", worldOptions.SunElevation.ToString(), sun);
            XmlNode fog = root.CreateElement("Fog");
            SetAttribute("visualRange", worldOptions.FogVisualRange.ToString(), fog);
            XmlNode precipitation = root.CreateElement("Precipitation");
            SetAttribute("precipitationType", worldOptions.PrecipitationType.ToString().ToLower(), precipitation);
            SetAttribute("intensity", worldOptions.PrecipitationIntensity.ToString(), precipitation);
            XmlNode road_condition = root.CreateElement("RoadCondition");
            SetAttribute("frictionScaleFactor", worldOptions.FrictionScaleFactor.ToString(), road_condition);

            // Hierarchy
            globalAction.AppendChild(environment_action);
            environment_action.AppendChild(environment);
            environment.AppendChild(time_of_day);
            environment.AppendChild(weather);
            weather.AppendChild(sun);
            weather.AppendChild(fog);
            weather.AppendChild(precipitation);
            environment.AppendChild(road_condition);
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
