using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapController : MonoBehaviour
{
    public GameObject circlePrefab;
    private Dictionary<string, List<JsonWaypoint>> waypoints;
    void Start()
    {
        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            var action = new MapChangeAction(x);
            loadWaypoints(action.name);
        });
    }

    void loadWaypoints(string mapName)
    {
        var text = Resources.Load<TextAsset>("Waypoints/" + mapName);
        waypoints = JsonConvert.DeserializeObject<Dictionary<string, List<JsonWaypoint>>>(text.text);
        foreach (KeyValuePair<string, List<JsonWaypoint>> entry in waypoints)
        {
            foreach (JsonWaypoint waypoint in entry.Value)
            {
                waypoint.x = (waypoint.x - -114.59522247314453f) * 25 / 100;
                waypoint.y = (waypoint.y - -68.72904205322266f) * 25 / 100 * (-1);
                Instantiate(circlePrefab, new Vector3(waypoint.x, waypoint.y, -0.05f), Quaternion.identity);
            }

        }
    }

    /*
     * waypoints in Carla Coordinates
     * pos in World Coordinates (Pixel Coordinates / 100)
     * 
    */
    public JsonWaypoint? findNearestWaypoint(Vector2 pos)
    {
        JsonWaypoint closestWaypoint = null;
        double distance = double.MaxValue;
        foreach (KeyValuePair<string, List<JsonWaypoint>> entry in waypoints)
        {
            foreach (JsonWaypoint waypoint in entry.Value)
            {
                double currDistance = Math.Sqrt(Math.Pow(waypoint.x - pos.x, 2) + Math.Pow(waypoint.y - pos.y, 2));
                if (currDistance < distance)
                {
                    closestWaypoint = waypoint;
                    distance = currDistance;
                }
            }
        }
        return closestWaypoint;
    }

}

public class JsonWaypoint
{
    public float x { get; set; }
    public float y { get; set; }
    public float rot { get; set; }
}