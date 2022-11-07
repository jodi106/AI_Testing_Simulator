using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class TriggerInfo
    /// Contains information regarding the Trigger of an ActionType in a Waypoint
    {
        public TriggerInfo(string triggerType, int simulationTime = 1, Waypoint afterAction = null)
        {
            TriggerType = triggerType;
            SimulationTime = simulationTime;
            AfterAction = afterAction;
        }

        public string TriggerType { get; set; }
        public int SimulationTime { get; set; }
        public Waypoint AfterAction { get; set; }  // corresponds to a previously executed ActionType
    }
}
