using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ExportScenario.Entities;

namespace ExportScenario.XMLBuilder
{
    internal class BuildAction
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

        public void AcquirePositionAction(XmlNode routingAction)
        /// Creates AcquirePositionAction
        {
            // TODO Variables that need to be inside ScenarioInfo class TODO
            string x = "0.0";
            string y = "0.0";
            string z = "0.0";
            string h = "0.0";

            XmlNode acquirePositionAction = root.CreateElement("AcquirePositionAction");
            XmlNode position = root.CreateElement("position");
            XmlNode worldPosition = root.CreateElement("WorldPosition");
            SetAttribute("x", x, worldPosition);
            SetAttribute("y", y, worldPosition);
            SetAttribute("z", z, worldPosition);
            SetAttribute("h", h, worldPosition);

            // Hierarchy
            routingAction.AppendChild(acquirePositionAction);
            acquirePositionAction.AppendChild(position);
            position.AppendChild(worldPosition);
            /*
                    <RoutingAction>
                    <AcquirePositionAction>
                        <Position>
                        <WorldPosition x="402" y="-150" z="0.3" h="29.9"/>
                        </Position>
                    </AcquirePositionAction>
                    </RoutingAction>
            */
        }

        public void AssignRouteAction(List<string[]> waypoints, XmlNode privateAction)
        /// Creates AssignRouteAction
        {
            // routeStrategy is 'fastest' for vehicles and 'shortest' for pedestrians
            string routeStrategy = "fastest";

            XmlNode routingAction = root.CreateElement("RoutingAction");
            XmlNode assignRouteAction = root.CreateElement("AssignRouteAction");
            XmlNode route = root.CreateElement("Route");
            SetAttribute("name", "Route", route);
            SetAttribute("closed", "false", route);

            foreach (string[] point in waypoints)
            {
                XmlNode waypoint = root.CreateElement("Waypoint");
                SetAttribute("routeStrategy", routeStrategy, waypoint);
                XmlNode position = root.CreateElement("position");
                XmlNode worldPosition = root.CreateElement("WorldPosition");
                SetAttribute("x", point[0], worldPosition);
                SetAttribute("y", point[1], worldPosition);
                SetAttribute("z", point[2], worldPosition);
                SetAttribute("h", point[3], worldPosition);

                route.AppendChild(waypoint);
                waypoint.AppendChild(position);
                position.AppendChild(worldPosition);
            }

            privateAction.AppendChild(routingAction);
            routingAction.AppendChild(assignRouteAction);
            assignRouteAction.AppendChild(route);

            /*
            <RoutingAction>
            <AssignRouteAction>
                <Route name = "TestRoute" closed = "false">
                    <Waypoint routeStrategy = "fastest">
                        <Position>
                        <WorldPosition x="412" y="-100" z="0.3" h="29.9"/>
                        </Position>
                    </Waypoint>
                    <Waypoint routeStrategy = "fastest">
                        <Position>
                        <WorldPosition x="402" y="-150" z="0.3" h="29.9"/>
                        </Position>
                    </Waypoint>					  
                    <Waypoint routeStrategy = "fastest">
                        <Position>
                        <WorldPosition x="412" y="-200" z="0.3" h="29.9"/>
                        </Position>
                    </Waypoint>	
                    <Waypoint routeStrategy = "fastest">
                        <Position>
                        <WorldPosition x="390" y="-300" z="0.3" h="29.9"/>
                        </Position>
                    </Waypoint>
                </Route>
            </AssignRouteAction>
            </RoutingAction>
            */
        }

        public void SpeedAction(double absoluteTargetSpeedValue, XmlNode privateAction, string speedActionDynamics = "step", double speedActionDynamicsValue = 0, string dynamicsDimension = "time")
        /// Creates Speed Action
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

        public void LaneChangeAction(double laneChangeActionDynamicsValue, string entityRef, int relativeTargetLaneValue, XmlNode privateAction)
        /// Creates LaneChangeAction
        {
            XmlNode lateralAction = root.CreateElement("LateralAction");
            XmlNode laneChangeAction = root.CreateElement("LaneChangeAction");
            XmlNode laneChangeActionDynamics = root.CreateElement("LaneChangeActionDynamics");
            SetAttribute("dynamicsShape", "linear", laneChangeActionDynamics);
            SetAttribute("value", laneChangeActionDynamicsValue.ToString(), laneChangeActionDynamics);
            SetAttribute("dynamicsDimension", "distance", laneChangeActionDynamics);
            XmlNode laneChangeTarget = root.CreateElement("LaneChangeTarget");
            XmlNode relativeTargetLane = root.CreateElement("RelativeTargetLane");
            SetAttribute("entityRef", entityRef, relativeTargetLane);
            SetAttribute("value", relativeTargetLaneValue.ToString(), relativeTargetLane);

            privateAction.AppendChild(lateralAction);
            lateralAction.AppendChild(laneChangeAction);
            laneChangeAction.AppendChild(laneChangeActionDynamics);
            laneChangeAction.AppendChild(laneChangeTarget);
            laneChangeTarget.AppendChild(relativeTargetLane);
            /*
            <LateralAction>
            <LaneChangeAction>
                <LaneChangeActionDynamics dynamicsShape="linear" value="25" dynamicsDimension="distance"/>
                <LaneChangeTarget>
                    <RelativeTargetLane entityRef="adversary" value="-1"/>
                </LaneChangeTarget>
            </LaneChangeAction>
            </LateralAction>
            */
        }

        public void TeleportAction(Coord3D spawnPoint, XmlNode privateAction, double rotation)
        {
            XmlNode teleport_action = root.CreateElement("TeleportAction");
            XmlNode position = root.CreateElement("Position");
            XmlNode world_position = root.CreateElement("WorldPosition");

            SetAttribute("x", spawnPoint.X.ToString(), world_position);
            SetAttribute("y", spawnPoint.Y.ToString(), world_position);
            SetAttribute("z", spawnPoint.Z.ToString(), world_position);
            SetAttribute("h", rotation.ToString(), world_position);

            privateAction.AppendChild(teleport_action);
            teleport_action.AppendChild(position);
            position.AppendChild(world_position);
        }

        public void ControllerAction(XmlNode privateAction, string controlMode = "simulation", string propertyName = "module", string controllerName = "HeroAgent")
        {
            
            XmlNode controller_action = root.CreateElement("ControllerAction");
            XmlNode assign_controller_action = root.CreateElement("AssignControllerAction");
            XmlNode controller = root.CreateElement("Controller");
            SetAttribute("name", controllerName, controller);
            XmlNode properties = root.CreateElement("Properties");
            XmlNode property = root.CreateElement("Property");
            SetAttribute("name", propertyName, property);
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
            privateAction.AppendChild(controller_action);
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

        // Public Actions
        public void EnvironmentAction(WorldOptions worldOptions, XmlNode globalAction) 
        {
            XmlNode environment_action = root.CreateElement("EnvironmentAction");
            XmlNode environment = root.CreateElement("Environment");
            SetAttribute("name", "Environment1", environment);
            XmlNode time_of_day = root.CreateElement("TimeOfDay");
            SetAttribute("animation", "false", time_of_day);
            SetAttribute("dateTime", worldOptions.Date_Time, time_of_day);
            XmlNode weather = root.CreateElement("Weather");
            SetAttribute("cloudState", worldOptions.CloudState, weather);
            XmlNode sun = root.CreateElement("Sun");
            SetAttribute("intensity", worldOptions.SunIntensity.ToString(), sun);
            SetAttribute("azimuth", worldOptions.SunAzimuth.ToString(), sun);
            SetAttribute("elevation", worldOptions.SunElevation.ToString(), sun);
            XmlNode fog = root.CreateElement("Fog");
            SetAttribute("visualRange", worldOptions.FogVisualRange.ToString(), fog);
            XmlNode precipitation = root.CreateElement("Precipitation");
            SetAttribute("precipitationType", worldOptions.PrecipitationTypes, precipitation);
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
