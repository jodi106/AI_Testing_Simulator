using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Path // Story in .xosc
    /// <summary>Creates Path object. Contains Actions-info for a specific Entity created by Gui-User.</summary>
    {
        public Path()
        {
            EventList = new List<Waypoint>();
            //RoutePositions = new List<Coord3D>();
             
        }

        public Path(List<Waypoint> eventList, ActionType assignRoute, Waypoint overallStartTrigger = null, Waypoint overallStopTrigger = null)
        {
            OverallStartTrigger = overallStartTrigger;
            OverallStopTrigger = overallStopTrigger;
            EventList = eventList;
            AssignRoute = assignRoute;
            AssignRouteWaypoint = new Waypoint(1, 
                    null, 
                    new ActionType("AssignRouteAction", getRoutePositions()),
                    new List<TriggerInfo>() { new TriggerInfo("SimulationTimeCondition", 0, "greaterThan")});
            EventList.Insert(0,AssignRouteWaypoint);
        }

        public Waypoint OverallStartTrigger { get; set; } // A Waypoint Object containing Info regarding the overall start trigger of an act
        public Waypoint OverallStopTrigger { get; set; }
        public List<Waypoint> EventList { get; set; }

        public List<Coord3D> getRoutePositions()
        /// Creates a List of all Waypoint.Positions in the Path to define the Entities Route via a AssignRouteAction
        {
            List<Coord3D> routePositions = new List<Coord3D>();
            for (int i = 0; i < EventList.Count; i++)
            {
                routePositions.Add(EventList[i].Position);
            }

            return routePositions;
        }
        public Waypoint getEventById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
