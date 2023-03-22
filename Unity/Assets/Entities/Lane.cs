using System;
using System.Collections.Generic;



/**
*The Lane class represents a lane of a road.
*It contains information such as the lane ID, road ID, waypoints, and next road and lane IDs.
*/
namespace Entity
{
    [Serializable]
    /// <summary>
    /// Represents a lane on a road network.
    //It contains information such as the lane ID, road ID, waypoints, and next road and lane IDs.
    /// </summary>
    public class Lane
    {

        /// <summary>
        /// Initializes a new instance of the Lane class.
        /// </summary>
        public Lane()
        {

        }

        /// <summary>
        /// Initializes a new instance of the Lane class with the specified ID and road ID.
        /// </summary>
        /// <param name="id">The ID of the lane.</param>
        /// <param name="roadId">The ID of the road the lane belongs to.</param>
        public Lane(int id, int roadId)
        {
            Id = id;
            RoadId = roadId;
        }

        /// <summary>
        /// Initializes a new instance of the Lane class with the specified ID, road ID, next road and lane IDs, and physical next road and lane IDs.
        /// </summary>
        /// <param name="id">The ID of the lane.</param>
        /// <param name="roadId">The ID of the road the lane belongs to.</param>
        /// <param name="nextRoadAndLaneIds">A list of tuples representing the IDs of the next road and lane the vehicle can move to from this lane.</param>
        /// <param name="physicalNextRoadAndLaneIds">A list of tuples representing the IDs of the next physical road and lane the vehicle can move to from this lane.</param>
        public Lane(int id, int roadId, List<(int, int)> nextRoadAndLaneIds, List<(int, int)> physicalNextRoadAndLaneIds)
        {
            Id = id;
            RoadId = roadId;
            NextRoadAndLaneIds = nextRoadAndLaneIds;
            PhysicalNextRoadAndLaneIds = physicalNextRoadAndLaneIds;
        }


        /// <summary>
        /// Initializes a new instance of the Lane class with the specified ID, road ID, waypoints, and next road and lane IDs.
        /// </summary>
        /// <param name="id">The ID of the lane.</param>
        /// <param name="roadId">The ID of the road the lane belongs to.</param>
        /// <param name="waypoints">A list of AStarWaypoints representing the path of the lane.</param>
        /// <param name="nextRoadAndLaneIds">A list of tuples representing the IDs of the next road and lane the vehicle can move to from this lane.</param>
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
