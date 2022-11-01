using System;
using System.Collections.Generic;

namespace Entities
{
    public class Path
    {
        public Path()
        {
        }

        public Path(List<Waypoint> eventList)
        {
            EventList = eventList;
        }

        public List<Waypoint> EventList { get; }

        public Waypoint getEventById(int id)
        {
            throw new NotImplementedException();
        }
    }

}