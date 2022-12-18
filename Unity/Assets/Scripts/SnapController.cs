using Entity;
using Newtonsoft.Json;
using PriorityQueue;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SnapController : MonoBehaviour
{
    public GameObject circlePrefab;

    private Dictionary<int, Road> roads;

    private Dictionary<Location, GameObject> waypointGameObjects;

    private GameObject LastClickedWaypointGameObject;

    void Start()
    {
        roads = new();

        waypointGameObjects = new();

        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            var action = new MapChangeAction(x);

            foreach (var (_, waypoint) in waypointGameObjects)
            {
                Destroy(waypoint);
            }

            LoadRoads(action.name);
        });
    }
    public void Update()
    {
        if (LastClickedWaypointGameObject is not null)
        {
            var sprite = LastClickedWaypointGameObject.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            LastClickedWaypointGameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

            var (_, waypoint) = FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            var waypointGameObject = waypointGameObjects[waypoint.Location];

            if (waypointGameObject is not null)
            {
                waypointGameObject.GetComponent<SpriteRenderer>().color = Color.green;
                waypointGameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
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

            var waypoints = new List<AStarWaypoint>();

            var index = 0;

            foreach (var jsonWaypoint in jsonWaypoints)
            {
                var position = new Vector3((jsonWaypoint.X - -114.59522247314453f) / 4 - 28.077075f,
                                    (jsonWaypoint.Y - -68.72904205322266f) / 4 * (-1) + 26.24f, -0.05f);

                var waypointGameObject = Instantiate(
                    circlePrefab,
                    position,
                    Quaternion.identity);

                waypointGameObject.transform.eulerAngles = Vector3.forward * (-jsonWaypoint.Rot);

                var location = new Location(waypointGameObject.transform.position, waypointGameObject.transform.eulerAngles.z);

                waypoints.Add(new AStarWaypoint(index++, laneId, location));

                //Debug.Log(waypointGameObject.transform.eulerAngles.ToString() + " "+ jsonWaypoint.Rot.ToString());

                waypointGameObjects.Add(location,waypointGameObject);
            }

            roads[roadId].Lanes[laneId].Waypoints = waypoints;

        }
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
                    double currDistance = FastEuclideanDistance(waypoint.Location.Vector3, mousePosition);    

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

    public Path FindPath(Vector2 start, Vector2 end)
    {
        (Lane startLane, AStarWaypoint startWaypoint) = FindLaneAndWaypoint(start);
        (_, AStarWaypoint endWaypoint) = FindLaneAndWaypoint(end);

        var prioQueue = new SimplePriorityQueue<Lane, double>();

        prioQueue.Enqueue(startLane, 0);

        var cameFrom = new Dictionary<AStarWaypoint, AStarWaypoint>
        {
            { startWaypoint, null }
        };

        var costSoFar = new Dictionary<Lane, double>
        {
            { startLane, 0 }
        };

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

                var newCost = costSoFar[currentLane] + currentLane.Waypoints.Count; //weight is amount of waypoints of a lane

                if (!costSoFar.ContainsKey(nextLane) || newCost < costSoFar[nextLane])
                {
                    costSoFar[nextLane] = newCost;

                    var priority = newCost + FastEuclideanDistance(
                        endWaypoint.Location.Vector3, 
                        nextLane.Waypoints[0].Location.Vector3
                        );

                    prioQueue.Enqueue(nextLane, priority);

                    cameFrom[nextLane.Waypoints[0]] = currentWaypoint;
                }
            }

        }

    FoundPath:
        var waypointPath = new List<Waypoint>();
            currentWaypoint = endWaypoint;
            if (!cameFrom.ContainsKey(endWaypoint))
            {
                Debug.Log("No Path, this shouldnt happend so if you see this, smth is burning");
            }
            else
            {
                while (currentWaypoint != startWaypoint)
                {
                waypointPath.Add(currentWaypoint);
                    currentWaypoint = cameFrom[currentWaypoint];
                }
                waypointPath.Add(startWaypoint);
                waypointPath.Reverse();
            }

            return new Path(waypointPath);
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


    //Only for Town06 later do as extension method for Vector3 or Location
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
        return (float)(Math.PI / 180 * rotation);
    }

}

public struct JsonWaypoint
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Rot { get; set; }
}

