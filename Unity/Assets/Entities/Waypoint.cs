using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
namespace Entity
{
    [Serializable]
    public class Waypoint : ICloneable// Event in .xosc
    /// <summary>Create Waypoint Object. Contains User defined Input for a specific Event on a Entity Path</summary>
    {
        public Waypoint(Location location)
        {
            Location = location;
            Actions = new List<ActionType>();
        }

        public Waypoint()
        {
        }

        public Waypoint(float x, float y)
        {
            Location = new Location(x, y);
        }

        public Waypoint(Location location, ActionType actionTypeInfo, List<TriggerInfo> triggerList, string priority = "overwrite")
        {
            Location = location;
            ActionTypeInfo = actionTypeInfo;
            Priority = priority;
            TriggerList = triggerList;
            CalculateLocationCarla();
            Actions = new List<ActionType>();
        }

        public void setPosition(float x, float y)
        {
            Location = new Location(x, y, Location.Z, Location.Rot);
            this.View?.onChangePosition(x, y);
            CalculateLocationCarla();
        }

        public Location Location { get; private set; } // coords of unity editor GUI
        public Location LocationCarla { get; set; } // coords of Carla. Used in Export xosc.
        public ActionType ActionTypeInfo { get; set; } 
        public string Priority { get; set; } // has enum: PriorityType
        public List<TriggerInfo> TriggerList { get; set; }
        // One Waypoint can have mutliple triggers for an event

        [field: NonSerialized]
        public IBaseView View { get; set; }
        public List<ActionType> Actions { get; set; } // Actions without Triggers

        public Adversary StartRouteOfOtherVehicle { get; set; }

        public void CalculateLocationCarla()
        {
            (float xCarla, float yCarla) = SnapController.UnityToCarla(Location.X, Location.Y);
            float rotCarla = SnapController.UnityRotToRadians(Location.Rot);
            rotCarla = (float)Math.Round(rotCarla * 100f) / 100f; // otherwise we'll have a number like this 3.339028E-05
            this.LocationCarla = new Location(xCarla, yCarla, 0.3f, rotCarla);
        }

        public object Clone()
        {
            var cloneWaypoint = new Waypoint();
            if (this.Location != null) cloneWaypoint.Location = (Location)this.Location.Clone();           
            if (this.LocationCarla != null) cloneWaypoint.LocationCarla = (Location)this.LocationCarla.Clone();
            if (this.ActionTypeInfo != null) cloneWaypoint.ActionTypeInfo = (ActionType)this.ActionTypeInfo.Clone();
            if (this.StartRouteOfOtherVehicle != null) cloneWaypoint.StartRouteOfOtherVehicle = (Adversary)this.StartRouteOfOtherVehicle.Clone();
            if (this.TriggerList != null) cloneWaypoint.TriggerList = this.TriggerList.Select(x => (TriggerInfo)x.Clone()).ToList();

            cloneWaypoint.Actions = new();
            if (this.Actions != null)           
                cloneWaypoint.Actions = this.Actions.Select(x => (ActionType)x.Clone()).ToList();
            
            return cloneWaypoint;
        }
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

        public AStarWaypoint(float x, float y) : base (x,y)
        {
        }

        public int IndexInLane { get; set; }

        public int LaneId { get; set; }
    }
}