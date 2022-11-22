using System.Collections.Generic;


namespace Entity
{ 
    public class Waypoint // Event in .xosc
    /// <summary>Create Waypoint Object. Contains User defined Input for a specific Event on a Entity Path</summary>
    {
        public Waypoint(int id, Location position, ActionType actionTypeInfo, List<TriggerInfo> triggerList, string priority = "overwrite")
        {
            Id = id;
            Position = position;
            ActionTypeInfo = actionTypeInfo;
            Priority = priority;
            TriggerList = triggerList;
        }

        public int Id { get; set; }
        public Location Position { get; set; }
        public ActionType ActionTypeInfo { get; set; } 
        public string Priority { get; set; } // has enum: PriorityType
        public List<TriggerInfo> TriggerList { get; set; }
        // One Waypoint can have mutliple triggers for an event

    }
}