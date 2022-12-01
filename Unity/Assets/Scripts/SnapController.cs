using Entity;
using Newtonsoft.Json;
using PriorityQueue;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JsonWaypoint
{
    public float x { get; set; }
    public float y { get; set; }
    public float rot { get; set; }
}

public class Road
{
    public Road(int id, SortedList<int, Lane> lanes)
    {
        Id = id;
        Lanes = lanes;
    }

    public int Id { get; set; }


    public SortedList<int, Lane> Lanes { get; set; }
}

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

public class LaneWaypoint
{

    public LaneWaypoint(int position, int laneId, int roadId, GameObject waypointGameObject, Vector3 coords)
    {
        Id = roadId.ToString() + laneId.ToString() + position.ToString();
        Position = position;
        LaneId = laneId;
        RoadId = roadId;
        WaypointGameObject = waypointGameObject;
        Coords = coords;
    }

    public string Id { get; set; }

    public int Position { get; set; }

    public int LaneId { get; set; }

    public int RoadId { get; set; }


    public GameObject WaypointGameObject { get; set; }

    public Vector3 Coords { get; set; }

    public bool Equals(LaneWaypoint o)
    {
        return o.Id == this.Id;
    }


}

public class SnapController : MonoBehaviour
{
    public GameObject circlePrefab;

    private Dictionary<int, Road> roads;

    public bool highlight { get; set; }

    private GameObject waypoint;

    void Start()
    {
        roads = new Dictionary<int, Road>();
        highlight = true;

        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            var action = new MapChangeAction(x);

            foreach (var (_, road) in roads)
            {
                foreach (var (_, lane) in road.Lanes)
                {
                    foreach (var waypoint in lane.Waypoints)
                    {
                        Destroy(waypoint.WaypointGameObject);
                    }
                }
            }

            LoadRoads(action.name);
        });
    }

    (int, int) GetRoadIdAndLaneIdFromString(string laneIdString)
    {
        var laneIdSplit = laneIdString.Split("R")[1].Split("L");
        return (int.Parse(laneIdSplit[0]), int.Parse(laneIdSplit[1]));
    }

    void LoadRoads(string mapName)
    {
        roads = new Dictionary<int, Road>();

        var text = Resources.Load<TextAsset>("Waypoints/" + mapName + "_topology");

        var jsonLanes = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text.text);

        foreach (var (laneIdString, nextRoadAndLaneIdStrings) in jsonLanes)
        {
            (var roadId, var laneId) = GetRoadIdAndLaneIdFromString(laneIdString);

            if (!roads.ContainsKey(roadId))
            {
                roads.Add(roadId, new Road(roadId, new SortedList<int, Lane>()));
            }

            var nextRoadAndLaneIds = new List<(int, int)>();

            nextRoadAndLaneIdStrings.ForEach(nextString =>
                nextRoadAndLaneIds.Add(GetRoadIdAndLaneIdFromString(nextString))
            );

            roads[roadId].Lanes.Add(laneId, new Lane(laneId, roadId, nextRoadAndLaneIds));
        }

        foreach (var (roadId, road) in roads)
        {
            foreach (var (laneId, lane) in road.Lanes)
            {
                var nextPossibleRoadAndLaneIds = new List<(int, int)>();

                foreach ((var nextRoadId, var nextLaneId) in lane.NextRoadAndLaneIds)
                {
                    nextPossibleRoadAndLaneIds.Add((nextRoadId, nextLaneId));

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

            var waypoints = new List<LaneWaypoint>();

            var index = 0;

            foreach (var jsonWaypoint in jsonWaypoints)
            {
                var whiteCircle = Instantiate(
                    circlePrefab,
                    new Vector3((jsonWaypoint.x - -114.59522247314453f) / 4 - 28.077075f, (jsonWaypoint.y - -68.72904205322266f) / 4 * (-1) + 26.24f, -0.05f),
                    Quaternion.identity);

                whiteCircle.transform.eulerAngles = Vector3.forward * (-jsonWaypoint.rot);

                waypoints.Add(new LaneWaypoint(index, laneId, roadId, whiteCircle, whiteCircle.transform.position));
                index++;
            }

            roads[roadId].Lanes[laneId].Waypoints = waypoints;

        }
    }

    public void Update()
    {
        if (waypoint is not null)
        {
            var sprite = waypoint.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            waypoint.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); ;
        }

        if (highlight)
        {
            var (_, waypoint) = FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (waypoint is not null)
            {

                waypoint.WaypointGameObject.GetComponent<SpriteRenderer>().color = Color.green;
                waypoint.WaypointGameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }
    }

    /*
     * waypoints in Carla Coordinates
     * pos in World Coordinates (Pixel Coordinates / 100)
     * 
    */
    public (Lane, LaneWaypoint) FindLaneAndWaypoint(Vector2 pos)
    {
        LaneWaypoint closestWaypoint = null;
        Lane laneToReturn = null;
        double distance = double.MaxValue;

        foreach (var (_, road) in roads)
        {
            foreach (var (_, lane) in road.Lanes)
            {
                foreach (var waypoint in lane.Waypoints)
                {
                    double currDistance = Math.Sqrt(Math.Pow(waypoint.WaypointGameObject.transform.position.x - pos.x, 2) +
                        Math.Pow(waypoint.WaypointGameObject.transform.position.y - pos.y, 2));

                    if (currDistance == 0)
                    {
                        return (lane, waypoint);

                    }

                    if (currDistance < distance)
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

    public double Heuristic(Vector3 a, Vector3 b)
    {
        return Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2);
    }

    public List<GameObject> findPath(Vector2 start, Vector2 end)
    {
        (Lane startLane, LaneWaypoint startWaypoint) = FindLaneAndWaypoint(start);
        (Lane endLane, LaneWaypoint endWaypoint) = FindLaneAndWaypoint(end);

        var prioQueue = new SimplePriorityQueue<Lane, double>();

        prioQueue.Enqueue(startLane, 0);

        var cameFrom = new Dictionary<LaneWaypoint, LaneWaypoint>
        {
            { startWaypoint, null }
        };

        var costSoFar = new Dictionary<Lane, double>
        {
            { startLane, 0 }
        };

        var foundPath = false;

        var firstIteration = true;

        LaneWaypoint currentWaypoint;

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

            for (int waypointIndex = currentWaypoint.Position + 1; waypointIndex < currentLane.Waypoints.Count; waypointIndex++)
            {
                if (currentWaypoint == endWaypoint)
                {
                    foundPath = true;
                    break;
                }

                var nextWaypoint = currentLane.Waypoints[waypointIndex];
                cameFrom[nextWaypoint] = currentWaypoint;

                currentWaypoint = nextWaypoint;
            }

            if (foundPath)
            {
                break;
            }


            foreach ((var nextRoadId, var nextLaneId) in currentLane.NextRoadAndLaneIds)
            {
                var nextLane = roads[nextRoadId].Lanes[nextLaneId];

                var newCost = costSoFar[currentLane] + currentLane.Waypoints.Count; //weight is length of the lane

                if (!costSoFar.ContainsKey(nextLane) || newCost < costSoFar[nextLane])
                {
                    costSoFar[nextLane] = newCost;

                    var priority = newCost + Heuristic(endWaypoint.WaypointGameObject.transform.position, nextLane.Waypoints[0].WaypointGameObject.transform.position); // euclidean distance

                    prioQueue.Enqueue(nextLane, priority);

                    cameFrom[nextLane.Waypoints[0]] = currentWaypoint;
                }
            }

        }


        var path = new List<GameObject>();
        var current = endWaypoint;
        if (!cameFrom.ContainsKey(endWaypoint))
        {
            Debug.Log("No Path, this shouldnt happend so if you see this, smth is burning");
        }
        else
        {
            while (current != startWaypoint)
            {
                path.Add(current.WaypointGameObject);
                current = cameFrom[current];
            }
            path.Add(startWaypoint.WaypointGameObject);
            path.Reverse();
        }

        return path;
    }
}

