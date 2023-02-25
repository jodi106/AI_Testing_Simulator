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
    public class BuildXML
    /// <summary>Class to create an combine all relevant XML Blocks to final OpenScenario file.</summary>
    {
        private XmlDocument root;
        private XmlNode openScenario;
        private ScenarioInfo scenarioInfo;
        private XmlNode storyBoard;

        private bool builtAtLeastOneStory = false;


        public BuildXML(ScenarioInfo scenarioInfo)
        /// Constructor to initializes BuildXML object with head section.
        {
            this.scenarioInfo = scenarioInfo;

            root = new XmlDocument();
            XmlNode xmlVersion = root.CreateXmlDeclaration("1.0", null, null);
            root.AppendChild(xmlVersion);
            openScenario = root.CreateElement("OpenSCENARIO");
            root.AppendChild(openScenario);
        }

        public void CombineXML()
        /// Combines all xml blocks.
        {
            BuildFirstOpenScenarioElements(scenarioInfo.Path, scenarioInfo.MapURL);

            BuildEntities entities = new BuildEntities(scenarioInfo, root, openScenario);
            entities.CombineEntities();
            BuildStoryboard();

            ExportXML(scenarioInfo.Path);
        }

        public void ExportXML(string path)
        /// Exports the finished OpenScenario file to defined path.
        {
            root.Save(path);
            //root.Save(scenario_name + "3.xosc");
            //root.Save("..\\..\\..\\" + scenario_name + ".xosc");
            //root.Save(Console.Out);
        }

        private void BuildFirstOpenScenarioElements(string scenario_name = "MyScenario", string map = "Town04") // you can rename this method
        /// Creates first ScenarioElements: FileHeader, ParameterDeclarations(EMPTY), CatalogLocations(EMPTY), RoadNetwork.
        {
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

        public void BuildStoryboard()
        /// Combines Init block and all Entity Story blocks. Every Entity has one seperate Story.
        {
            builtAtLeastOneStory = false;

            storyBoard = root.CreateElement("Storyboard");
            openScenario.AppendChild(storyBoard);
            BuildInit init = new BuildInit(scenarioInfo, root, storyBoard);
            init.CombineInit();

            //TODO Pedestrian Botch
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

        public void BuildVehicleStories(Vehicle vehicle)
        /// Creates Vehicle Stories from story head and Events.
        {
            if (vehicle.Path is null) return;
            bool isNullOrEmpty = vehicle.Path.WaypointList?.Any() != true;
            if (!isNullOrEmpty)
            {
                // IMPORTANT !!!
                vehicle.getCarlaLocation();
                vehicle.Path.InitAssignRouteWaypoint(vehicle.SpawnPoint.Rot);

                XmlNode story = root.CreateElement("Story");
                SetAttribute("name", "adversary" + vehicle.Id + "_Story", story);
                XmlNode act = root.CreateElement("Act");
                SetAttribute("name", "adversary" + vehicle.Id + "_Act", act);
                XmlNode maneuverGroup = root.CreateElement("ManeuverGroup");
                SetAttribute("maximumExecutionCount", "1", maneuverGroup);
                SetAttribute("name", "adversary" + vehicle.Id + "Sequence", maneuverGroup);
                XmlNode actors = root.CreateElement("Actors");
                SetAttribute("selectTriggeringEntities", "false", actors);
                XmlNode entityRef = root.CreateElement("EntityRef");
                SetAttribute("entityRef", "adversary" + vehicle.Id, entityRef);
                XmlNode maneuver = root.CreateElement("Maneuver");
                SetAttribute("name", "adversary" + vehicle.Id + "_Maneuver", maneuver);

                for (int i = 0; i < vehicle.Path.WaypointList.Count; i++)
                //foreach
                {
                    if (vehicle.Path.WaypointList[i].ActionTypeInfo.Name == "AssignRouteAction")
                    {
                        BuildEvent(maneuver, vehicle.Path.WaypointList[i].ActionTypeInfo, vehicle.Path.WaypointList[i].TriggerList);
                    }

                    if (vehicle.Path.WaypointList[i].Actions?.Any() == true)
                    {
                        BuildEventsInWaypoint(maneuver, vehicle.Path.WaypointList[i], vehicle);
                    }
                }

                // hierarchy
                storyBoard.AppendChild(story);
                story.AppendChild(act);
                act.AppendChild(maneuverGroup);

                // ToDo implement using OverallStartTrigger from Path
                XmlNode actStartTrigger = root.CreateElement("StartTrigger");

                act.AppendChild(actStartTrigger);
                maneuverGroup.AppendChild(actors);
                actors.AppendChild(entityRef);
                maneuverGroup.AppendChild(maneuver);

                builtAtLeastOneStory = true;
            }
        }
        public void BuildPedestrianStories(Pedestrian pedestrian)
        /// Creates Pedestrian Stories from story head and Events.
        {
            if (pedestrian.Path is null) return;
            bool isNullOrEmpty = pedestrian.Path.WaypointList?.Any() != true;
            if (!isNullOrEmpty)
            {
                // IMPORTANT !!!
                pedestrian.getCarlaLocation();
                pedestrian.Path.InitAssignRouteWaypoint(pedestrian.SpawnPoint.Rot);

                XmlNode story = root.CreateElement("Story");
                SetAttribute("name", "adversary_pedestrian" + pedestrian.Id + "_Story", story);
                XmlNode act = root.CreateElement("Act");
                SetAttribute("name", "adversary_pedestrian" + pedestrian.Id + "_Act", act);
                XmlNode maneuverGroup = root.CreateElement("ManeuverGroup");
                SetAttribute("maximumExecutionCount", "1", maneuverGroup);
                SetAttribute("name", "adversary_pedestrian" + pedestrian.Id + "Sequence", maneuverGroup);
                XmlNode actors = root.CreateElement("Actors");
                SetAttribute("selectTriggeringEntities", "false", actors);
                XmlNode entityRef = root.CreateElement("EntityRef");
                SetAttribute("entityRef", "adversary_pedestrian" + pedestrian.Id, entityRef);
                XmlNode maneuver = root.CreateElement("Maneuver");
                SetAttribute("name", "adversary_pedestrian" + pedestrian.Id + "_Maneuver", maneuver);

                for (int i = 0; i < pedestrian.Path.WaypointList.Count; i++)
                {
                    if (pedestrian.Path.WaypointList[i].ActionTypeInfo.Name == "AssignRouteAction")
                    {
                        BuildEvent(maneuver, pedestrian.Path.WaypointList[i].ActionTypeInfo, pedestrian.Path.WaypointList[i].TriggerList);
                    }

                    if (pedestrian.Path.WaypointList[i].Actions?.Any() == true)
                    {
                        BuildEventsInWaypoint(maneuver, pedestrian.Path.WaypointList[i], pedestrian);
                    }
                }

                // hierarchy
                storyBoard.AppendChild(story);
                story.AppendChild(act);
                act.AppendChild(maneuverGroup);

                // ToDo implement using OverallStartTrigger from Path
                XmlNode actStartTrigger = root.CreateElement("StartTrigger");

                act.AppendChild(actStartTrigger);
                maneuverGroup.AppendChild(actors);
                actors.AppendChild(entityRef);
                maneuverGroup.AppendChild(maneuver);

                builtAtLeastOneStory = true;
                //// ToDo implement using OverallStopTrigger from Path.
                //XmlNode stopTrigger = root.CreateElement("StopTrigger");
                //storyBoard.AppendChild(stopTrigger);
            }
        }

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


        public void BuildEventsInWaypoint(XmlNode maneuver, Waypoint waypoint, BaseEntity entity)
        /// Creates Events by combining Actions and Triggers and combines them to one XML Block. One Event corresponds to one Waypoint Object in the Path.
        {
            //BuildEvent(maneuver, waypoint.ActionTypeInfo, waypoint.TriggerList);

            // Build Actions (0 - 3x)
            foreach (ActionType ac in waypoint.Actions)
            {
                List<TriggerInfo> simpleTrigger = new List<TriggerInfo>();
                simpleTrigger.Add(new TriggerInfo("DistanceCondition", entity.Id, "lessThan", 20, waypoint.LocationCarla));
                BuildEvent(maneuver, ac, simpleTrigger);
            }

            //XmlNode new_event = root.CreateElement("Event");
            //SetAttribute("name", waypoint.ActionTypeInfo.Name + waypoint.ActionTypeInfo.ID, new_event);
            //SetAttribute("priority", waypoint.Priority, new_event);
            //XmlNode action = root.CreateElement("Action");
            //SetAttribute("name", waypoint.ActionTypeInfo.Name + waypoint.ActionTypeInfo.ID, action);

            //// Create Action
            //BuildAction buildAction = new BuildAction(root, "buildAction");
            //Type type = typeof(BuildAction);
            //MethodInfo mi = type.GetMethod(waypoint.ActionTypeInfo.Name);
            //mi.Invoke(buildAction, new object[2] { action, waypoint });
            //new_event.AppendChild(action);

            //// Create Trigger(s)
            //BuildTrigger buildTrigger = new BuildTrigger(root, scenarioInfo);
            //buildTrigger.CombineTrigger(new_event, true, waypoint);
            //maneuver.AppendChild(new_event);
        }

        public void BuildEvent(XmlNode maneuver, ActionType actionType, List<TriggerInfo> triggerInfo)
        {
            XmlNode new_event = root.CreateElement("Event");
            SetAttribute("name", actionType.Name + actionType.ID, new_event);
            SetAttribute("priority", "overwrite", new_event); // Dynamic?
            XmlNode action = root.CreateElement("Action");
            SetAttribute("name", actionType.Name + actionType.ID, action);

            // Create Action
            BuildAction buildAction = new BuildAction(root, "buildAction");
            Type type = typeof(BuildAction);
            MethodInfo mi = type.GetMethod(actionType.Name);
            mi.Invoke(buildAction, new object[2] { action, actionType });
            new_event.AppendChild(action);

            // Create Trigger(s)
            BuildTrigger buildTrigger = new BuildTrigger(root, scenarioInfo);
            buildTrigger.CombineTrigger(new_event, true, triggerInfo);
            maneuver.AppendChild(new_event);
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


