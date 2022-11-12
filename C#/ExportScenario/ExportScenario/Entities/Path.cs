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
        }

        public Path(Waypoint overallStartTrigger, List<Waypoint> eventList, Waypoint overallStopTrigger = null)
        {
            OverallStartTrigger = overallStartTrigger;
            OverallStopTrigger = overallStopTrigger;
            EventList = eventList;
        }

        public Waypoint OverallStartTrigger { get; set; } // A Waypoint Object containing Info regarding the overall start trigger of an act
        public Waypoint OverallStopTrigger { get; set; }
        public List<Waypoint> EventList { get; set; }
        

        public Waypoint getEventById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
