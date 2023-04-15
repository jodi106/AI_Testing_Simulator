using Entity;
using Newtonsoft.Json;
using PriorityQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnapController : MonoBehaviour
{
    public GameObject circlePrefab;

    public Dictionary<int, Road> roads;

    private Dictionary<Location, GameObject> waypointGameObjects;

    private Dictionary<string, MapDimension> mapDimensions;

    private GameObject LastClickedWaypointGameObject;

    public bool IgnoreClicks { get; set; }

    void Start()
    {
        IgnoreClicks = false;

        roads = new();

        waypointGameObjects = new();

        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            var action = new MapChangeAction(x);

            foreach (var (_, waypoint) in waypointGameObjects)
            {
                Destroy(waypoint);
            }

            waypointGameObjects.Clear();
            roads.Clear();
            LastClickedWaypointGameObject = null;

            if (action.name != "") LoadRoads(action.name);
        });

        var dimensions = Resources.Load<TextAsset>("waypoints/dimensions");

        mapDimensions = JsonConvert.DeserializeObject<Dictionary<string, MapDimension>>(dimensions.text);
    }
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

    public (int, int) GetRoadIdAndLaneIdFromString(string laneIdString)
    {
        var laneIdSplit = laneIdString.Split("R")[1].Split("L");
        return (int.Parse(laneIdSplit[0]), int.Parse(laneIdSplit[1]));
    }

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
                    ((jsonWaypoint.X - mapDimensions[mapName].minX) - ((-mapDimensions[mapName].minX + mapDimensions[mapName].maxX) / 2)) / 4,
                    ((jsonWaypoint.Y - mapDimensions[mapName].minY) - ((-mapDimensions[mapName].minY + mapDimensions[mapName].maxY) / 2)) / 4 * (-1),
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

    public Location FindWaypoint(Vector2 mousePosition)
    {
        (_, var waypoint) = FindLaneAndWaypoint(mousePosition);
        return waypoint?.Location;
    }

    /*
     * waypoints in Carla Coordinates
     * pos in World Coordinates (Pixel Coordinates / 100)
     * 
    */
    public (Lane, AStarWaypoint) FindLaneAndWaypoint(Vector2 mousePosition)
    {
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

    public static double FastEuclideanDistance(Vector2 a, Vector2 b)
    {
        return Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2);
    }

    public (Lane, Lane) getNeighboringLanes(Lane l)
    {
        var r = roads[l.RoadId];

        var index = r.Lanes.IndexOfKey(l.Id);

        Lane prev = index > 0 ? r.Lanes.Values[index - 1] : null;
        Lane next = index < r.Lanes.Count - 1 ? r.Lanes.Values[index + 1] : null;

        return (prev, next);

    }

    public bool checkLaneChange(Lane startLane, Lane endLane, AStarWaypoint startWaypoint, AStarWaypoint endWaypoint)
    {
        if ((startLane.RoadId == endLane.RoadId && endWaypoint.IndexInLane <= startWaypoint.IndexInLane)
            || endLane.Id * startLane.Id < 0 || FastEuclideanDistance(startWaypoint.Location.Vector3Ser.ToVector3(), endWaypoint.Location.Vector3Ser.ToVector3()) > 15)
        {
            return false;
        }

        (var left, var right) = getNeighboringLanes(startLane);

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

    // return a tuple containing a list of waypoints and indices of lanechanges
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

        if (checkLaneChange(startLane, endLane, startWaypoint, endWaypoint))
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
                    if(!currentLane.PhysicalNextRoadAndLaneIds.Contains((nextRoadId, nextLaneId)))
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
    public static (float x, float y) CarlaToUnity(float x, float y)
    {

        //Convert to Mouse Coordinates
        x = (x - -114.59522247314453f) / 4;
        y = (y - -68.72904205322266f) / 4 * (-1);


        //Handle Offset, so that 0,0 is in the middle
        x = x + -28.077075f;
        y = y + 26.24f;

        return (x, y);
    }


    //Only for Town06 later do as extension method for Vector3Ser or Location
    public static (float x, float y) UnityToCarla(float x, float y)
    {

        x = x + 28.077075f;
        y = y + -26.24f;

        x = x * 4;
        y = y * 4 * (-1);


        x = (x + -114.59522247314453f);
        y = (y + -68.72904205322266f);

        return (x, y);
    }

    public static float UnityRotToRadians(float rotation)
    {
        return (float)(Math.PI / 180 * -rotation);
    }

    public static int CalculateTargetLaneValueCarla(Lane startLane, Lane endLane)
    {
        // later for code refactoring
        return -1;
    }

}

public struct JsonWaypoint
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Rot { get; set; }
}

public struct MapDimension
{
    public float maxX { get; set; }
    public float maxY { get; set; }
    public float minX { get; set; }
    public float minY { get; set; }
}
