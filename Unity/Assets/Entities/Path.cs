using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entity
{
    [Serializable]
    public class Path : ICloneable// Story in .xosc
    /// <summary>Creates Path object. Contains Actions-info for a specific Entity created by Gui-User.</summary>
    {
        public Path()
        {
            WaypointList = new List<Waypoint>();
        }

        public Path(List<Waypoint> waypointList, Waypoint overallStartTrigger = null, Waypoint overallStopTrigger = null)
        {
            OverallStartTrigger = overallStartTrigger;
            OverallStopTrigger = overallStopTrigger;
            WaypointList = waypointList;

            AssignRouteWaypoint = new Waypoint(
                    WaypointList[0].Location,
                    new ActionType("AssignRouteAction", GetRouteLocations()),
                    new List<TriggerInfo>() { new TriggerInfo("SimulationTimeCondition", 0, "greaterThan")});
            WaypointList.Insert(0,AssignRouteWaypoint);
        }

        public Waypoint OverallStartTrigger { get; set; } // A Waypoint Object containing Info regarding the overall start trigger of an act
        public Waypoint OverallStopTrigger { get; set; }
        public List<Waypoint> WaypointList { get; set; }

        public Waypoint AssignRouteWaypoint { get; set; } // is first waypoint of WaypointList to create OpenScenario Event AssignRouteAction and corresponding Trigger

        public object Clone()
        {
            Path clonePath = new Path();

            clonePath.OverallStartTrigger = new Waypoint();
            if (this.OverallStartTrigger!= null) 
                clonePath.OverallStartTrigger = (Waypoint)this.OverallStartTrigger.Clone();
            
            clonePath.OverallStopTrigger = new Waypoint();
            if (this.OverallStopTrigger != null)
                clonePath.OverallStopTrigger = (Waypoint)this.OverallStopTrigger.Clone();

            clonePath.WaypointList = this.WaypointList.Select(x => (Waypoint)x.Clone()).ToList();

            return clonePath; 
        }

        public List<Location> GetRouteLocations()
        /// Creates a List of all Waypoint.Locations in the Path to define the Entities Route via a AssignRouteAction
        {
            return WaypointList.Where(w => w.ActionTypeInfo is not null).Select(w => w.Location).ToList();
        }

        public List<Location> GetAllLocations()
        {
            return WaypointList.Select(w => w.Location).ToList();
        }

        public bool IsEmpty()
        {
            return this.WaypointList.Count == 0;
        }

        // Needs to be invoked at the End after WayPointList is finished (so when ExportButton is pressed)
        // Creates Waypoint with the ActionType "AssignRouteAction" that contains all waypoint locations.
        public void InitAssignRouteWaypoint(Location spawnpoint)
        {
            var first = WaypointList[0];
            Quaternion rotation = Quaternion.Euler(0f, 0f, spawnpoint.Rot);
            var originalStartLocation = (Location) first.Location.Clone();
            first.Location.Vector3Ser.SetFromVector3(first.Location.Vector3Ser.ToVector3() + rotation * Vector3.right);

            var locations = GetRouteLocations();
            locations.Insert(0, spawnpoint); // To avoid having a AssignRouteAction with just 2 waypoints

            Waypoint assignRouteWaypoint = new Waypoint(
                    originalStartLocation,
                    new ActionType("AssignRouteAction", locations),
                    new List<TriggerInfo>() { new TriggerInfo("SimulationTimeCondition", 0, "greaterThan") });

            AssignRouteWaypoint = assignRouteWaypoint;
            WaypointList.Insert(0, AssignRouteWaypoint);
        }
    }
}
