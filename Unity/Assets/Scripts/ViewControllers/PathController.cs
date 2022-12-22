using Entity;
using System.Collections.Generic;
using System.Linq;
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
        lineRenderer.startWidth = lineRenderer.endWidth = 0.1f;
        previewRenderer = gameObject.transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
        previewRenderer.startWidth = previewRenderer.endWidth = 0.1f;
        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();

        building = true;

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            if (building)
            {
                this.destroy();
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

    public void destroy()
    {
        foreach (WaypointViewController wp in waypoints)
        {
            Destroy(wp.gameObject);
        }
        Destroy(gameObject);
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

                previewRenderer.positionCount = path.Count;

                int i = 0;
                foreach (var location in path)
                {
                    previewRenderer.SetPosition(i, location);
                    i++;
                }
            }
        }
        else
        {

        }
    }
    public void AddMoveToWaypoint(Vector2 location)
    {
        //TODO get angle and fix waypoint

        GameObject wpGameObject = Instantiate(waypointPrefab, new Vector3(location.x, location.y, -0.5f), Quaternion.identity);
        WaypointViewController viewController = wpGameObject.GetComponent<WaypointViewController>();
        viewController.wp = new Waypoint(new Location(location, 0), new ActionType("MoveToAction"), new List<TriggerInfo>());
        this.path.WaypointList.Add(viewController.wp);
        waypoints.Add(viewController);
        SpriteRenderer s = wpGameObject.GetComponent<SpriteRenderer>();
        s.color = new Color(0, 0, 0);

        if (lineRenderer.positionCount > 0)
        {
            var path = snapController.FindPath(lineRenderer.GetPosition(lineRenderer.positionCount - 1), location);

            foreach (var loc in path)
            {
                lineRenderer.positionCount++;
                var position = new Vector3(loc.x, loc.y, -0.1f);
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
            }
        }
        else
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(0, location);
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
        this.lineRenderer.startColor = this.lineRenderer.endColor = color;
        color = new Color(color.r, color.g, color.b, 0.5f);
        this.previewRenderer.startColor = this.previewRenderer.endColor = color;
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
            nearestWaypoint.openEditDialog();
        }
        else
        {
            // open edit dialog, create wp if an action was added, otherwise discard and do not create waypoint
            // maybe use separate dialogs for editing / creating
        }
    }
}