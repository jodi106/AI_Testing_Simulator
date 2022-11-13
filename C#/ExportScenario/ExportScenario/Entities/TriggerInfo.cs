using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class TriggerInfo
    /// Contains information regarding the Trigger of an ActionType in a Waypoint
    {
        private static int autoIncrementId = 0;

        // Constructor for "SimulationTimeCondition"
        public TriggerInfo(string triggerType, int simulationTime, string rule, double delay=0, string conditionEdge="rising")
        {
            ID = autoIncrementId++;
            TriggerType = triggerType; 
            Delay = delay;
            ConditionEdge = conditionEdge;
            SimulationTime = simulationTime;
            Rule = rule;
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
        public int SimulationTime { get; set; }
        public string Rule {  get; set; } // "equalTo" , "greaterThan" , "lessThan"
        public ActionType AfterAction { get; set; }  // corresponds to a previously executed ActionType
    }
}
