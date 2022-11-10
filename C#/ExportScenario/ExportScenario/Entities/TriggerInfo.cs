using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class TriggerInfo
    /// Contains information regarding the Trigger of an ActionType in a Waypoint
    {
        private static int autoIncrementId = 0;
        public TriggerInfo(string triggerType, double delay, string conditionEdge, int simulationTime)
        {
            ID = autoIncrementId++;
            TriggerType = triggerType;
            Delay = delay;
            ConditionEdge = conditionEdge;
            SimulationTime = simulationTime;
        }

        public TriggerInfo(string triggerType, double delay, string conditionEdge, ActionType afterAction)
        /// for StoryboardElementStateCondition
        {
            ID = autoIncrementId++;
            TriggerType = triggerType;
            Delay = delay;
            ConditionEdge = conditionEdge;
            AfterAction = afterAction; // use ActionType.Name + ActionType.ID
        }

        public int ID { get; set; }
        public string TriggerType { get; set; }
        public double Delay { get; set; }
        public string ConditionEdge { get; set; }
        public int SimulationTime { get; set; }
        public ActionType AfterAction { get; set; }  // corresponds to a previously executed ActionType
    }
}
