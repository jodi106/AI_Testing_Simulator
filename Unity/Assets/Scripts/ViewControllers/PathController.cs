using Entity;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PathController : MonoBehaviour
{
    public GameObject waypointPrefab;

    private LineRenderer lineRenderer;
    private LineRenderer previewRenderer;
    private EdgeCollider2D edgeCollider;
    private List<WaypointViewController> waypoints;
    private bool building;
    private IBaseEntityWithPathController entityController;
    private SnapController snapController;
    public Path path { get; set; }

    private void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();
        this.path = new Path();
        waypoints = new List<WaypointViewController>();

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.SetWidth(0.1f, 0.1f);
        previewRenderer = gameObject.transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
        previewRenderer.SetWidth(0.1f, 0.1f);
        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();

        building = true;

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            if (building)
            {
                Destroy(gameObject);
            }
        });

        EventManager.StartListening(typeof(SubmitPathSelectionAction), x =>
        {
            this.complete();
        });

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (building)
            {
                var action = new MouseClickAction(x);
                var (_, waypoint) = snapController.FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                AddMoveToWaypoint(waypoint.Location.Vector3);
            }
        });
    }

    public void SetEntityController(IBaseEntityWithPathController controller)
    {
        this.entityController = controller;
        AddMoveToWaypoint(controller.getPosition());
    }

    public void Update()
    {
        if (building)
        {
            var (_, waypoint) = snapController.FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (lineRenderer.positionCount > 0)
            {
                var path = snapController.FindPath(lineRenderer.GetPosition(lineRenderer.positionCount - 1), waypoint.Location.Vector3);

                if (path is null)
                {
                    previewRenderer.positionCount = 0;
                    return;
                }

                var locations = path.GetRouteLocations();

                previewRenderer.positionCount = locations.Count;

                int i = 0;
                foreach (var location in locations)
                {
                    previewRenderer.SetPosition(i, location.Vector3);
                    i++;
                }
            }
        }
        else
        {

        }
    }
    public void AddMoveToWaypoint(Vector2 wp)
    {
        //TODO get angle and fix waypoint
        var waypoint = new Waypoint(new Location(wp, 0), new ActionType("MoveToAction"), new List<TriggerInfo>());

        this.path.WaypointList.Add(waypoint);

        if (lineRenderer.positionCount > 0)
        {
            var path = snapController.FindPath(lineRenderer.GetPosition(lineRenderer.positionCount - 1), wp);

            var locations = path.GetAllLocations();

            foreach (var location in locations)
            {
                lineRenderer.positionCount++;
                var position = new Vector3(location.X, location.Y, -0.1f);
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
            }
        }
        else
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(0, wp);
        }
        Vector2[] positions = new Vector2[lineRenderer.positionCount];
        for (var i = 0; i < lineRenderer.positionCount; i++)
        {
            positions[i] = lineRenderer.GetPosition(i);
        }
        edgeCollider.SetPoints(positions.ToList());
    }

    public void moveWaypoint(Waypoint waypoint, Vector2 position)
    {

    }

    public void complete()
    {
        previewRenderer.positionCount = 0;
        building = false;
        entityController.submitPath(path);
    }

    public void setColor(Color color)
    {
        this.lineRenderer.SetColors(color, color);
        color = new Color(color.r, color.g, color.b, 0.5f);
        this.previewRenderer.SetColors(color, color);
    }

    public void OnMouseDown()
    {
        //find closest linerenderer position to the click
        double min = double.MaxValue;
        Vector2 location = Vector2.zero;
        double dist;
        int index = 0;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            if ((dist = SnapController.FastEuclideanDistance(Camera.main.ScreenToWorldPoint(Input.mousePosition), lineRenderer.GetPosition(i))) < min)
            {
                min = dist;
                location = lineRenderer.GetPosition(i);
                index = i;
            }
        }
        //check if there is a waypoint at the position of the linerenderer position
        //waypoints visual position and actual location can differ
        //always disregard waypoint indicators from snapController here; rely on linerenderer
        WaypointViewController nearestWaypoint = null;
        foreach (WaypointViewController waypoint in waypoints)
        {
            if (waypoint.wp.Location.X == location.x && waypoint.wp.Location.Y == location.y)
            {
                nearestWaypoint = waypoint;
                break;
            }
        }
        if (nearestWaypoint != null)
        {
            // open edit dialog
        }
        else
        {
            // open edit dialog, create wp if an action was added, otherwise discard and do not create waypoint
            GameObject wpGameObject = Instantiate(waypointPrefab, new Vector3(location.x, location.y, -0.5f), Quaternion.identity);
            WaypointViewController viewController = wpGameObject.GetComponent<WaypointViewController>();
            viewController.wp = new Waypoint(new Location(location.x, location.y, 0, 0)); // find way to get correct location
            waypoints.Insert(index, viewController);
            SpriteRenderer s = wpGameObject.GetComponent<SpriteRenderer>();
            s.color = new Color(255, 255, 0);
        }
    }
}