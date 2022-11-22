using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class TriggerInfo
    /// <summary>Contains information about the Trigger of an ActionType in a Waypoint Object.</summary>
    {
        private static int autoIncrementId = 0;
               
        public TriggerInfo(string triggerType, int simulationTime, string rule, double delay=0, string conditionEdge="rising")
        /// Constructor for "SimulationTimeCondition"
        {
            ID = autoIncrementId++;
            TriggerType = triggerType; 
            SimulationTime = simulationTime;
            Rule = rule;
            Delay = delay;
            ConditionEdge = conditionEdge;
        }

        public TriggerInfo(string triggerType, string entityRef, string rule, double value, Coord3D worldPosition, double delay = 0, string conditionEdge = "rising")
        /// Constructor for "DistanceCondition"
        {
            ID = autoIncrementId++;
            TriggerType = triggerType;
            EntityRef = entityRef;
            Rule = rule;
            Value = value;
            WorldPosition = worldPosition;
            Delay = delay;
            ConditionEdge = conditionEdge;
        }

        public TriggerInfo(string triggerType, double delay, string conditionEdge, ActionType afterAction)
        /// for StoryboardElementStateCondition
        {
            ID = autoIncrementId++;
            TriggerType = triggerType; // "examples: SimulationTimeCondition", "DistanceCondition"
            Delay = delay;
            ConditionEdge = conditionEdge;
            AfterAction = afterAction; // use ActionType.Name + ActionType.ID
        }

        public int ID { get; set; }
        public string TriggerType { get; set; } // "examples: SimulationTimeCondition", "DistanceCondition"
        public double Delay { get; set; } // default: 0
        public string ConditionEdge { get; set; } // default: "rising"
        public string EntityRef {  get; set; }
        public double SimulationTime { get; set; }
        public double Value {  get; set; }
        public string Rule {  get; set; } // "equalTo" , "greaterThan" , "lessThan"
        public Coord3D WorldPosition { get;set; } 
        public ActionType AfterAction { get; set; }  // corresponds to a previously executed ActionType
    }
}
