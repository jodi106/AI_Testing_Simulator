using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
namespace Entity
{ 
    public class Waypoint : ICloneable// Event in .xosc
    /// <summary>Create Waypoint Object. Contains User defined Input for a specific Event on a Entity Path</summary>
    {
        public Waypoint(Location location)
        {
            Location = location;
        }

        public Waypoint(Location location, ActionType actionTypeInfo, List<TriggerInfo> triggerList, string priority = "overwrite")
        {
            Location = location;
            ActionTypeInfo = actionTypeInfo;
            Priority = priority;
            TriggerList = triggerList;
            CalculateLocationCarla();
        }

        public void setLocation(Location location)
        {
            this.Location = location;
            this.View?.onChangePosition(location);
            CalculateLocationCarla();
        }

        public Location Location { get; private set; } // coords of unity editor GUI
        public Location LocationCarla { get; set; } // coords of Carla. Used in Export xosc.
        public ActionType ActionTypeInfo { get; set; } 
        public string Priority { get; set; } // has enum: PriorityType
        public List<TriggerInfo> TriggerList { get; set; }
        // One Waypoint can have mutliple triggers for an event
        public IBaseView View { get; set; }
        //public int Id { get; set; }
        public List<ActionType> Actions { get; set; } // I don't need a TriggerList for them because all TriggerInfos are the same for these actions

        public void CalculateLocationCarla()
        {
            (float xCarla, float yCarla) = SnapController.UnityToCarla(Location.X, Location.Y);
            float rotCarla = SnapController.UnityRotToRadians(Location.Rot);
            rotCarla = (float)Math.Round(rotCarla * 100f) / 100f; // otherwise we'll have a number like this 3.339028E-05
            this.LocationCarla = new Location(xCarla, yCarla, 0.3f, rotCarla);
        }

        public object Clone()
        {
            var cloneWaypoint = new Waypoint((Location)this.Location.Clone());
            cloneWaypoint.LocationCarla = (Location)this.LocationCarla.Clone();
            cloneWaypoint.ActionTypeInfo = (ActionType)this.ActionTypeInfo.Clone();
            cloneWaypoint.TriggerList = this.TriggerList.Select(x => (TriggerInfo)x.Clone()).ToList();

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

        public int IndexInLane { get; set; }

        public int LaneId { get; set; }
    }
}