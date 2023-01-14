using System;
using System.Collections.Generic;
using System.Linq;

namespace Entity
{
    public class Path // Story in .xosc
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

            //TODO: Fix Position set only to 0,0,0,0 now for presentation
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
        public void InitAssignRouteWaypoint()
        {
            Waypoint assignRouteWaypoint = new Waypoint(
                    WaypointList[0].Location,
                    new ActionType("AssignRouteAction", GetRouteLocations()),
                    new List<TriggerInfo>() { new TriggerInfo("SimulationTimeCondition", 0, "greaterThan") });

            // only update if there are changes (without this check we'll have multiple AssignRouteActions for the same entity)
            if (!this.WaypointList[0].Location.Equals(assignRouteWaypoint.Location)
                || this.AssignRouteWaypoint is null)
            {
                AssignRouteWaypoint = assignRouteWaypoint;
                WaypointList.Insert(0, AssignRouteWaypoint);
            }
        }
    }
}
