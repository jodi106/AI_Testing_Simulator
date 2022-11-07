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
                // continue...

            } else if (ValueCondition.Equals("StoryboardElementStateCondition"))
            {

            } else
            {
                Console.WriteLine("Naming error in value condition. This name is not supported.");
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

            //xmlBrick.append
            /*
            <ConditionName>
            <ByValueCondition>

                //Space for value condition
            </ByValueCondition>
            */

            if (ValueCondition == "StoryboardElementStateCondition")
            {
                //xmlBrick.append
                /*
                <StoryboardElementStateCondition storyboardElementType="action" storyboardElementRef="STORY_BOARD_ELEMENT_NAME" state="completeState"/>
                */
            }

            if (ValueCondition == "SimulationTimeCondition")
            {
                /*
                <SimulationTimeCondition value="2.0" rule="greaterThan"/>
                */
            }



        }

        //public void ByEntityCondition(string EntityRef, string EntityCondition, dict args)
        public void ByEntityCondition(string EntityRef, string EntityCondition, string dict_args) // original: dict args
        {
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

            /// xmlBrick.append
            /*
            <ByEntityCondition>
                <TriggeringEntities triggeringEntitiesRule="any">
                    <EntityRef entityRef="adversary0"/>
                </TriggeringEntities>
                    <EntityCondition>
                        //Space for entity condition
                    </EntityCondition>
            </ByEntityCondition>
            */
            if (EntityCondition == "ReachPositionCondition")
            {
                /*
                <ReachPositionCondition tolerance="2.0">
                    <Position>
                    <WorldPosition x="402" y="-150" z="0.3" h="29.9"/>
                    </Position>
                </ReachPositionCondition>
                */
            }

            if (EntityCondition == "DistanceCondition")
            {
                /*
                */
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
