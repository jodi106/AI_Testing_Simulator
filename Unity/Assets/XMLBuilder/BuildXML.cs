using Entity;
using Assets.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;

namespace ExportScenario.XMLBuilder
{

    /// <summary>
    /// Class to create an combine all relevant XML Blocks to final OpenScenario file
    /// </summary>
    public class BuildXML
    {
        private XmlDocument root;
        private XmlNode openScenario;
        private ScenarioInfo scenarioInfo;
        private XmlNode storyBoard;

        private bool builtAtLeastOneStory = false;

        /// <summary>
        /// Constructor that initializes BuildXML object with head section.
        /// </summary>
        /// <param name="scenarioInfo">Scenario information object</param>
        public BuildXML(ScenarioInfo scenarioInfo)
        {
            this.scenarioInfo = scenarioInfo;

            root = new XmlDocument();
            XmlNode xmlVersion = root.CreateXmlDeclaration("1.0", null, null);
            root.AppendChild(xmlVersion);
            openScenario = root.CreateElement("OpenSCENARIO");
            root.AppendChild(openScenario);
        }

        /// <summary>
        /// Combines all XML blocks.
        /// </summary>
        public void CombineXML()
        {
            BuildFirstOpenScenarioElements(scenarioInfo.Path, scenarioInfo.MapURL);

            BuildEntities entities = new BuildEntities(scenarioInfo, root, openScenario);
            entities.CombineEntities();
            BuildStoryboard();

            ExportXML(scenarioInfo.Path);
        }

        /// <summary>
        /// Exports the finished OpenScenario file to the defined path.
        /// </summary>
        /// <param name="path">Path where the OpenScenario file will be saved</param>
        public void ExportXML(string path)
        {
            root.Save(path);
            //root.Save(scenario_name + "3.xosc");
            //root.Save("..\\..\\..\\" + scenario_name + ".xosc");
            //root.Save(Console.Out);
        }

        /// <summary>
        /// Creates the first ScenarioElements: FileHeader, ParameterDeclarations (EMPTY), CatalogLocations (EMPTY), and RoadNetwork.
        /// </summary>
        /// <param name="scenario_name">The name of the scenario (default is "MyScenario")</param>
        /// <param name="map">The name of the map (default is "Town04")</param>
        private void BuildFirstOpenScenarioElements(string scenario_name = "MyScenario", string map = "Town04") // you can rename this method
        /// Creates first ScenarioElements: FileHeader, ParameterDeclarations(EMPTY), CatalogLocations(EMPTY), RoadNetwork.
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            
            // split scenario_name: e.g. C:\\Users\\jonas\\Desktop\\myScenario.xosc --> myScenario
            char[] separators = new char[] { '/', '\\' };
            string[] name_split = scenario_name.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            scenario_name = name_split[name_split.Length - 1];
            scenario_name = scenario_name.Split(".")[0];

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

        /// <summary>
        /// Combines Init block and all Entity Story blocks. Every Entity has one separate Story.
        /// </summary>
        public void BuildStoryboard()
        {
            builtAtLeastOneStory = false;

            storyBoard = root.CreateElement("Storyboard");
            openScenario.AppendChild(storyBoard);
            BuildInit init = new BuildInit(scenarioInfo, root, storyBoard);
            init.CombineInit();

            BuildEgoStory(scenarioInfo.EgoVehicle);

            for (int i = 0; i < scenarioInfo.Vehicles.Count; i++)
            {
                BuildVehicleStories(scenarioInfo.Vehicles[i]);
            }

            for (int i = 0; i < scenarioInfo.Pedestrians.Count; i++)
            {
                BuildPedestrianStories(scenarioInfo.Pedestrians[i]);
            }

            if (!builtAtLeastOneStory)
            {
                // create empty dummy story to avoid syntax error
                BuildEmptyStory();
            }

            XmlNode stoptrigger = root.CreateElement("StopTrigger");
            storyBoard.AppendChild(stoptrigger);

            // Create Criteria Conditions
            BuildTrigger buildTrigger = new BuildTrigger(root, scenarioInfo);
            buildTrigger.CriteriaConditions(stoptrigger);
        }

        /// <summary>
        /// Creates Ego Vehicle Stories from the story head and Events.
        /// </summary>
        /// <param name="ego">The Ego vehicle object</param>
        public void BuildEgoStory(Ego ego)
        /// Creates Vehicle Stories from story head and Events.
        {
            if (ego.Destination is null) return;

            XmlNode story = root.CreateElement("Story");
            SetAttribute("name", ego.Id + "_Story", story);
            XmlNode act = root.CreateElement("Act");
            SetAttribute("name", ego.Id + "_Act", act);
            XmlNode maneuverGroup = root.CreateElement("ManeuverGroup");
            SetAttribute("maximumExecutionCount", "1", maneuverGroup);
            SetAttribute("name", ego.Id + "Sequence", maneuverGroup);
            XmlNode actors = root.CreateElement("Actors");
            SetAttribute("selectTriggeringEntities", "false", actors);
            XmlNode entityRef = root.CreateElement("EntityRef");
            SetAttribute("entityRef", ego.Id, entityRef);
            XmlNode maneuver = root.CreateElement("Maneuver");
            SetAttribute("name", ego.Id + "_Maneuver", maneuver);

            // IMPORTANT !!!
            var egoAction = new ActionType("AcquirePositionAction", new List<Location>() { ego.Destination });
            var egoTrigger = new List<TriggerInfo>() { new TriggerInfo("SimulationTimeCondition", 0, "greaterThan") };
            BuildEvent(maneuver, egoAction, egoTrigger, "EgoVehicle-Destination");

            // hierarchy
            storyBoard.AppendChild(story);
            story.AppendChild(act);
            act.AppendChild(maneuverGroup);
            maneuverGroup.AppendChild(actors);
            actors.AppendChild(entityRef);
            maneuverGroup.AppendChild(maneuver);

            XmlNode actStartTrigger = root.CreateElement("StartTrigger");
            act.AppendChild(actStartTrigger);

            builtAtLeastOneStory = true;
        }

        /// <summary>
        /// Creates Vehicle Stories from the story head and Events.
        /// </summary>
        /// <param name="vehicle">The Adversary vehicle object</param>
        public void BuildVehicleStories(Adversary vehicle)
        /// Creates Vehicle Stories from story head and Events.
        {
            if (vehicle.Path is null) return;
            bool isNullOrEmpty = vehicle.Path.WaypointList?.Any() != true;
            if (!isNullOrEmpty)
            {
                // IMPORTANT !!!
                vehicle.getCarlaLocation();
                vehicle.Path.InitAssignRouteWaypoint(vehicle.SpawnPoint);

                XmlNode story = root.CreateElement("Story");
                SetAttribute("name", vehicle.Id + "_Story", story);
                XmlNode act = root.CreateElement("Act");
                SetAttribute("name", vehicle.Id + "_Act", act);
                XmlNode maneuverGroup = root.CreateElement("ManeuverGroup");
                SetAttribute("maximumExecutionCount", "1", maneuverGroup);
                SetAttribute("name", vehicle.Id + "Sequence", maneuverGroup);
                XmlNode actors = root.CreateElement("Actors");
                SetAttribute("selectTriggeringEntities", "false", actors);
                XmlNode entityRef = root.CreateElement("EntityRef");
                SetAttribute("entityRef", vehicle.Id, entityRef);
                XmlNode maneuver = root.CreateElement("Maneuver");
                SetAttribute("name", vehicle.Id + "_Maneuver", maneuver);

                for (int i = 0; i < vehicle.Path.WaypointList.Count; i++)
                {
                    if (vehicle.Path.WaypointList[i].ActionTypeInfo.Name == "AssignRouteAction")
                    {
                        // Get route strategies of all waypoints and save them in one array
                        List<WaypointStrategy> routeStrategies = new List<WaypointStrategy>();
                        foreach (Waypoint w in vehicle.Path.WaypointList)
                        {
                            routeStrategies.Add(w.Strategy);
                        }
                        BuildEvent(maneuver, vehicle.Path.WaypointList[i].ActionTypeInfo, vehicle.Path.WaypointList[i].TriggerList, 
                            null, routeStrategies);
                    }

                    else if (vehicle.Path.WaypointList[i].ActionTypeInfo.Name == "AcquirePositionAction")
                    {
                        BuildEvent(maneuver, vehicle.Path.WaypointList[i].ActionTypeInfo, vehicle.Path.WaypointList[i].TriggerList);
                    }

                    if (vehicle.Path.WaypointList[i].Actions?.Any() == true)
                    {
                        BuildEventsInWaypoint(maneuver, vehicle.Path.WaypointList[i], vehicle);
                    }
                }

                // Each adversary needs a StopAction at the end of their path. Otherwise they might run into walls and do non intentional stuff.
                ActionType stopActionEndOfPath = new ActionType("StopAction", 5, 0);
                TriggerInfo triggerEndOfPathReached1 = new TriggerInfo("StoryboardElementStateCondition", vehicle.Path.WaypointList[0].ActionTypeInfo);
                TriggerInfo triggerEndOfPathReached2 = new TriggerInfo("ReachPositionCondition", vehicle.Id, 10, vehicle.Path.WaypointList[vehicle.Path.WaypointList.Count-1].Location);
                List<TriggerInfo> triggerList = new List<TriggerInfo> { triggerEndOfPathReached1, triggerEndOfPathReached2 };
                foreach (Waypoint w in vehicle.Path.WaypointList)
                {
                    foreach (ActionType action in w.Actions)
                    triggerList.Add(new TriggerInfo("StoryboardElementStateCondition", action));
                }
                BuildEvent(maneuver, stopActionEndOfPath, triggerList);

                // hierarchy
                storyBoard.AppendChild(story);
                story.AppendChild(act);
                act.AppendChild(maneuverGroup);
                maneuverGroup.AppendChild(actors);
                actors.AppendChild(entityRef);
                maneuverGroup.AppendChild(maneuver);

                // Start Story StartTrigger
                StartStory(act, maneuver, vehicle);

                builtAtLeastOneStory = true;
            }
        }

        /// <summary>
        /// Creates Pedestrian Stories from the story head and Events.
        /// </summary>
        /// <param name="pedestrian">The Adversary pedestrian object</param>
        public void BuildPedestrianStories(Adversary pedestrian)
        /// Creates Pedestrian Stories from story head and Events.
        {
            if (pedestrian.Path is null) return;
            bool isNullOrEmpty = pedestrian.Path.WaypointList?.Any() != true;
            if (!isNullOrEmpty)
            {
                // IMPORTANT !!!
                pedestrian.getCarlaLocation();
                pedestrian.Path.InitAssignRouteWaypoint(pedestrian.SpawnPoint);

                XmlNode story = root.CreateElement("Story");
                SetAttribute("name", pedestrian.Id + "_Story", story);
                XmlNode act = root.CreateElement("Act");
                SetAttribute("name", pedestrian.Id + "_Act", act);
                XmlNode maneuverGroup = root.CreateElement("ManeuverGroup");
                SetAttribute("maximumExecutionCount", "1", maneuverGroup);
                SetAttribute("name", pedestrian.Id + "Sequence", maneuverGroup);
                XmlNode actors = root.CreateElement("Actors");
                SetAttribute("selectTriggeringEntities", "false", actors);
                XmlNode entityRef = root.CreateElement("EntityRef");
                SetAttribute("entityRef", pedestrian.Id, entityRef);
                XmlNode maneuver = root.CreateElement("Maneuver");
                SetAttribute("name", pedestrian.Id + "_Maneuver", maneuver);

                for (int i = 0; i < pedestrian.Path.WaypointList.Count; i++)
                {
                    if (pedestrian.Path.WaypointList[i].ActionTypeInfo.Name == "AssignRouteAction")
                    {
                        // Get route strategies of all waypoints and save them in one array
                        List<WaypointStrategy> routeStrategies = new List<WaypointStrategy>();
                        foreach (Waypoint w in pedestrian.Path.WaypointList)
                        {
                            routeStrategies.Add(w.Strategy);
                        }
                        BuildEvent(maneuver, pedestrian.Path.WaypointList[i].ActionTypeInfo, pedestrian.Path.WaypointList[i].TriggerList, null, routeStrategies);
                    }

                    else if (pedestrian.Path.WaypointList[i].ActionTypeInfo.Name == "AcquirePositionAction")
                    {
                        BuildEvent(maneuver, pedestrian.Path.WaypointList[i].ActionTypeInfo, pedestrian.Path.WaypointList[i].TriggerList);
                    }

                    if (pedestrian.Path.WaypointList[i].Actions?.Any() == true)
                    {
                        BuildEventsInWaypoint(maneuver, pedestrian.Path.WaypointList[i], pedestrian);
                    }
                }

                // Each adversary needs a StopAction at the end of their path. Otherwise they might run into walls and do non intentional stuff.
                ActionType stopActionEndOfPath = new ActionType("StopAction", 5, 0);
                TriggerInfo triggerEndOfPathReached1 = new TriggerInfo("StoryboardElementStateCondition", pedestrian.Path.WaypointList[0].ActionTypeInfo);
                TriggerInfo triggerEndOfPathReached2 = new TriggerInfo("ReachPositionCondition", pedestrian.Id, 10, pedestrian.Path.WaypointList[pedestrian.Path.WaypointList.Count - 1].Location);
                List<TriggerInfo> triggerList = new List<TriggerInfo> { triggerEndOfPathReached1, triggerEndOfPathReached2 };
                foreach (Waypoint w in pedestrian.Path.WaypointList)
                {
                    foreach (ActionType action in w.Actions)
                        triggerList.Add(new TriggerInfo("StoryboardElementStateCondition", action));
                }
                BuildEvent(maneuver, stopActionEndOfPath, triggerList);

                // hierarchy
                storyBoard.AppendChild(story);
                story.AppendChild(act);
                act.AppendChild(maneuverGroup);
                maneuverGroup.AppendChild(actors);
                actors.AppendChild(entityRef);
                maneuverGroup.AppendChild(maneuver);

                // Start Story StartTrigger
                StartStory(act, maneuver, pedestrian);

                builtAtLeastOneStory = true;
            }
        }

        /// <summary>
        /// Builds an event by combining actions and triggers and appends it to a given XML node.
        /// One event corresponds to one waypoint object in the path.
        /// </summary>
        /// <param name="maneuver">The XML node to which the event will be appended.</param>
        /// <param name="actionType">The action type to be executed in the event.</param>
        /// <param name="triggerInfo">The list of trigger information to be executed in the event.</param>
        /// <param name="name">The name of the event. If null, a default name will be set based on the action type.</param>
        public void BuildEvent(XmlNode maneuver, ActionType actionType, List<TriggerInfo> triggerInfo, string name = null, List<WaypointStrategy> routeStrategies = null)
        {
            XmlNode new_event = root.CreateElement("Event");
            if (name == null) SetAttribute("name", actionType.Name + actionType.ID, new_event);
            else SetAttribute("name", actionType.Name + "-" + name, new_event);
            SetAttribute("priority", "overwrite", new_event); // Dynamic?
            XmlNode action = root.CreateElement("Action");
            SetAttribute("name", actionType.Name + actionType.ID, action);

            // Create Action
            BuildAction buildAction = new BuildAction(root, "buildAction");
            Type type = typeof(BuildAction);
            MethodInfo mi = type.GetMethod(actionType.Name);
            if (actionType.Name == "AssignRouteAction")
            {
                mi.Invoke(buildAction, new object[3] { action, actionType, routeStrategies});
            }
            else
            {
                mi.Invoke(buildAction, new object[2] { action, actionType });
            }
            
            new_event.AppendChild(action);

            // Create Trigger(s)
            BuildTrigger buildTrigger = new BuildTrigger(root, scenarioInfo);
            buildTrigger.CombineTrigger(new_event, true, triggerInfo);
            maneuver.AppendChild(new_event);
        }

        /// <summary>
        /// Builds events by combining actions and triggers and combines them to one XML block.
        /// One event corresponds to one waypoint object in the path.
        /// </summary>
        /// <param name="maneuver">The XML node to which the events will be appended.</param>
        /// <param name="waypoint">The waypoint object containing the actions and location information.</param>
        /// <param name="entity">The entity associated with the events.</param>
        public void BuildEventsInWaypoint(XmlNode maneuver, Waypoint waypoint, BaseEntity entity)
        {
            int indexStopAction = waypoint.Actions.FindIndex(action => action.Name == "StopAction");
            int indexSpeedAction = waypoint.Actions.FindIndex(action => action.Name == "SpeedAction");
            int indexLaneChangeAction = waypoint.Actions.FindIndex(action => action.Name == "LaneChangeAction");
            
            // Set correct speed for SpeedAction after StopAction
            double accelerateSpeed = 0;
            if (indexStopAction >= 0 && indexSpeedAction >= 0) // Check if SpeedAction and StopAction exist both
            {
                // element exists
                accelerateSpeed = waypoint.Actions[indexSpeedAction].AbsoluteTargetSpeedValueKMH;
            }
            else if (indexStopAction >= 0 && indexSpeedAction == -1)
            {
                accelerateSpeed = waypoint.Actions[indexStopAction].AbsoluteTargetSpeedValueKMH;
            }

            // Build Actions (0 - 3x)
            if (indexLaneChangeAction >= 0)
            {
                List<TriggerInfo> simpleTrigger = new List<TriggerInfo>();
                simpleTrigger.Add(new TriggerInfo("DistanceCondition", entity.Id, "lessThan", 5, waypoint.Location));
                BuildEvent(maneuver, waypoint.Actions[indexLaneChangeAction], simpleTrigger);
            }

            if (indexStopAction >= 0)
            {
                // 1. SpeedAction to 0
                List<TriggerInfo> simpleTrigger = new List<TriggerInfo>();
                simpleTrigger.Add(new TriggerInfo("DistanceCondition", entity.Id, "lessThan", 5, waypoint.Location));
                BuildEvent(maneuver, waypoint.Actions[indexStopAction], simpleTrigger);

                // 2. SpeedAction to x>0
                ActionType accelerateAction = new ActionType("SpeedAction", accelerateSpeed);
                List<TriggerInfo> triggers = new List<TriggerInfo>();
                triggers.Add(new TriggerInfo("StandStillCondition", entity.Id, waypoint.Actions[indexStopAction].StopDuration));
                triggers.Add(new TriggerInfo("StoryboardElementStateCondition", waypoint.Actions[indexStopAction]));
                BuildEvent(maneuver, accelerateAction, triggers);
            }

            if (indexSpeedAction >= 0 && indexStopAction == -1)
            {
                List<TriggerInfo> simpleTrigger = new List<TriggerInfo>();
                simpleTrigger.Add(new TriggerInfo("DistanceCondition", entity.Id, "lessThan", 5, waypoint.Location));
                BuildEvent(maneuver, waypoint.Actions[indexSpeedAction], simpleTrigger);
            }
        }

        /// <summary>
        /// Starts the story by creating and appending the start trigger to a given XML node.
        /// </summary>
        /// <param name="act">The XML node representing the act to which the start trigger will be appended.</param>
        /// <param name="maneuver">The XML node representing the maneuver to which the start trigger will be appended.</param>
        /// <param name="vehicle">The adversary vehicle associated with the start trigger.</param>
        private void StartStory(XmlNode act, XmlNode maneuver, Adversary vehicle)
        {
            ActionType startStorySpeedAction;
            TriggerInfo startStoryTrigger;

            if (vehicle.StartPathInfo != null)
            {
                if (vehicle.StartPathInfo.Type == "Time")
                {
                    startStorySpeedAction = new ActionType("SpeedAction", vehicle.InitialSpeedKMH);
                    startStoryTrigger = new TriggerInfo("ReachPositionCondition", vehicle.Id, 5, vehicle.SpawnPoint);

                    BuildEvent(maneuver, startStorySpeedAction, new List<TriggerInfo> { startStoryTrigger }, "OverallStartCondition");
                    vehicle.InitialSpeedKMH = 0;

                    startStoryTrigger = new TriggerInfo("SimulationTimeCondition", vehicle.StartPathInfo.Time, "greaterThan");
                    BuildTrigger buildTrigger = new BuildTrigger(root, scenarioInfo);
                    buildTrigger.CombineTrigger(act, true, new List<TriggerInfo> { startStoryTrigger });
                }
                else if (vehicle.StartPathInfo.Type == "Waypoint") 
                {
                    startStorySpeedAction = new ActionType("SpeedAction", vehicle.InitialSpeedKMH);
                    string? triggerEntityId = (vehicle.StartPathInfo.Type == "Ego") ? vehicle.StartPathInfo.EgoVehicle.Id : vehicle.StartPathInfo.Vehicle.Id;
                    startStoryTrigger = new TriggerInfo("ReachPositionCondition", triggerEntityId, vehicle.StartPathInfo.Distance, vehicle.StartPathInfo.LocationCarla);

                    BuildEvent(maneuver, startStorySpeedAction, new List<TriggerInfo> { startStoryTrigger }, "OverallStartCondition");
                    vehicle.InitialSpeedKMH = 0;

                    XmlNode actStartTrigger = root.CreateElement("StartTrigger");
                    act.AppendChild(actStartTrigger);
                }
                else if (vehicle.StartPathInfo.Type == "Ego")
                {
                    startStorySpeedAction = new ActionType("SpeedAction", vehicle.InitialSpeedKMH);
                    string? triggerEntityId = (vehicle.StartPathInfo.Type == "Ego") ? vehicle.StartPathInfo.EgoVehicle.Id : vehicle.StartPathInfo.Vehicle.Id;
                    startStoryTrigger = new TriggerInfo("RelativeDistanceCondition", triggerEntityId, vehicle.Id, vehicle.StartPathInfo.Distance);

                    BuildEvent(maneuver, startStorySpeedAction, new List<TriggerInfo> { startStoryTrigger }, "OverallStartCondition");
                    vehicle.InitialSpeedKMH = 0;

                    XmlNode actStartTrigger = root.CreateElement("StartTrigger");
                    act.AppendChild(actStartTrigger);
                }
            }
            else
            {
                XmlNode actStartTrigger = root.CreateElement("StartTrigger");
                act.AppendChild(actStartTrigger);
            }
        }

        /// <summary>
        /// Builds an empty story by creating and appending the necessary XML nodes.
        /// </summary>
        public void BuildEmptyStory()
        {
            XmlNode story = root.CreateElement("Story");
            SetAttribute("name", "empty_Story", story);
            XmlNode act = root.CreateElement("Act");
            SetAttribute("name", "empty_Act", act);
            XmlNode maneuverGroup = root.CreateElement("ManeuverGroup");
            SetAttribute("maximumExecutionCount", "1", maneuverGroup);
            SetAttribute("name", "empty_Sequence", maneuverGroup);
            XmlNode actors = root.CreateElement("Actors");
            SetAttribute("selectTriggeringEntities", "false", actors);
            XmlNode actStartTrigger = root.CreateElement("StartTrigger");

            storyBoard.AppendChild(story);
            story.AppendChild(act);
            act.AppendChild(maneuverGroup);
            act.AppendChild(actStartTrigger);
            maneuverGroup.AppendChild(actors);
        }

        /// <summary>
        /// Helper method to set an attribute with a given name and value for a specified XML element.
        /// </summary>
        /// <param name="name">The name of the attribute to be set.</param>
        /// <param name="value">The value to be assigned to the attribute.</param>
        /// <param name="element">The XML element to which the attribute is to be added.</param>
        private void SetAttribute(string name, string value, XmlNode element)
        {
            XmlAttribute attribute = root.CreateAttribute(name);
            attribute.Value = value;
            element.Attributes.Append(attribute);
        }
    }
}
