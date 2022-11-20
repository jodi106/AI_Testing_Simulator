using Entities;
using System.Collections.Generic;

namespace Models
{
    public class Waypoint
    {
        public Waypoint(int id, Coord position, List<BaseEntity> involvedEntities, string actionType)
        {
            Id = id;
            Position = position;
            InvolvedEntities = involvedEntities;
            ActionType = actionType;
            snap = false;
        }

        public int Id { get; set; }
        public Coord Position { get; set; }
        public List<BaseEntity> InvolvedEntities { get; set; }
        public string ActionType { get; set; }
        // determines whether the waypoint should be a waypoint that defines the vehicles path (in the GUI)
        // TODO: find better name
        public bool snap;
    }
}
