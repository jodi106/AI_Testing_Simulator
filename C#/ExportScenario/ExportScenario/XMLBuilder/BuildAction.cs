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
            ///xmlBlock = <Action name="REFERENZ_NAME"><PrivateAction> </PrivateAction></Action>
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

        public void SpeedAction(double speedActionDynamicsValue, double absoluteTargetSpeedValue, XmlNode privateAction)
        /// Creates Speed Action
        {
            XmlNode longitudinalAction = root.CreateElement("LongitudinalAction");
            XmlNode speedAction = root.CreateElement("SpeedAction");
            XmlNode speedActionDynamics = root.CreateElement("SpeedActionDynamics");
            SetAttribute("dynamicsShape", "step", speedActionDynamics);
            SetAttribute("value", speedActionDynamicsValue.ToString(), speedActionDynamics);
            SetAttribute("dynamicsDimension", "distance", speedActionDynamics);
            XmlNode speedActionTarget = root.CreateElement("SpeedActionTarget");
            XmlNode absoluteTargetSpeed = root.CreateElement("AbsoluteTargetSpeed");
            SetAttribute("value", absoluteTargetSpeedValue.ToString(), absoluteTargetSpeed);

            privateAction.AppendChild(longitudinalAction);
            longitudinalAction.AppendChild(speedAction);
            speedAction.AppendChild(speedActionDynamics);
            speedAction.AppendChild(speedActionTarget);
            speedActionTarget.AppendChild(absoluteTargetSpeed);

            /*
            <LongitudinalAction>
            <SpeedAction>
                <SpeedActionDynamics dynamicsShape="step" value="10" dynamicsDimension="time"/>
                <SpeedActionTarget>
                    <AbsoluteTargetSpeed value="0.0"/>
                </SpeedActionTarget>
            </SpeedAction>
            </LongitudinalAction>        
            */
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

        /// helper
        private void SetAttribute(string name, string value, XmlNode element)
        {
            XmlAttribute attribute = root.CreateAttribute(name);
            attribute.Value = value;
            element.Attributes.Append(attribute);
        }
    }
}
