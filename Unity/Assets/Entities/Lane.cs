using System.Collections.Generic;

namespace Entity
{
    public class Lane
    {
        public Lane()
        {
        }

        public Lane(int id, int roadId)
        {
            Id = id;
            RoadId = roadId;
        }

        public Lane(int id, int roadId, List<(int, int)> nextRoadAndLaneIds)
        {
            Id = id;
            RoadId = roadId;
            NextRoadAndLaneIds = nextRoadAndLaneIds;
        }

        public Lane(int id, int roadId, List<LaneWaypoint> waypoints, List<(int, int)> nextRoadAndLaneIds)
        {
            Id = id;
            RoadId = roadId;
            Waypoints = waypoints;
            NextRoadAndLaneIds = nextRoadAndLaneIds;
        }

        public int Id { get; set; }

        public int RoadId { get; set; }

        public List<LaneWaypoint> Waypoints { get; set; }

        public List<(int, int)> NextRoadAndLaneIds { get; set; }
    }
}
