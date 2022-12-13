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
                    new Location(0,0,0,0), 
                    new ActionType("AssignRouteAction", GetRouteLocations()),
                    new List<TriggerInfo>() { new TriggerInfo("SimulationTimeCondition", 0, "greaterThan")});
            WaypointList.Insert(0,AssignRouteWaypoint);
        }

        public Waypoint OverallStartTrigger { get; set; } // A Waypoint Object containing Info regarding the overall start trigger of an act
        public Waypoint OverallStopTrigger { get; set; }
        public List<Waypoint> WaypointList { get; set; }

        public Waypoint AssignRouteWaypoint { get; set; }

        public List<Location> GetRouteLocations()
        /// Creates a List of all Waypoint.Locations in the Path to define the Entities Route via a AssignRouteAction
        {
            return WaypointList.Where(w => w.ActionTypeInfo is not null).Select(w => w.Location).ToList();
        }

        public List<Location> GetAllLocations()
        {
            return WaypointList.Select(w => w.Location).ToList();
        }
    }
}
