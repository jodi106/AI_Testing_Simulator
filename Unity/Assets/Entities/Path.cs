using System;
using System.Collections.Generic;

namespace Entities
{
    public class Path
    {
        public Path()
        {
        }

        public Path(List<Event> eventList)
        {
            EventList = eventList;
        }

        public List<Event> EventList { get; }

        public Event getEventById(int id)
        {
            throw new NotImplementedException();
        }
    }

}