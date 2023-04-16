using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using Entity;
using static System.Collections.Specialized.BitVector32;

namespace ExportScenario.XMLBuilder
{

    /// <summary>
    ///  Class to create Tiggers. Trigger allow to define start or stop conditions for Storys or Events.
    /// </summary>
    internal class BuildTrigger
    {
        private XmlDocument root;
        private ScenarioInfo scenarioInfo;

        /// <summary>
        /// Constructor for the BuildTrigger class.
        /// </summary>
        /// <param name="root">XmlDocument instance representing the root element.</param>
        /// <param name="scenarioInfo">ScenarioInfo instance containing scenario information.</param>
        public BuildTrigger(XmlDocument root, ScenarioInfo scenarioInfo)
        {
            this.root = root;
            this.scenarioInfo = scenarioInfo;
        }

        /// <summary>
        /// Combines Trigger xmlBlock - if required - with multiple conditions in a condition group.
        /// </summary>
        /// <param name="parentNode">Parent XmlNode where the trigger will be appended.</param>
        /// <param name="start">True if creating a StartTrigger, false if creating a StopTrigger.</param>
        /// <param name="TriggerList">List of TriggerInfo objects containing the trigger information.</param>
        public void CombineTrigger(XmlNode parentNode, bool start, List<TriggerInfo> TriggerList)
        {
            XmlNode trigger;
            if (start)
            {
                trigger = root.CreateElement("StartTrigger");
            }
            else
            {
                trigger = root.CreateElement("StopTrigger");
            }

            XmlNode conditionGroup = root.CreateElement("ConditionGroup");
            for (int i = 0; i < TriggerList.Count; i++)
            {
                XmlNode condition = root.CreateElement("Condition");
                SetAttribute("name", TriggerList[i].TriggerType + TriggerList[i].ID, condition);
                SetAttribute("delay", TriggerList[i].Delay.ToString(), condition);
                SetAttribute("conditionEdge", TriggerList[i].ConditionEdge.ToString().FirstCharToLowerCase(), condition);

                // Invokes a Method for specified object with specified inputs via a String
                MethodInfo mi = this.GetType().GetMethod(TriggerList[i].TriggerType);
                mi.Invoke(this, new object[] { condition, TriggerList[i] });
                conditionGroup.AppendChild(condition);
            }
            parentNode.AppendChild(trigger);
            trigger.AppendChild(conditionGroup);
        }

        /// <summary>
        /// Creates SimulationTimeCondition. Triggers after the simulation has run for a specified time.
        /// </summary>
        /// <param name="condition">XmlNode representing the condition element.</param>
        /// <param name="triggerInfo">TriggerInfo object containing the trigger information.</param>
        public void SimulationTimeCondition(XmlNode condition, TriggerInfo triggerInfo)
        /// Creates SimulationTimeCondition. Triggers after simulation ran for specified time.
        {
            XmlNode byValueCondition = root.CreateElement("ByValueCondition");
            condition.AppendChild(byValueCondition);
            XmlNode simulationTimeCondition = root.CreateElement("SimulationTimeCondition");
            SetAttribute("value", triggerInfo.SimulationTimeValue.ToString(), simulationTimeCondition);
            SetAttribute("rule", triggerInfo.Rule.ToString().FirstCharToLowerCase(), simulationTimeCondition);
            byValueCondition.AppendChild(simulationTimeCondition);
        }

        /// <summary>
        /// Creates DistanceCondition. Triggers when a specified entity has traveled a specified distance.
        /// </summary>
        /// <param name="condition">XmlNode representing the condition element.</param>
        /// <param name="triggerInfo">TriggerInfo object containing the trigger information.</param>
        public void DistanceCondition(XmlNode condition, TriggerInfo triggerInfo)
        {
            XmlNode byEntityCondition = root.CreateElement("ByEntityCondition");
            XmlNode triggeringEntities = root.CreateElement("TriggeringEntities");
            SetAttribute("triggeringEntitiesRule", "any", triggeringEntities);
            XmlNode entityRef = root.CreateElement("EntityRef");
            SetAttribute("entityRef", triggerInfo.EntityRef, entityRef);

            XmlNode entityCondition = root.CreateElement("EntityCondition");
            XmlNode distanceCondition = root.CreateElement("DistanceCondition");
            SetAttribute("freespace", "false", distanceCondition); // true is not implemented in carla
            SetAttribute("rule", triggerInfo.Rule.ToString().FirstCharToLowerCase(), distanceCondition);
            SetAttribute("value", triggerInfo.Value.ToString(), distanceCondition);
            SetAttribute("alongRoute", "true", distanceCondition);
            XmlNode position = root.CreateElement("Position");
            XmlNode worldposition = root.CreateElement("WorldPosition");
            SetAttribute("x", triggerInfo.WorldPositionCarla.Vector3Ser.ToVector3().x.ToString(), worldposition);
            SetAttribute("y", triggerInfo.WorldPositionCarla.Vector3Ser.ToVector3().y.ToString(), worldposition);
            SetAttribute("z", triggerInfo.WorldPositionCarla.Vector3Ser.ToVector3().z.ToString(), worldposition);
            SetAttribute("h", triggerInfo.WorldPositionCarla.Rot.ToString(), worldposition);

            // hierarchy
            condition.AppendChild(byEntityCondition);
            byEntityCondition.AppendChild(triggeringEntities);
            byEntityCondition.AppendChild(entityCondition);
            triggeringEntities.AppendChild(entityRef);
            entityCondition.AppendChild(distanceCondition);
            distanceCondition.AppendChild(position);
            position.AppendChild(worldposition);
        }

        /// <summary>
        /// Create RelativeDistanceCondition. Like ReachPositionAction but also works at start of scenario.
        /// </summary>
        /// <param name="condition">An XmlNode to store the condition</param>
        /// <param name="triggerInfo">A TriggerInfo object containing the required information for creating the condition</param
        public void RelativeDistanceCondition(XmlNode condition, TriggerInfo triggerInfo)
        {
            XmlNode byEntityCondition = root.CreateElement("ByEntityCondition");
            XmlNode triggeringEntities = root.CreateElement("TriggeringEntities");
            SetAttribute("triggeringEntitiesRule", "any", triggeringEntities);
            XmlNode entityRef = root.CreateElement("EntityRef");
            SetAttribute("entityRef", triggerInfo.EntityRef, entityRef);

            XmlNode entityCondition = root.CreateElement("EntityCondition");
            XmlNode relativeDistanceCondition = root.CreateElement("RelativeDistanceCondition");
            SetAttribute("entityRef", triggerInfo.EntitySelf, relativeDistanceCondition);
            SetAttribute("relativeDistanceType", "cartesianDistance", relativeDistanceCondition);
            SetAttribute("value", triggerInfo.Value.ToString(), relativeDistanceCondition);
            SetAttribute("freespace", "false", relativeDistanceCondition);
            SetAttribute("rule", "lessThan", relativeDistanceCondition);
            //SetAttribute("tolerance", triggerInfo.Value.ToString(), reachPositionCondition); 

            // hierarchy
            condition.AppendChild(byEntityCondition);
            byEntityCondition.AppendChild(triggeringEntities);
            byEntityCondition.AppendChild(entityCondition);
            triggeringEntities.AppendChild(entityRef);
            entityCondition.AppendChild(relativeDistanceCondition);
        }
        
        /// <summary>
        /// Create DistanceCondition. Triggers when specified entity traveled specified distance.
        /// Same as DistanceCondition but simpler to read.
        /// </summary>
        /// <param name="condition">An XmlNode to store the condition</param>
        /// <param name="triggerInfo">A TriggerInfo object containing the required information for creating the condition</param>
        public void ReachPositionCondition(XmlNode condition, TriggerInfo triggerInfo)
        {
            XmlNode byEntityCondition = root.CreateElement("ByEntityCondition");
            XmlNode triggeringEntities = root.CreateElement("TriggeringEntities");
            SetAttribute("triggeringEntitiesRule", "any", triggeringEntities);
            XmlNode entityRef = root.CreateElement("EntityRef");
            SetAttribute("entityRef", triggerInfo.EntityRef, entityRef);

            XmlNode entityCondition = root.CreateElement("EntityCondition");
            XmlNode reachPositionCondition = root.CreateElement("ReachPositionCondition");
            SetAttribute("tolerance", "5", reachPositionCondition); 
            //SetAttribute("tolerance", triggerInfo.Value.ToString(), reachPositionCondition); 
            
            XmlNode position = root.CreateElement("Position");
            XmlNode worldposition = root.CreateElement("WorldPosition");
            SetAttribute("x", triggerInfo.WorldPositionCarla.Vector3Ser.ToVector3().x.ToString(), worldposition);
            SetAttribute("y", triggerInfo.WorldPositionCarla.Vector3Ser.ToVector3().y.ToString(), worldposition);
            SetAttribute("z", triggerInfo.WorldPositionCarla.Vector3Ser.ToVector3().z.ToString(), worldposition);
            SetAttribute("h", triggerInfo.WorldPositionCarla.Rot.ToString(), worldposition);

            // hierarchy
            condition.AppendChild(byEntityCondition);
            byEntityCondition.AppendChild(triggeringEntities);
            byEntityCondition.AppendChild(entityCondition);
            triggeringEntities.AppendChild(entityRef);
            entityCondition.AppendChild(reachPositionCondition);
            reachPositionCondition.AppendChild(position);
            position.AppendChild(worldposition);
        }

        /// <summary>
        /// Create StandStillCondition. Triggers when specified entity does not move for a specific time.
        /// </summary>
        /// <param name="condition">An XmlNode to store the condition</param>
        /// <param name="triggerInfo">A TriggerInfo object containing the required information for creating the condition</param>
        public void StandStillCondition(XmlNode condition, TriggerInfo triggerInfo)
        {
            XmlNode byEntityCondition = root.CreateElement("ByEntityCondition");
            XmlNode triggeringEntities = root.CreateElement("TriggeringEntities");
            SetAttribute("triggeringEntitiesRule", "any", triggeringEntities);
            XmlNode entityRef = root.CreateElement("EntityRef");
            SetAttribute("entityRef", triggerInfo.EntityRef, entityRef);

            XmlNode entityCondition = root.CreateElement("EntityCondition");
            XmlNode standStillCondition = root.CreateElement("StandStillCondition");
            SetAttribute("duration", triggerInfo.Value.ToString(), standStillCondition);

            // hierarchy
            condition.AppendChild(byEntityCondition);
            byEntityCondition.AppendChild(triggeringEntities);
            byEntityCondition.AppendChild(entityCondition);
            triggeringEntities.AppendChild(entityRef);
            entityCondition.AppendChild(standStillCondition);
        }

        /// <summary>
        /// Trigger that is true if another Action is completed. Useful to create follow up actions.
        /// </summary>
        /// <param name="condition">An XmlNode to store the condition</param>
        /// <param name="triggerInfo">A TriggerInfo object containing the required information for creating the condition</param>
        public void StoryboardElementStateCondition(XmlNode condition, TriggerInfo triggerInfo)
        {
            XmlNode byValueCondition = root.CreateElement("ByValueCondition");
            condition.AppendChild(byValueCondition);
            XmlNode storyboardElementStateCondition = root.CreateElement("StoryboardElementStateCondition");
            SetAttribute("storyboardElementType", "action", storyboardElementStateCondition);
            SetAttribute("storyboardElementRef", triggerInfo.AfterAction.Name + triggerInfo.AfterAction.ID, storyboardElementStateCondition);
            SetAttribute("state", "completeState", storyboardElementStateCondition);
            byValueCondition.AppendChild(storyboardElementStateCondition);
        }

        /// <summary>
        /// Creates a group of CriteriaConditions and appends it to the given stopTrigger XmlNode.
        /// </summary>
        /// <param name="stopTrigger">An XmlNode to which the created ConditionGroup will be appended</param>
        public void CriteriaConditions(XmlNode stopTrigger)
        {
            XmlNode conditionGroup = root.CreateElement("ConditionGroup");

            CriteriaCondition(conditionGroup, "criteria_RunningStopTest", "", "");
            CriteriaCondition(conditionGroup, "criteria_RunningRedLightTest", "", "");
            CriteriaCondition(conditionGroup, "criteria_WrongLaneTest", "", "");
            CriteriaCondition(conditionGroup, "criteria_OnSidewalkTest", "", "");
            CriteriaCondition(conditionGroup, "criteria_KeepLaneTest", "", "");
            CriteriaCondition(conditionGroup, "criteria_CollisionTest", "", "");
            CriteriaCondition(conditionGroup, "criteria_DrivenDistanceTest", "distance_success", "1");

            stopTrigger.AppendChild(conditionGroup);
        }

        /// <summary>
        /// Creates a CriteriaCondition XmlNode and appends it to the given conditionGroup XmlNode.
        /// </summary>
        /// <param name="conditionGroup">An XmlNode to which the created CriteriaCondition will be appended</param>
        /// <param name="name">The name of the CriteriaCondition</param>
        /// <param name="parameterRef">The parameter reference for the ParameterCondition XmlNode</param>
        /// <param name="value">The value for the ParameterCondition XmlNode</param>
        private void CriteriaCondition(XmlNode conditionGroup, string name, string parameterRef, string value)
        {
            XmlNode condition = root.CreateElement("Condition");
            SetAttribute("name", name, condition);
            SetAttribute("delay", "0", condition);
            SetAttribute("conditionEdge", "rising", condition);
            XmlNode byValueCondition = root.CreateElement("ByValueCondition");
            XmlNode parameterCondition = root.CreateElement("ParameterCondition");
            SetAttribute("parameterRef", parameterRef, parameterCondition);
            SetAttribute("value", value, parameterCondition);
            SetAttribute("rule", "lessThan", parameterCondition);

            conditionGroup.AppendChild(condition);
            condition.AppendChild(byValueCondition);
            byValueCondition.AppendChild(parameterCondition);
        }


        /// <summary>
        /// Creates and appends an XmlAttribute to the given XmlNode element.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        /// <param name="element">The XmlNode element to which the attribute will be appended</param>
        private void SetAttribute(string name, string value, XmlNode element)
        {
            XmlAttribute attribute = root.CreateAttribute(name);
            attribute.Value = value;
            element.Attributes.Append(attribute);
        }
    }
}

public static class Helper
{

    /// <summary>
    /// Converts the first character of a string to lowercase, if it is an uppercase character.
    /// </summary>
    /// <param name="str">The input string</param>
    /// <returns>A string with the first character in lowercase, if it was uppercase; otherwise, the original string</returns
    public static string? FirstCharToLowerCase(this string? str)
    {
        if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
            return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];

        return str;
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