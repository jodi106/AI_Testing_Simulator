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
    public Road(int id, SortedDictionary<int, Lane> lanes)
    {
        Id = id;
        Lanes = lanes;
    }

    public int Id { get; set; } 

    public SortedDictionary<int,Lane> Lanes { get; set; }
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

    public List<(int,int)> NextRoadAndLaneIds { get; set; }
}

public class LaneWaypoint {

    public LaneWaypoint(int id, int laneId, GameObject waypointGameObject, Vector3 coords)
    {
        Id = id;
        LaneId = laneId;
        WaypointGameObject = waypointGameObject;
        Coords = coords;
    }

    public int Id { get; set; }

    public int LaneId { get; set; }

    public GameObject WaypointGameObject { get; set; }

    public Vector3 Coords { get; set; }

    public String ToString()
    {
        return Id.ToString() + " " + LaneId.ToString() + " " + Coords.ToString();
    }

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

            foreach (var (_,road) in roads)
            {
                foreach (var (_,lane) in road.Lanes)
                {
                    foreach(var waypoint in lane.Waypoints)
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
                roads.Add(roadId, new Road(roadId, new SortedDictionary<int,Lane>()));
            }

            var nextRoadAndLaneIds = new List<(int,int)>();

            nextRoadAndLaneIdStrings.ForEach(nextString =>
                nextRoadAndLaneIds.Add(GetRoadIdAndLaneIdFromString(nextString))
            );

            roads[roadId].Lanes.Add(laneId,new Lane(laneId,roadId, nextRoadAndLaneIds));
        }
        

        text = Resources.Load<TextAsset>("Waypoints/" + mapName);

        var jsonLaneWithWaypoint = JsonConvert.DeserializeObject<Dictionary<string, List<JsonWaypoint>>>(text.text);

        foreach (var (laneIdString, jsonWaypoints) in jsonLaneWithWaypoint)
        {
            (var roadId, var laneId) = GetRoadIdAndLaneIdFromString(laneIdString);

            var waypoints = new List<LaneWaypoint>();
            
            foreach (var jsonWaypoint in jsonWaypoints)
            {


                var whiteCircle = Instantiate(
                    circlePrefab,
                    new Vector3((jsonWaypoint.x - -114.59522247314453f) / 4 - 28.077075f, (jsonWaypoint.y - -68.72904205322266f) / 4 * (-1) + 26.24f, -0.05f),
                    Quaternion.identity);

                whiteCircle.transform.eulerAngles = Vector3.forward * (-jsonWaypoint.rot);

                waypoints.Add(new LaneWaypoint(laneId + roadId, laneId, whiteCircle, whiteCircle.transform.position));
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
            var (_,waypoint) = FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
    public (Lane,LaneWaypoint) FindLaneAndWaypoint(Vector2 pos)
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

        var currentWaypoint = startWaypoint;

        var currentIndex = startLane.Waypoints.IndexOf(startWaypoint);

        var flag = false;

        var c = 0;

        while ( prioQueue.Count != 0 && c < 100)
        {
            c++;

            var currentLane = prioQueue.Dequeue();

            var nextWaypoint = currentLane.Waypoints[currentIndex++];

            cameFrom[nextWaypoint] = currentWaypoint;

            currentWaypoint = nextWaypoint;
            


            while (currentIndex < currentLane.Waypoints.Count)
            {
                if (currentWaypoint == endWaypoint)
                {
                    flag = true;
                    break;
                }

                nextWaypoint = currentLane.Waypoints[currentIndex++];

                cameFrom[nextWaypoint] = currentWaypoint;
            }

            if (flag)
            {
                break;
            }

            foreach ((var nextRoadId, var nextLaneId) in currentLane.NextRoadAndLaneIds)
            {
                var nextLane = roads[nextRoadId].Lanes[nextLaneId];

                var newCost = costSoFar[currentLane] + currentLane.Waypoints.Count; //Count is weight of the lane

                if (!costSoFar.ContainsKey(nextLane) || newCost < costSoFar[nextLane])
                {
                    costSoFar[nextLane] = newCost;

                    var priority = newCost + Heuristic(endWaypoint.WaypointGameObject.transform.position, nextLane.Waypoints[0].WaypointGameObject.transform.position); // euclidean distance

                    prioQueue.Enqueue(nextLane, priority);

                    nextWaypoint = nextLane.Waypoints[0];

                    cameFrom[nextWaypoint] = currentWaypoint;
                }
            }

            currentIndex = 0;
        }

        foreach(var (key,value) in cameFrom)
        {
            Debug.Log(key + " "+ value.ToString());
        }
        
        var path = new List<GameObject>();
        var current = currentWaypoint;
        while (current != startWaypoint)
        {
            path.Add(current.WaypointGameObject);
            current = cameFrom[current];
        }
        path.Add(startWaypoint.WaypointGameObject);
        path.Reverse();
        return path;


        //// TODO: detect lane changes
        //if (startLane.Equals(endLane))
        //{

        //}

        //var visited = new HashSet<Lane>();
        //var queue = new Queue<Lane>();

        //var pred = new Dictionary<string, string>();

        //queue.Enqueue(startLane);

        //bool found = false;

        //while (queue.Count > 0)
        //{
        //    if (found)
        //    {
        //        break;
        //    }

        //    var vertex = queue.Dequeue();
        //    visited.Add(vertex);

        //    foreach (var neighboar in topology[vertex])
        //    {
        //        if (visited.Contains(neighboar))
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            pred[neighboar] = vertex;
        //            queue.Enqueue(neighboar);
        //            if (neighboar.Equals(endLane))
        //            {
        //                found = true;
        //                break;
        //            }
        //        }
        //    }
        //}

        //if (found)
        //{
        //    List<string> lanePath = new List<string>();
        //    List<GameObject> path = new List<GameObject>();
        //    lanePath.Add(endLane);
        //    string p = pred[endLane];
        //    lanePath.Insert(0, p);
        //    Debug.Log(p);
        //    while (p != startLane)
        //    {
        //        p = pred[p];
        //        lanePath.Insert(0, p);
        //        Debug.Log(p);
        //    }
        //    foreach (string lane in lanePath)
        //    {
        //        if (lane.Equals(startLane) || lane.Equals(endLane)) continue;
        //        path.AddRange(waypoints[lane]);
        //    }

        //    int startIndex = waypoints[startLane].FindIndex(x => x == startWP);
        //    int endIndex = waypoints[endLane].FindIndex(x => x == endWP);
        //    path.InsertRange(0, waypoints[startLane].GetRange(startIndex, waypoints[startLane].Count - startIndex));
        //    path.AddRange(waypoints[endLane].GetRange(0, endIndex + 1));

        //    return path;
        //}
    }

    //void loadLanes(string mapName)
    //{
    //    lanes = new Dictionary<string, Lane>();

    //    var text = Resources.Load<TextAsset>("Waypoints/" + mapName + "_topology");

    //    var jsonLanes = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text.text);

    //    foreach (var (laneId, nextLanes) in jsonLanes)
    //    {
    //        var lane = new Lane(laneId, nextLanes);
    //    }

    //    text = Resources.Load<TextAsset>("Waypoints/" + mapName);

    //    var jsonWaypoints = JsonConvert.DeserializeObject<Dictionary<string, List<JsonWaypoint>>>(text.text);

    //    foreach (var (laneId, waypoints) in jsonWaypoints)
    //    {
    //        lanes[laneId].Waypoints = new List<GameObject>();

    //        foreach (JsonWaypoint waypoint in waypoints)
    //        {
    //            waypoint.x = (waypoint.x - -114.59522247314453f) / 4;
    //            waypoint.y = (waypoint.y - -68.72904205322266f) / 4 * (-1);

    //            var whiteCircle = Instantiate(
    //                circlePrefab,
    //                new Vector3(waypoint.x - 28.077075f, waypoint.y + 26.24f, -0.05f),
    //                Quaternion.identity);

    //            whiteCircle.transform.eulerAngles = Vector3.forward * (-waypoint.rot);

    //            lanes[laneId].Waypoints.Add(whiteCircle);
    //        }
    //    }
    //}
}

