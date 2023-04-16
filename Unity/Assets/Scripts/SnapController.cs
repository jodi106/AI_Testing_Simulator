using Entity;
using Newtonsoft.Json;
using PriorityQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// SnapController class handles loading and updating of road and waypoint data for the map.
/// </summary>
public class SnapController : MonoBehaviour
{
    public GameObject circlePrefab;

    public Dictionary<int, Road> roads;

    private Dictionary<Location, GameObject> waypointGameObjects;

    private static Dictionary<string, MapDimension> mapDimensions;

    private GameObject LastClickedWaypointGameObject;

    private static String mapName = "";

    // signals that an entity is currently selected so that waypoints can be placed on top of waypoints and paths of other entities
    public bool IgnoreClicks { get; set; }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {

        IgnoreClicks = false;

        roads = new();

        waypointGameObjects = new();

        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            var action = new MapChangeAction(x);

            mapName = action.Name;

            foreach (var (_, waypoint) in waypointGameObjects)
            {
                Destroy(waypoint);
            }

            waypointGameObjects.Clear();
            roads.Clear();
            LastClickedWaypointGameObject = null;

            if (action.Name != "") LoadRoads(action.Name);
        });

        var dimensions = Resources.Load<TextAsset>("waypoints/dimensions");

        mapDimensions = JsonConvert.DeserializeObject<Dictionary<string, MapDimension>>(dimensions.text);
    }

    /// <summary>
    /// Highlights the waypoint indicator nearest to the mouse position.
    /// </summary>
    public void Update()
    {
        if (MainController.freeze) return;

        if (LastClickedWaypointGameObject is not null)
        {
            var sprite = LastClickedWaypointGameObject.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            LastClickedWaypointGameObject.transform.position = HeightUtil.SetZ(LastClickedWaypointGameObject.transform.position, HeightUtil.WAYPOINT_INDICATOR);
        }

        var (_, waypoint) = FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (waypoint is not null)
        {
            var waypointGameObject = waypointGameObjects[waypoint.Location];

            if (waypointGameObject is not null)
            {
                waypointGameObject.GetComponent<SpriteRenderer>().color = Color.green;
                waypointGameObject.transform.position = HeightUtil.SetZ(waypointGameObject.transform.position, HeightUtil.WAYPOINT_INDICATOR_SELECTED);
                LastClickedWaypointGameObject = waypointGameObject;
            }
        }
    }

    /// <summary>
    /// Parses a lane ID string and returns the corresponding road ID and lane ID as a tuple.
    /// </summary>
    /// <param name="laneIdString">The lane ID string to parse.</param>
    /// <returns>Returns a tuple containing the road ID and lane ID.</returns>
    public (int, int) GetRoadIdAndLaneIdFromString(string laneIdString)
    {
        var laneIdSplit = laneIdString.Split("R")[1].Split("L");
        return (int.Parse(laneIdSplit[0]), int.Parse(laneIdSplit[1]));
    }

    /// <summary>
    /// Loads the road and waypoint data for a specified map.
    /// </summary>
    /// <param name="mapName">The name of the map to load road and waypoint data for.</param>
    void LoadRoads(string mapName)
    {
        roads = new Dictionary<int, Road>();

        var text = Resources.Load<TextAsset>("topology/" + mapName + "_topology");

        var jsonLanes = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text.text);

        foreach (var (laneIdString, nextRoadAndLaneIdStrings) in jsonLanes)
        {
            (var roadId, var laneId) = GetRoadIdAndLaneIdFromString(laneIdString);

            if (!roads.ContainsKey(roadId))
            {
                roads.Add(roadId, new Road(roadId, new SortedList<int, Lane>()));
            }

            var nextRoadAndLaneIds = new List<(int, int)>();
            var physicalNextRoadAndLaneIds = new List<(int, int)>();

            nextRoadAndLaneIdStrings.ForEach(nextString =>
                nextRoadAndLaneIds.Add(GetRoadIdAndLaneIdFromString(nextString))
            );

            roads[roadId].Lanes.Add(laneId, new Lane(laneId, roadId, nextRoadAndLaneIds, physicalNextRoadAndLaneIds));
        }

        foreach (var (roadId, road) in roads)
        {
            foreach (var (laneId, lane) in road.Lanes)
            {
                var nextPossibleRoadAndLaneIds = new List<(int, int)>();

                foreach ((var nextRoadId, var nextLaneId) in lane.NextRoadAndLaneIds)
                {
                    nextPossibleRoadAndLaneIds.Add((nextRoadId, nextLaneId));
                    lane.PhysicalNextRoadAndLaneIds.Add((nextRoadId, nextLaneId));

                    var nextRoad = roads[nextRoadId];

                    var nextLane = nextRoad.Lanes[nextLaneId];

                    foreach (var (neighbouringLaneId, _) in nextRoad.Lanes)
                    {
                        if ((neighbouringLaneId == nextLaneId - 1 || neighbouringLaneId == nextLaneId + 1) && neighbouringLaneId * nextLaneId > 0)
                        {
                            nextPossibleRoadAndLaneIds.Add((nextRoadId, neighbouringLaneId));
                        }
                    }
                }

                lane.NextRoadAndLaneIds = nextPossibleRoadAndLaneIds;
            }
        }

        text = Resources.Load<TextAsset>("Waypoints/" + mapName);

        var jsonLaneWithWaypoint = JsonConvert.DeserializeObject<Dictionary<string, List<JsonWaypoint>>>(text.text);

        foreach (var (laneIdString, jsonWaypoints) in jsonLaneWithWaypoint)
        {
            (var roadId, var laneId) = GetRoadIdAndLaneIdFromString(laneIdString);

            var waypoints = new List<AStarWaypoint>();

            var index = 0;

            foreach (var jsonWaypoint in jsonWaypoints)
            {
                var position = new Vector3(
                    ((jsonWaypoint.X - mapDimensions[mapName].MinX) - ((-mapDimensions[mapName].MinX + mapDimensions[mapName].MaxX) / 2)) / 4,
                    ((jsonWaypoint.Y - mapDimensions[mapName].MinY) - ((-mapDimensions[mapName].MinY + mapDimensions[mapName].MaxY) / 2)) / 4 * (-1),
                    HeightUtil.WAYPOINT_INDICATOR);
                var waypointGameObject = Instantiate(
                    circlePrefab,
                    position,
                    Quaternion.identity);

                waypointGameObject.transform.eulerAngles = Vector3.forward * (-jsonWaypoint.Rot);

                var location = new Location(waypointGameObject.transform.position, waypointGameObject.transform.eulerAngles.z);

                waypoints.Add(new AStarWaypoint(index++, laneId, location));

                waypointGameObjects.Add(location, waypointGameObject);
            }

            roads[roadId].Lanes[laneId].Waypoints = waypoints;

        }
    }

    /// <summary>
    /// Finds the nearest waypoint to a given mouse position.
    /// </summary>
    /// <param name="mousePosition">The Vector2 representing the mouse position.</param>
    /// <returns>Returns a Location object of the nearest waypoint.</returns>
    public Location FindWaypoint(Vector2 mousePosition)
    {
        (_, var waypoint) = FindLaneAndWaypoint(mousePosition);
        return waypoint?.Location;
    }


    /// <summary>
    /// Finds the closest Lane and AStarWaypoint to the given mouse position.
    /// </summary>
    /// <param name="mousePosition">A Vector2 representing the position of the mouse cursor.</param>
    /// <returns>A tuple containing the closest Lane and AStarWaypoint to the given mouse position.</returns
    public (Lane, AStarWaypoint) FindLaneAndWaypoint(Vector2 mousePosition)
    {
        //waypoints in Carla Coordinates, pos in World Coordinates (Pixel Coordinates / 100)
        AStarWaypoint closestWaypoint = null;
        Lane laneToReturn = null;

        double distance = double.MaxValue;

        foreach (var (_, road) in roads)
        {
            foreach (var (_, lane) in road.Lanes)
            {
                foreach (var waypoint in lane.Waypoints)
                {
                    double currDistance = FastEuclideanDistance(waypoint.Location.Vector3Ser.ToVector3(), mousePosition);

                    if (currDistance == 0) return (lane, waypoint);

                    else if (currDistance < distance)
                    {
                        closestWaypoint = waypoint;
                        distance = currDistance;
                        laneToReturn = lane;
                    }
                }
            }
        }

        return (laneToReturn, closestWaypoint);
    }


    /// <summary>
    /// Computes the fast Euclidean distance between two Vector2 points.
    /// </summary>
    /// <param name="a">A Vector2 representing the first point.</param>
    /// <param name="b">A Vector2 representing the second point.</param>
    /// <returns>A double representing the fast Euclidean distance between the two points.</returns>
    public static double FastEuclideanDistance(Vector2 a, Vector2 b)
    {
        return Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2);
    }

    /// <summary>
    /// Returns the neighboring lanes of a given Lane.
    /// </summary>
    /// <param name="l">A Lane object.</param>
    /// <returns>A tuple containing the previous and next lanes of the given Lane.</returns>
    public (Lane, Lane) GetNeighboringLanes(Lane l)
    {
        var r = roads[l.RoadId];

        var index = r.Lanes.IndexOfKey(l.Id);

        Lane prev = index > 0 ? r.Lanes.Values[index - 1] : null;
        Lane next = index < r.Lanes.Count - 1 ? r.Lanes.Values[index + 1] : null;

        return (prev, next);

    }

    /// <summary>
    /// Checks if a lane change is possible between the start and end lanes and waypoints.
    /// </summary>
    /// <param name="startLane">The starting Lane.</param>
    /// <param name="endLane">The ending Lane.</param>
    /// <param name="startWaypoint">The starting AStarWaypoint.</param>
    /// <param name="endWaypoint">The ending AStarWaypoint.</param>
    /// <returns>True if a lane change is possible, false otherwise.</returns
    public bool CheckLaneChange(Lane startLane, Lane endLane, AStarWaypoint startWaypoint, AStarWaypoint endWaypoint)
    {
        if ((startLane.RoadId == endLane.RoadId && endWaypoint.IndexInLane <= startWaypoint.IndexInLane)
            || endLane.Id * startLane.Id < 0 || FastEuclideanDistance(startWaypoint.Location.Vector3Ser.ToVector3(), endWaypoint.Location.Vector3Ser.ToVector3()) > 15)
        {
            return false;
        }

        (var left, var right) = GetNeighboringLanes(startLane);

        List<Lane> lanes = new List<Lane>();
        if (left is not null)
        {
            lanes.Add(left);
            foreach ((var roadIndex, var laneIndex) in left.NextRoadAndLaneIds)
            {
                lanes.Add(roads[roadIndex].Lanes[laneIndex]);
            }
        }
        if (right is not null)
        {
            lanes.Add(right);
            foreach ((var roadIndex, var laneIndex) in right.NextRoadAndLaneIds)
            {
                lanes.Add(roads[roadIndex].Lanes[laneIndex]);
            }
        }

        foreach (var lane in lanes)
        {
            if (endLane.RoadId == lane.RoadId && endWaypoint.LaneId == lane.Id)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Finds a path between the start and end positions, considering lane changes and optional waypoint ignoring.
    /// </summary>
    /// <param name="start">A Vector2 representing the starting position.</param>
    /// <param name="end">A Vector2 representing the ending position.</param>
    /// <param name="ignoreWaypoints">A boolean indicating whether to ignore waypoints and return a direct path.</param>
    /// <returns>A tuple containing a list of Vector2 waypoints and a list of integer indices representing lane changes in the path.</returns>
    public (List<Vector2>, List<int>) FindPath(Vector2 start, Vector2 end, bool ignoreWaypoints)
    {
        if (ignoreWaypoints)
        {
            return (new List<Vector2> { start, end }, new List<int>());
        }

        (Lane startLane, AStarWaypoint startWaypoint) = FindLaneAndWaypoint(start);
        (Lane endLane, AStarWaypoint endWaypoint) = FindLaneAndWaypoint(end);

        if (startLane == null || startWaypoint == null || endWaypoint == null)
        {
            throw new Exception("Invalid start or end coordinates");
        }

        if (CheckLaneChange(startLane, endLane, startWaypoint, endWaypoint))
        {
            return (new List<Vector2>() { startWaypoint.Location.Vector3Ser.ToVector3(), endWaypoint.Location.Vector3Ser.ToVector3() },
                new List<int>() { 0 });
        }

        var prioQueue = new SimplePriorityQueue<Lane, double>();

        var laneChangeIndices = new List<int>();

        prioQueue.Enqueue(startLane, 0);

        var cameFrom = new Dictionary<AStarWaypoint, AStarWaypoint>
        {
            { startWaypoint, null }
        };

        var costSoFar = new Dictionary<Lane, double> { };

        var laneChanges = new HashSet<AStarWaypoint>();
        var physicalLaneChanges = new HashSet<AStarWaypoint>();

        var firstIteration = true;

        AStarWaypoint currentWaypoint;


        while (prioQueue.Count != 0)
        {
            var currentLane = prioQueue.Dequeue();

            if (firstIteration)
            {
                // this is the only time, a path might not start at the start of the lane, maybe straight from the middle
                currentWaypoint = startWaypoint;
                firstIteration = false;
            }
            else
            {
                currentWaypoint = currentLane.Waypoints[0];
            }

            for (int waypointIndex = currentWaypoint.IndexInLane + 1; waypointIndex < currentLane.Waypoints.Count; waypointIndex++)
            {
                if (currentWaypoint == endWaypoint) goto FoundPath; // end algorithm

                var nextWaypoint = currentLane.Waypoints[waypointIndex];

                cameFrom[nextWaypoint] = currentWaypoint;

                currentWaypoint = nextWaypoint;
            }

            foreach ((var nextRoadId, var nextLaneId) in currentLane.NextRoadAndLaneIds)
            {
                var nextLane = roads[nextRoadId].Lanes[nextLaneId];

                var newCost = currentLane.Waypoints.Count; //weight is amount of waypoints of a lane

                if (currentLane != startLane) // had to remove startLane from costSoFar in case that end and start lanes are the same
                {
                    newCost += (int)costSoFar[currentLane];
                }

                if (!costSoFar.ContainsKey(nextLane) || newCost < costSoFar[nextLane])
                {
                    costSoFar[nextLane] = newCost;

                    var priority = newCost + FastEuclideanDistance(
                        endWaypoint.Location.Vector3Ser.ToVector3(),
                        nextLane.Waypoints[0].Location.Vector3Ser.ToVector3()
                        );

                    prioQueue.Enqueue(nextLane, priority);

                    cameFrom[nextLane.Waypoints[0]] = currentWaypoint;
                    laneChanges.Add(nextLane.Waypoints[0]);
                    if (!currentLane.PhysicalNextRoadAndLaneIds.Contains((nextRoadId, nextLaneId)))
                    {
                        physicalLaneChanges.Add(nextLane.Waypoints[0]);
                    }
                }
            }

        }

    FoundPath:
        var waypointPath = new List<Vector2>();

        if (!cameFrom.ContainsKey(endWaypoint))
        {
            Debug.Log("No Path");
            return (null, null);
        }

        currentWaypoint = endWaypoint;
        int index = 0;

        while (currentWaypoint != startWaypoint)
        {
            if (laneChanges.Contains(currentWaypoint))
            {
                if (physicalLaneChanges.Contains(currentWaypoint))
                {
                    laneChangeIndices.Add(index + 1);
                }
                var i = 4;
                while (i > 0 && cameFrom[currentWaypoint] != startWaypoint)
                {
                    currentWaypoint = cameFrom[currentWaypoint];
                    i--;
                }
            }
            waypointPath.Add(currentWaypoint.Location.Vector3Ser.ToVector3());
            currentWaypoint = cameFrom[currentWaypoint];
            index++;
        }

        waypointPath.Add(startWaypoint.Location.Vector3Ser.ToVector3());
        waypointPath.Reverse();
        laneChangeIndices = laneChangeIndices.Select(x => waypointPath.Count() - x - 1).ToList();

        return (waypointPath, laneChangeIndices);
    }

    /// <summary>
    /// Converts Unity coordinates to Carla coordinates.
    /// </summary>
    /// <param name="x">A float representing the X coordinate in Unity.</param>
    /// <param name="y">A float representing the Y coordinate in Unity.</param>
    /// <returns>A tuple containing the X and Y coordinates in Carla.</returns>
    public static (float x, float y) UnityToCarla(float x, float y)
    {
        //Only for Town06 later do as extension method for Vector3Ser or Location
        x = 0.5f * (8f * x + mapDimensions[mapName].MinX + mapDimensions[mapName].MaxX);
        y = 0.5f * (-8f * y + mapDimensions[mapName].MinY + mapDimensions[mapName].MaxY);


        return (x, y);
    }

    /// <summary>
    /// Converts a Unity rotation value to radians.
    /// </summary>
    /// <param name="rotation">A float representing the rotation value in Unity.</param>
    /// <returns>A float representing the rotation value in radians.</returns>
    public static float UnityRotToRadians(float rotation)
    {
        return (float)(Math.PI / 180 * -rotation);
    }

    /// <summary>
    /// Calculates the target lane value for Carla, given start and end lanes. (NOT IMPLEMENTD)
    /// </summary>
    /// <param name="startLane">The starting Lane object.</param>
    /// <param name="endLane">The ending Lane object.</param>
    /// <returns>An integer representing the target lane value for Carla.</returns>
    public static int CalculateTargetLaneValueCarla(Lane startLane, Lane endLane)
    {
        // later for code refactoring
        return -1;
    }

}

/// <summary>
/// A struct representing a JSON waypoint with X, Y coordinates, and rotation.
/// </summary>
public struct JsonWaypoint
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Rot { get; set; }
}

/// <summary>
/// A struct representing the dimensions of a map.
/// </summary>
public struct MapDimension
{
    public float MaxX { get; set; }
    public float MaxY { get; set; }
    public float MinX { get; set; }
    public float MinY { get; set; }
}

/// <summary>
/// Represents a Waypoint in A* algorithm with additional properties which is used to find the shortest path between 2 points
/// </summary>
public class AStarWaypoint
{
    /// <summary>
    /// Constructor for AStarWaypoint class with indexInLane, laneId, location, actionTypeInfo, triggerList and priority parameters
    /// </summary>
    /// <param name="indexInLane">Index in Lane of the waypoint</param>
    /// <param name="laneId">ID of the Lane of the waypoint</param>
    /// <param name="location">Location of the waypoint</param>
    /// <param name="actionTypeInfo">ActionType of the waypoint</param>
    /// <param name="triggerList">List of TriggerInfo objects for the waypoint</param>
    /// <param name="priority">Priority of the waypoint. Default value is "overwrite"</param>
    public AStarWaypoint(int indexInLane, int laneId, Location location)
    {
        IndexInLane = indexInLane;
        LaneId = laneId;
        Location = location;
    }

    public int IndexInLane { get; set; }

    public int LaneId { get; set; }

    public Location Location { get; set; }
}
