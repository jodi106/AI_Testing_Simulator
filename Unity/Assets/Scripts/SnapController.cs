using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SnapController : MonoBehaviour
{
    public GameObject circlePrefab;
    private Dictionary<string, List<GameObject>> waypoints;
    private Dictionary<string, List<string>> topology;

    public bool highlight { get; set; }
    private GameObject waypoint;

    void Start()
    {
        waypoints = new Dictionary<string, List<GameObject>>();
        topology = new Dictionary<string, List<string>>();
        highlight = true;
        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            var action = new MapChangeAction(x);

            foreach (KeyValuePair<string, List<GameObject>> entry in waypoints)
            {
                foreach (GameObject waypoint in entry.Value)
                {
                    Destroy(waypoint);
                }
            }
            loadWaypoints(action.name);
            loadTopology(action.name);
        });
    }

    void loadWaypoints(string mapName)
    {
        Dictionary<string, List<JsonWaypoint>> jsonWaypoints;
        var text = Resources.Load<TextAsset>("Waypoints/" + mapName);
        jsonWaypoints = JsonConvert.DeserializeObject<Dictionary<string, List<JsonWaypoint>>>(text.text);
        foreach (KeyValuePair<string, List<JsonWaypoint>> entry in jsonWaypoints)
        {
            waypoints[entry.Key] = new List<GameObject>();
            foreach (JsonWaypoint waypoint in entry.Value)
            {
                waypoint.x = (waypoint.x - -114.59522247314453f) / 4;
                waypoint.y = (waypoint.y - -68.72904205322266f) / 4 * (-1);
                var n = Instantiate(circlePrefab, new Vector3(waypoint.x - 28.077075f, waypoint.y + 26.24f, -0.05f), Quaternion.identity);
                n.transform.eulerAngles = Vector3.forward * (-waypoint.rot);
                waypoints[entry.Key].Add(n);
            }

        }
    }

    void loadTopology(string mapName)
    {
        var text = Resources.Load<TextAsset>("Waypoints/" + mapName + "_topology");
        Dictionary<string, List<string>> jsonEntries = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text.text);
        foreach (KeyValuePair<string, List<string>> entry in jsonEntries)
        {
            topology[entry.Key] = new List<string>();
            foreach (string lane in entry.Value)
            {
                topology[entry.Key].Add(lane);
            }
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
            waypoint = findNearestWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (waypoint is not null)
            {
                waypoint.GetComponent<SpriteRenderer>().color = Color.green;
                waypoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }
    }

    /*
     * waypoints in Carla Coordinates
     * pos in World Coordinates (Pixel Coordinates / 100)
     * 
    */
    public GameObject findNearestWaypoint(Vector2 pos)
    {
        GameObject closestWaypoint = null;
        double distance = double.MaxValue;
        foreach (KeyValuePair<string, List<GameObject>> entry in waypoints)
        {
            foreach (GameObject waypoint in entry.Value)
            {
                double currDistance = Math.Sqrt(Math.Pow(waypoint.transform.position.x - pos.x, 2) + Math.Pow(waypoint.transform.position.y - pos.y, 2));
                if (currDistance < distance)
                {
                    closestWaypoint = waypoint;
                    distance = currDistance;
                }
            }
        }
        return closestWaypoint;
    }

    //TODO improve
    public string findLane(Vector2 position)
    {
        GameObject closestWaypoint = findNearestWaypoint(position);
        foreach (KeyValuePair<string, List<GameObject>> entry in waypoints)
        {
            foreach (GameObject wp in entry.Value)
            {
                if(wp == closestWaypoint)
                {
                    return entry.Key;
                }
            }
        }
        return null;
    }

    public List<GameObject> findPath(Vector2 start, Vector2 end)
    {
        string startLane = findLane(start);
        string endLane = findLane(end);

        GameObject startWP = findNearestWaypoint(start);
        GameObject endWP = findNearestWaypoint(end);

        // TODO: detect lane changes
        if (startLane.Equals(endLane))
        {

        }

        var visited = new HashSet<string>();
        var queue = new Queue<string>();
        Dictionary<string, string> pred = new Dictionary<string, string>();

        queue.Enqueue(startLane);

        bool found = false;
        while (queue.Count > 0)
        {
            if(found)
            {
                break;
            }
            var vertex = queue.Dequeue();
            visited.Add(vertex);
            foreach(var neighboar in topology[vertex]) {
                if(visited.Contains(neighboar))
                {
                    continue;
                } else
                {
                    pred[neighboar] = vertex;
                    queue.Enqueue(neighboar);
                    if(neighboar.Equals(endLane))
                    {
                        found = true;
                        break;
                    }
                }
            }
        }

        if(found)
        {
            List<string> lanePath = new List<string>();
            List<GameObject> path = new List<GameObject>();
            lanePath.Add(endLane);
            string p = pred[endLane];
            lanePath.Insert(0, p);
            Debug.Log(p);
            while(p != startLane)
            {
                p = pred[p];
                lanePath.Insert(0, p);                                      
                Debug.Log(p);
            }
            foreach(string lane in lanePath)
            {
                if (lane.Equals(startLane) || lane.Equals(endLane)) continue;
                path.AddRange(waypoints[lane]);
            }

            int startIndex = waypoints[startLane].FindIndex(x => x == startWP);
            int endIndex = waypoints[endLane].FindIndex(x => x == endWP);
            path.InsertRange(0, waypoints[startLane].GetRange(startIndex, waypoints[startLane].Count - startIndex));
            path.AddRange(waypoints[endLane].GetRange(0, endIndex + 1));

            return path;
        } else
        {
            return null;
        }
    }

}

public class JsonWaypoint
{
    public float x { get; set; }
    public float y { get; set; }
    public float rot { get; set; }
}