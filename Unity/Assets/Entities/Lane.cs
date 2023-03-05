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

        public Lane(int id, int roadId, List<(int, int)> nextRoadAndLaneIds, List<(int, int)> physicalNextRoadAndLaneIds)
        {
            Id = id;
            RoadId = roadId;
            NextRoadAndLaneIds = nextRoadAndLaneIds;
            PhysicalNextRoadAndLaneIds = physicalNextRoadAndLaneIds;
        }

        public Lane(int id, int roadId, List<AStarWaypoint> waypoints, List<(int, int)> nextRoadAndLaneIds)
        {
            Id = id;
            RoadId = roadId;
            Waypoints = waypoints;
            NextRoadAndLaneIds = nextRoadAndLaneIds;
        }

        public int Id { get; set; }

        public int RoadId { get; set; }

        public List<AStarWaypoint> Waypoints { get; set; }

        public List<(int, int)> NextRoadAndLaneIds { get; set; }

        public List<(int, int)> PhysicalNextRoadAndLaneIds { get; set; }
    }
}
