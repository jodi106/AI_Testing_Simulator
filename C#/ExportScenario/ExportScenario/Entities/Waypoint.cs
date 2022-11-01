using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Waypoint
    {
        public Waypoint(int id, Coord position, List<BaseEntity> involvedEntities, string actionType)
        {
            Id = id;
            Position = position;
            InvolvedEntities = involvedEntities;
            ActionType = actionType;
        }

        public int Id { get; set; }
        public Coord Position { get; set; }
        public List<BaseEntity> InvolvedEntities { get; set; }
        public string ActionType { get; set; }
    }
}
