using System;
using System.Collections.Generic;

namespace Entity
{
    public class Path // Story in .xosc
    /// <summary>Creates Path object. Contains Actions-info for a specific Entity created by Gui-User.</summary>
    {
        public Path()
        {
            EventList = new List<Waypoint>();
            //RoutePositions = new List<Coord3D>();

             
        }

        public Path(List<Waypoint> eventList, Waypoint overallStartTrigger = null, Waypoint overallStopTrigger = null)
        {
            OverallStartTrigger = overallStartTrigger;
            OverallStopTrigger = overallStopTrigger;
            EventList = eventList;
            AssignRouteWaypoint = new Waypoint(
                    null, 
                    new ActionType("AssignRouteAction", getRoutePositions()),
                    new List<TriggerInfo>() { new TriggerInfo("SimulationTimeCondition", 0, "greaterThan")});
            EventList.Insert(0,AssignRouteWaypoint);
        }

        public Waypoint OverallStartTrigger { get; set; } // A Waypoint Object containing Info regarding the overall start trigger of an act
        public Waypoint OverallStopTrigger { get; set; }
        public List<Waypoint> EventList { get; set; }

        public Waypoint AssignRouteWaypoint { get; set; }

        public List<Location> getRoutePositions()
        /// Creates a List of all Waypoint.Positions in the Path to define the Entities Route via a AssignRouteAction
        {
            List<Location> routePositions = new List<Location>();
            for (int i = 0; i < EventList.Count; i++)
            {
                if (EventList[i].ActionTypeInfo != null)
                {
                    routePositions.Add(EventList[i].Position);
                }
            }

            return routePositions;
        }
    }
}
