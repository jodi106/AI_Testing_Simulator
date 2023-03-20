using UnityEngine;
using System;

namespace Entity
{
    [Serializable]
    public class TriggerInfo : ICloneable
    /// <summary>Contains information about the Trigger of an ActionType in a Waypoint Object.</summary>
    {
        private static int autoIncrementId = 0;
        
        public TriggerInfo()
        {

        }

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
            CalculateLocationCarla();
            Delay = delay;
            ConditionEdge = conditionEdge;
        }

        public TriggerInfo(string triggerType, string entityRef, double value, Location worldPosition, string conditionEdge = "rising")
        /// Constructor for "ReachPositionCondition"
        {
            ID = autoIncrementId++;
            TriggerType = triggerType;
            EntityRef = entityRef;
            Value = value;
            WorldPosition = worldPosition;
            ConditionEdge = conditionEdge;
            CalculateLocationCarla();
        }

        public TriggerInfo(string triggerType, string entityRef, double duration, double delay = 0, string conditionEdge = "rising")
        /// for StandStillCondition
        {
            ID = autoIncrementId++;
            TriggerType = triggerType;
            EntityRef = entityRef;
            Value = duration;
            Delay = delay;
            ConditionEdge = conditionEdge;
        }

        public TriggerInfo(string triggerType, ActionType afterAction, string state = "completeState", double delay = 0, string conditionEdge = "rising")
        /// for StoryboardElementStateCondition
        {
            ID = autoIncrementId++;
            TriggerType = triggerType; // "examples: SimulationTimeCondition", "DistanceCondition"
            AfterAction = afterAction; // use ActionType.Name + ActionType.ID
            Delay = delay;
            ConditionEdge = conditionEdge;
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
        public Location WorldPositionCarla { get;set; } 
        public ActionType AfterAction { get; set; }  // corresponds to a previously executed ActionType

        public void CalculateLocationCarla()
        {
            (float xCarla, float yCarla) = RoadPieceController.UnityToCarla(WorldPosition.X, WorldPosition.Y);
            this.WorldPositionCarla = new Location(new Vector3(xCarla, yCarla, 0.3f));
        }

        public object Clone()
        {
            var cloneTriggerInfo = new TriggerInfo();
            cloneTriggerInfo.ID = this.ID;
            cloneTriggerInfo.TriggerType = String.Copy(this.TriggerType);
            cloneTriggerInfo.Delay = this.Delay;
            cloneTriggerInfo.ConditionEdge = String.Copy(this.ConditionEdge);

            cloneTriggerInfo.EntityRef = String.IsNullOrEmpty(this.EntityRef) ? String.Empty : string.Copy(this.EntityRef); //Value

            cloneTriggerInfo.SimulationTimeValue = this.SimulationTimeValue;
            cloneTriggerInfo.Value = this.Value;
            cloneTriggerInfo.Rule = String.Copy(this.Rule);

            cloneTriggerInfo.WorldPosition = new();
            if (this.WorldPosition != null)
               cloneTriggerInfo.WorldPosition = (Location)this.WorldPosition.Clone();

            cloneTriggerInfo.WorldPositionCarla = new();
            if (this.WorldPositionCarla != null)
                cloneTriggerInfo.WorldPositionCarla = (Location)this.WorldPositionCarla.Clone();

            this.AfterAction = new();
            if (this.AfterAction != null)
                cloneTriggerInfo.AfterAction = (ActionType)this.AfterAction.Clone();

            

            return cloneTriggerInfo;
        }
    }
}
