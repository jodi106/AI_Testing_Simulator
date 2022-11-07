using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ExportScenario.Entities;

namespace ExportScenario.XMLBuilder
{
    internal class BuildTrigger
    {
        private XmlDocument root;
        private ScenarioInfo scenarioInfo;
        // private int conditionNr = 0;

        public BuildTrigger(XmlDocument root, ScenarioInfo scenarioInfo)
        {
            this.root = root;
            this.scenarioInfo = scenarioInfo;
        }

        public void CombineTrigger(bool start, XmlNode parentNode, string triggerType) //ToDO change input to TriggerInfo object
        /// Combines Trigger xmlBlock - if required - with multiple condtitions in a condition group 
        {   

            XmlNode trigger;
            if (start)
            {
                ///if StartTrigger{xml = <StartTrigger></StartTrigger><ConditionGroup></ConditionGroup>}
                trigger = root.CreateElement("StartTrigger");
            }
            else
            {
                ///if StopStrigger{xml = <StopTrigger></StopTrigger><ConditionGroup></ConditionGroup> }
                trigger = root.CreateElement("StopTrigger");
            }
            XmlNode conditionGroup = root.CreateElement("ConditionGroup");

            // TODO change "String" to Object
            foreach (String c in byValueCondition)
            {
                ByValueCondition(conditionGroup, "SimulationTimeCondition", c);
            } 
            foreach (String c in byEntityCondition)
            {
                ByEntityCondition(conditionGroup, "", null, c);
            }

            parentNode.AppendChild(trigger);
            trigger.AppendChild(conditionGroup);

            // ToDo implement possibility to create multiple triggers in one contition group by looping over TriggerInfo list (list needs to be implemented, see Waypoint class)
                // ToDo implement Calling a function from triggerType string 



        }

        public void SimulationTimeCondition(XmlNode conditionGroup, Waypoint waypoint) 
        {
            XmlNode simulationTimeCondition = root.CreateElement("SimulationTimeCondition");
            SetAttribute("value", waypoint.Trigger_Info.SimulationTime.ToString(), simulationTimeCondition);
            // continue...
        }




        // ToDo create designated methods for all relevant triggers instead of only ByValueCondition and ByEntityCondition
        // <ConditionGroup><Condition name = "AfterAdversaryAccelerates" delay="0" conditionEdge="rising"> needs to be added to CombineTrigger





        public void ByValueCondition(XmlNode conditionGroup, string ValueCondition, string dict_args) // original: dict args

        {
            // TODO ScenarioInfo

            string conditionEdge = "rising"; // possible string values: "rising" , "falling" , "none" , "risingOrFalling"
            // doc: https://www.asam.net/static_downloads/ASAM_OpenSCENARIO_V1.2.0_Model_Documentation/modelDocumentation/content/ConditionEdge.html

            // SimulationTimeCondition
            double simulationTimeValue = 1;
            string simulationTimeRule = "greaterThan"; // "equalTo" , "greaterThan" , "lessThan" ,
                                                       // (not sure if supported) "greaterOrEqual" , "lessOrEqual" , "notEqualTo"

            // StoryBoardElementStateCondition
            string storyboardElementRef = "STORY_BOARD_ELEMENT_NAME"; // story, act, maneuverGroup, maneuver, event, or action name
            string storyboardElementType = "action"; // "story", "act", "maneuverGroup", "maneuver", "event", "action" 
                                               // This variable must have the same type as the storyboardElementRef name is referring to.
                                               // e.g. storyboardElementRef = "StartCar". This is of type action --> storyboardElementType = "action";
            string storyboardState = "completeState"; // "completeState", "endTransition", "runningState", "skipTransition", 
                                                      // "standbyState", "startTransition", "stopTransition"
                                                      // Doc: https://www.asam.net/static_downloads/ASAM_OpenSCENARIO_V1.2.0_Model_Documentation/modelDocumentation/content/StoryboardElementState.html

            // -----------------------------------------------------------------------------------------

            XmlNode condition = root.CreateElement("Condition");
            SetAttribute("name", "condition", condition);
            // conditionNr++; // I assume every condition needs a unique name. If not, this conditionNr can be deleted. -> does not need new name
            SetAttribute("delay", "0", condition);
            SetAttribute("conditionEdge", conditionEdge, condition);
            XmlNode byValueCondition = root.CreateElement("ByValueCondition");

            conditionGroup.AppendChild(condition);
            condition.AppendChild(byValueCondition);

            if (ValueCondition.Equals("SimulationTimeCondition"))
            {
                XmlNode simulationTimeCondition = root.CreateElement("SimulationTimeCondition");
                SetAttribute("value", simulationTimeValue.ToString(), simulationTimeCondition);
                SetAttribute("rule", simulationTimeRule, simulationTimeCondition);
                byValueCondition.AppendChild(simulationTimeCondition);
            } 
            else if (ValueCondition.Equals("StoryboardElementStateCondition"))
            {
                XmlNode storyboardElementStateCondition = root.CreateElement("StoryboardElementStateCondition");
                SetAttribute("storyboardElementRef", storyboardElementRef, storyboardElementStateCondition);
                SetAttribute("storyboardElementType", storyboardElementType, storyboardElementStateCondition);
                SetAttribute("state", storyboardState, storyboardElementStateCondition);
                byValueCondition.AppendChild(storyboardElementStateCondition);
            } else
            {
                Console.WriteLine("Naming error in value condition. This name is not supported.");
            }
        }

        /**
         <Condition>
           <ByEntityCondition>
               <TriggeringEntities>
                   <EntityRef entityRef="adversary0"/>
               </TriggeringEntities>
                   <EntityCondition>
                       //Space for entity condition
                   </EntityCondition>
           </ByEntityCondition>
         <Condition>
        */
        public void ByEntityCondition(XmlNode conditionGroup, string EntityCondition, List<string> allEntityRefs, string dict_args) // original: Dict args
        {
            // TODO ScenarioInfo

            string conditionEdge = "rising"; // possible string values: "rising" , "falling" , "none" , "risingOrFalling"
            // doc: https://www.asam.net/static_downloads/ASAM_OpenSCENARIO_V1.2.0_Model_Documentation/modelDocumentation/content/ConditionEdge.html

            List<string> allEntityRefs_; // contains strings like "adversary0" , "adversary0" , "hero", ...

            // ReachPositionCondition
            double tolerance = 3.0;

            //DistanceCondition
            bool freespace = false; // True: distance is measured between closest bounding box points. False: reference point distance is used.
            string rule = "greaterThan"; // "equalTo", "greaterThan", "lessThan", (not sure if supported) "greaterOrEqual", "lessOrEqual", "notEqualTo"
            double value = 3.0; // The distance value. Unit: [m]. Range: [0..inf[.
            string routingAlgorithm = "fastest"; // "undefined", "fastest" , "shortest" , ... (these 2 are the relevant options)

            // -----------------------------------------------------------------------

            XmlNode condition = root.CreateElement("Condition");
            SetAttribute("name", "condition" + conditionNr.ToString(), condition);
            conditionNr++; // I assume every condition needs a unique name. If not, this conditionNr can be deleted.
            SetAttribute("delay", "0", condition);
            SetAttribute("conditionEdge", conditionEdge, condition);
            XmlNode byEntityCondition = root.CreateElement("ByEntityCondition");
            XmlNode triggeringEntities = root.CreateElement("TriggeringEntities");
            SetAttribute("triggeringEntitiesRule", "any", triggeringEntities);
            
            foreach (string r in allEntityRefs)
            {
                XmlNode entityRef = root.CreateElement("EntityRef");
                SetAttribute("entityRef", r, entityRef);
                triggeringEntities.AppendChild(entityRef);
            }

            XmlNode entityCondition = root.CreateElement("EntityCondition");

            conditionGroup.AppendChild(condition);
            condition.AppendChild(byEntityCondition);
            byEntityCondition.AppendChild(triggeringEntities);
            byEntityCondition.AppendChild(entityCondition);

            if (EntityCondition.Equals("DistanceCondition"))
            {
                XmlNode distanceCondition = root.CreateElement("DistanceCondition");
                SetAttribute("freespace", freespace.ToString(), distanceCondition);
                SetAttribute("rule", rule, distanceCondition);
                SetAttribute("value", value.ToString(), distanceCondition);
                SetAttribute("routingAlgorithm", routingAlgorithm, distanceCondition);
                entityCondition.AppendChild(distanceCondition);
            }
            else if (EntityCondition.Equals("ReachPositionCondition"))
            {
                XmlNode reachPositionCondition = root.CreateElement("ReachPositionCondition");
                SetAttribute("tolerance", tolerance.ToString(), reachPositionCondition);
                entityCondition.AppendChild(reachPositionCondition);
            }
            else
            {
                Console.WriteLine("Naming error in Entity condition. This name is not supported.");
            }
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


/* All Value Conditions
            <!-- parameterCondition -->
            <!-- timeOfDayCondition -->
            <!-- IMPLEMENTED simulationTimeCondition params: value(float), rule (enum(less, greater, equal))-->
            <!-- IMPLEMENTED storyboardElementStateCondition params: storyboardElementType(enum(6 options)), storyboardElementRef(string), state (enum(7 options))-->
            <!-- userDefinedValueCondition -->
            <!-- trafficSignalCondition -->
            <!-- trafficSignalControllerCondition -->
            */

/* All EntityConditions
            <!-- endOfRoadCondition-->
            <!-- collisionCondition-->
            <!-- offroadCondition-->
            <!-- timeHeadwayCondition-->
            <!-- timeToCollisionCondition-->
            <!-- accelerationCondition-->
            <!-- standStillCondition-->
            <!-- speedCondition-->
            <!-- relativeSpeedCondition-->
            <!-- traveledDistanceCondition params-->
            <!-- IMPLEMETED reachPositionCondition params: tolerance(float), WorldPosition (WorldPosition CREATE STRING FROM X,Y,Z,H)  -->
            <!-- distanceCondition params-->
            <!-- relativeDistanceCondition-->
            */