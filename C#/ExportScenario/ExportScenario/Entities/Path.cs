using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Path
    {
        

        public Path()
        {
            EventList = new List<Waypoint>();
        }

        public Path(TriggerInfo overallStartTrigger, List<Waypoint> eventList)
        {
            OverallStartTrigger = overallStartTrigger;
            EventList = eventList;
        }

        public TriggerInfo OverallStartTrigger { get; set; }
        public List<Waypoint> EventList { get; set; }
        

        public Waypoint getEventById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
