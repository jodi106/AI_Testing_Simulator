using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SnapController : MonoBehaviour
{
    public GameObject circlePrefab;
    private Dictionary<string, List<GameObject>> waypoints;

    public bool highlight { get; set; }
    private GameObject waypoint;

    void Start()
    {
        waypoints = new Dictionary<string, List<GameObject>>();
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
                waypoint.x = (waypoint.x - -114.59522247314453f) * 25 / 100;
                waypoint.y = (waypoint.y - -68.72904205322266f) * 25 / 100 * (-1);
                var n = Instantiate(circlePrefab, new Vector3(waypoint.x, waypoint.y, -0.05f), Quaternion.identity);
                n.transform.eulerAngles = Vector3.forward * (-waypoint.rot);
                waypoints[entry.Key].Add(n);
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

}

public class JsonWaypoint
{
    public float x { get; set; }
    public float y { get; set; }
    public float rot { get; set; }
}