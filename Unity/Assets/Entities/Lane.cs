using System;
using System.Collections.Generic;

namespace Entity
{
    [Serializable]
    public class Lane
    {
        public Lane()
        {
        }

        public Lane(int id, int roadId, float angularOffset = 0)
        {
            Id = id;
            RoadId = roadId;
            AngularOffset = angularOffset;
        }

        public Lane(int id, int roadId, List<(int, int)> nextRoadAndLaneIds, List<(int, int)> physicalNextRoadAndLaneIds, float angularOffset = 0)

        {
            Id = id;
            RoadId = roadId;
            NextRoadAndLaneIds = nextRoadAndLaneIds;
            PhysicalNextRoadAndLaneIds = physicalNextRoadAndLaneIds;
            AngularOffset = angularOffset;
        }

        public Lane(int id, int roadId, List<Waypoint> waypoints, List<(int, int)> nextRoadAndLaneIds, float angularOffset = 0)
        {
            Id = id;
            RoadId = roadId;
            Waypoints = waypoints;
            NextRoadAndLaneIds = nextRoadAndLaneIds;
            AngularOffset = angularOffset;
        }

        public int Id { get; set; }

        public int RoadId { get; set; }

        public List<Waypoint> Waypoints { get; set; }

        public List<(int, int)> NextRoadAndLaneIds { get; set; }

        public List<(int, int)> PhysicalNextRoadAndLaneIds { get; set; }

        public float AngularOffset { get; set; }

    }
}
