using UnityEngine;
using System;

namespace Entity
{
    [Serializable]

    /// <summary>
    /// Contains information about the Trigger of an ActionType in a Waypoint Object.
    /// </summary>
    public class TriggerInfo : ICloneable
    {
        private static int autoIncrementId = 0;
        

        /// <summary>
        /// Initializes a new empty instance of the <see cref="TriggerInfo"/> class.
        /// </summary>
        public TriggerInfo()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerInfo"/> class for "SimulationTimeCondition".
        /// </summary>
        /// <param name="triggerType">The type of trigger.</param>
        /// <param name="simulationTimeValue">The simulation time value.</param>
        /// <param name="rule">The rule to compare the trigger value.</param>
        /// <param name="delay">The delay in seconds for the trigger.</param>
        /// <param name="conditionEdge">The condition edge to trigger ("rising" or "falling").</param>
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


        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerInfo"/> class for "DistanceCondition".
        /// </summary>
        /// <param name="triggerType">The type of trigger.</param>
        /// <param name="entityRef">The entity reference.</param>
        /// <param name="rule">The rule to compare the trigger value.</param>
        /// <param name="value">The distance value.</param>
        /// <param name="worldPosition">The world position to calculate the distance.</param>
        /// <param name="delay">The delay in seconds for the trigger.</param>
        /// <param name="conditionEdge">The condition edge to trigger ("rising" or "falling").</param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerInfo"/> class for "RelativeDistanceCondition".
        /// </summary>
        /// <param name="triggerType">The type of trigger.</param>
        /// <param name="entityRef">The entity reference.</param>
        /// <param name="entitySelf">The entity self reference.</param>
        /// <param name="value">The distance value.</param>
        /// <param name="conditionEdge">The condition edge to trigger ("rising" or "falling").</param>
        public TriggerInfo(string triggerType, string entityRef, string entitySelf, double value, string conditionEdge = "rising")
        /// Constructor for "RelativeDistanceCondition"
        {
            ID = autoIncrementId++;
            TriggerType = triggerType;
            EntityRef = entityRef;
            EntitySelf = entitySelf;
            Value = value;
            ConditionEdge = conditionEdge;
            CalculateLocationCarla();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerInfo"/> class for "ReachPositionCondition".
        /// </summary>
        /// <param name="triggerType">The type of trigger.</param>
        /// <param name="entityRef">The entity reference.</param>
        /// <param name="value">The distance value.</param>
        /// <param name="worldPosition">The world position to reach.</param>
        /// <param name="conditionEdge">The condition edge to trigger ("rising" or "falling").</param>
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

        /// <summary>
        /// Creates a new TriggerInfo object for StandStillCondition.
        /// </summary>
        /// <param name="triggerType">The type of trigger.</param>
        /// <param name="entityRef">The entity reference.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="delay">The delay. Defaults to 0.</param>
        /// <param name="conditionEdge">The condition edge. Defaults to "rising".</param>
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

        /// <summary>
        /// Initializes a new instance of the TriggerInfo class.
        /// </summary>
        /// <param name="triggerType">The type of trigger for the StoryboardElementStateCondition.</param>
        /// <param name="afterAction">The action to perform after the trigger occurs.</param>
        /// <param name="state">The complete state of the StoryboardElementStateCondition.</param>
        /// <param name="delay">The amount of delay before the trigger occurs.</param>
        /// <param name="conditionEdge">The edge (rising or falling) of the StoryboardElementStateCondition.</param>
        /// <returns>A new instance of the TriggerInfo class.</returns>
        public TriggerInfo(string triggerType, ActionType afterAction, string state = "completeState", double delay = 0, string conditionEdge = "rising")
        /// for StoryboardElementStateCondition. string triggerType must be StoryboardElementStateCondition
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
        public string EntitySelf {  get; set; } // only relevant for RelativeDistanceCondition
        public double SimulationTimeValue { get; set; } // double: 0 to infinitive, unit: seconds, atm only needed for TriggerType = "SimulationTimeCondition"
        public double Value {  get; set; } // 0 to infinitive, unit: seconds, atm needed for DistanceCondition
        public string Rule {  get; set; } // has enum: "equalTo" , "greaterThan" , "lessThan"
        public Location WorldPosition { get;set; } 
        public Location WorldPositionCarla { get;set; } 
        public ActionType AfterAction { get; set; }  // corresponds to a previously executed ActionType


        /// <summary>
        /// Converts the Unity Coordinates to CARLA Coordinates using the UnityToCarla Conversion Function and fills the WorldPositionCarla Attribute with it
        /// </summary>
        public void CalculateLocationCarla()
        {
            if (WorldPosition == null) return;
            (float xCarla, float yCarla) = SnapController.UnityToCarla(WorldPosition.X, WorldPosition.Y);
            this.WorldPositionCarla = new Location(new Vector3(xCarla, yCarla, 0.3f));
        }

        /// <summary>
        /// Creates a new object that is a deepcopy of the current instance of the TriggerInfo class.
        /// </summary>
        /// <returns>A new instance of the TriggerInfo class that is a deepcopy of this instance.</returns>
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
