using System.Collections.Generic;


namespace Entity
{ 
    public class Waypoint : BaseEntity// Event in .xosc
    /// <summary>Create Waypoint Object. Contains User defined Input for a specific Event on a Entity Path</summary>
    {
        public Waypoint(Location location)
        {
            Id = -1;
            Location = location;
        }

        public Waypoint(Location location, ActionType actionTypeInfo, List<TriggerInfo> triggerList, string priority = "overwrite")
        {
            Id = -1;
            Location = location;
            ActionTypeInfo = actionTypeInfo;
            Priority = priority;
            TriggerList = triggerList;
        }

        public Location Location { get; set; }
        public ActionType ActionTypeInfo { get; set; } 
        public string Priority { get; set; } // has enum: PriorityType
        public List<TriggerInfo> TriggerList { get; set; }
        // One Waypoint can have mutliple triggers for an event

    }

    public class AStarWaypoint : Waypoint
    {
        public AStarWaypoint(int indexInLane, int laneId, Location location, ActionType actionTypeInfo, List<TriggerInfo> triggerList, string priority = "overwrite") : 
            base(location, actionTypeInfo, triggerList, priority)
        {
            IndexInLane = indexInLane;
            LaneId = laneId;
        }

        public AStarWaypoint(int indexInLane, int laneId, Location location) : base(location)
        {
            IndexInLane = indexInLane;
            LaneId = laneId;
        }

        public int IndexInLane { get; set; }

        public int LaneId { get; set; }
    }
}