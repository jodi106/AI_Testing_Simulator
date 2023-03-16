using Entity;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


public class MapGeneratorStart : MonoBehaviour
{
    private List<Waypoint> waypoints;

    public GameObject circlePrefab;

    private List<GameObject> waypointGameObjects;

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

    // Start is called before the first frame update
    void Start()
    {

        waypoints = new List<Waypoint>();
        waypointGameObjects = new List<GameObject>();

        var text = Resources.Load<TextAsset>("Waypoints/Town10HD");
        Debug.Log(text);
        var jsonLaneWithWaypoint = JsonConvert.DeserializeObject<Dictionary<string, List<JsonWaypoint>>>(text.text);
        Debug.Log(jsonLaneWithWaypoint);

        foreach (var (laneIdString, jsonWaypoints) in jsonLaneWithWaypoint)
        {
            foreach (var jsonWaypoint in jsonWaypoints)
            {
                Debug.Log(jsonWaypoint.X);
                (float x, float y) = CarlaToUnity(jsonWaypoint.X, jsonWaypoint.Y);
                var vector3 = new Vector3(x, y, HeightUtil.WAYPOINT_INDICATOR);
                Debug.Log(vector3);

                var location = new Location(vector3);
                Debug.Log(location.Vector3.x);

                var waypoint = new Waypoint(location);
                Debug.Log(waypoint.Location.X);

                waypoints.Add(waypoint);
            }

        }

        foreach (var waypoint in waypoints)
        {
            var position = new Vector3(waypoint.Location.X, waypoint.Location.Y, HeightUtil.WAYPOINT_INDICATOR);
            Debug.Log(position);

            var waypointGameObject = Instantiate(
            circlePrefab,
                    position,
                    Quaternion.identity);

            waypointGameObjects.Add(waypointGameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
