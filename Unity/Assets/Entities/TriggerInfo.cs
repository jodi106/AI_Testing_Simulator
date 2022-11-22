namespace Entity
{
    public class TriggerInfo
    /// <summary>Contains information about the Trigger of an ActionType in a Waypoint Object.</summary>
    {
        private static int autoIncrementId = 0;
               
        public TriggerInfo(string triggerType, double simulationTimeValue, string rule, double delay=0, string conditionEdge="rising")
        /// Constructor for "SimulationTimeCondition"
        {
            ID = autoIncrementId++;
            TriggerType = triggerType; 
            SimulationTimeValue = simulationTimeValue; // TODO change to Value?
            Rule = rule;
            Delay = delay;
            ConditionEdge = conditionEdge;
        }

        public TriggerInfo(string triggerType, string entityRef, string rule, double value, Location worldPosition, double delay = 0, string conditionEdge = "rising")
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
        public string TriggerType { get; set; } // has enum; can be extended; "examples: SimulationTimeCondition", "DistanceCondition"
        public double Delay { get; set; } // double: 0 to infinitive, unit: seconds, default: 0 (for most cases 0 fits)
        public string ConditionEdge { get; set; } // has enum; default: "rising"; only in advanced settings
        public string EntityRef {  get; set; } // example: "adversary2" --> "adversary"+id
        public double SimulationTimeValue { get; set; } // double: 0 to infinitive, unit: seconds, atm only needed for TriggerType = "SimulationTimeCondition"
        public double Value {  get; set; } // 0 to infinitive, unit: seconds, atm needed for DistanceCondition
        public string Rule {  get; set; } // has enum: "equalTo" , "greaterThan" , "lessThan"
        public Location WorldPosition { get;set; } 
        public ActionType AfterAction { get; set; }  // corresponds to a previously executed ActionType
    }
}
