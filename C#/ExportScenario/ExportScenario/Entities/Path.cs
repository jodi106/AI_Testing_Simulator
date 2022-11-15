using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Path // Story in .xosc
    {
        public Path()
        {
            EventList = new List<Waypoint>();
            RoutePositions = new List<Coord3D>();
        }

        public Path(List<Waypoint> eventList, Waypoint overallStartTrigger = null, Waypoint overallStopTrigger = null)
        {
            OverallStartTrigger = overallStartTrigger;
            OverallStopTrigger = overallStopTrigger;
            EventList = eventList;
            RoutePositions = getRoutePositions();
        }

        public Waypoint OverallStartTrigger { get; set; } // A Waypoint Object containing Info regarding the overall start trigger of an act
        public Waypoint OverallStopTrigger { get; set; }
        public List<Waypoint> EventList { get; set; }
        public List<Coord3D> RoutePositions { get; set; }


        public Waypoint getEventById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Coord3D> getRoutePositions()
        {
            List<Coord3D> routePositions = new List<Coord3D>();
            for (int i = 0; i < EventList.Count; i++)
            {
                routePositions.Add(EventList[i].Position);
            }

            return routePositions;
        }
    }
}
