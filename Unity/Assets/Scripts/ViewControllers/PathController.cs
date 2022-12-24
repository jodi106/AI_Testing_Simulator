using Entity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathController : MonoBehaviour
{
    public GameObject waypointPrefab;

    private LineRenderer pathRenderer;
    private LineRenderer previewRenderer;
    private EdgeCollider2D edgeCollider;

    private List<WaypointViewController> waypointViewControllers;
    private IBaseEntityWithPathController entityWithPathController;
    private SnapController snapController;

    private bool building;

    public Path Path { get; set; }
    public Lane CurrentLane { get; set; }
    public AStarWaypoint CurrentWaypoint { get; set; }

    private void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();

        this.Path = new Path();

        waypointViewControllers = new List<WaypointViewController>();

        pathRenderer = gameObject.GetComponent<LineRenderer>();
        pathRenderer.startWidth = pathRenderer.endWidth = 0.1f;

        previewRenderer = gameObject.transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
        previewRenderer.startWidth = previewRenderer.endWidth = 0.1f;

        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.PATH_SELECTED);

        building = true;

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            if (building)
            {
                this.Destroy();
            }
        });

        EventManager.StartListening(typeof(SubmitPathSelectionAction), x =>
        {
            this.Complete();
        });

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (building)
            {
                var action = new MouseClickAction(x);
                AddMoveToWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        });
    }

    public void Select()
    {
        for (var i = 0; i < pathRenderer.positionCount; i++)
        {
            pathRenderer.SetPosition(i, HeightUtil.SetZ(pathRenderer.GetPosition(i), HeightUtil.PATH_SELECTED));
        }

        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.PATH_SELECTED);
        foreach (var waypoint in waypointViewControllers)
        {
            waypoint.gameObject.transform.position = HeightUtil.SetZ(waypoint.gameObject.transform.position, HeightUtil.WAYPOINT_SELECTED);
        }
    }

    public void Deselect()
    {
        for (var i = 0; i < pathRenderer.positionCount; i++)
        {
            pathRenderer.SetPosition(i, HeightUtil.SetZ(pathRenderer.GetPosition(i), HeightUtil.PATH_DESELECTED));
        }

        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.PATH_DESELECTED);

        foreach (var waypoint in waypointViewControllers)
        {
            waypoint.gameObject.transform.position = HeightUtil.SetZ(waypoint.gameObject.transform.position, HeightUtil.WAYPOINT_DESELECTED);
        }
    }

    public void Destroy()
    {
        foreach (WaypointViewController wp in waypointViewControllers)
        {
            Destroy(wp.gameObject);
        }
        Destroy(gameObject);
    }

    public void SetEntityController(IBaseEntityWithPathController controller)
    {
        this.entityWithPathController = controller;
        AddMoveToWaypoint(controller.getPosition()); //init with starting position of car
    }

    public void Update()
    {
        if (building)
        {
            AddMoveToWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition), preview: true);
        }
    }

    public void AddMoveToWaypoint(Vector2 mousePosition, bool preview = false)
    {
        var (nextLane, nextWaypoint) = snapController.FindLaneAndWaypoint(mousePosition);

        if (nextLane == null || nextWaypoint == null) {
            Debug.Log("Invalid mouse position, mouse probably not on road!");
            return;
        }

        Debug.Log("Valid mouse position");

        LineRenderer lineRenderer;

        if (preview && !Path.IsEmpty())
        {
            lineRenderer = previewRenderer;
            lineRenderer.SetPosition(0, HeightUtil.SetZ(CurrentWaypoint.Location.Vector3, HeightUtil.PATH_SELECTED));
            lineRenderer.positionCount = 1;
        } else
        {
            lineRenderer = pathRenderer;
        }

        var actionType = new ActionType("");

        if (Path.IsEmpty())
        {
            actionType = new ActionType("MoveToAction");
        }
        else if (nextLane.RoadId == CurrentLane.RoadId && nextLane.Id * CurrentLane.Id > 0 && // user wants to change lanes
               (nextLane.Id == CurrentLane.Id + 1 || nextLane.Id == CurrentLane.Id - 1) && // lanes are next to eachother
               (nextWaypoint.IndexInLane >= CurrentWaypoint.IndexInLane)) // lane change in forward direction
        {
            actionType = new ActionType("LaneChangeAction");
        }
        else
        {
            var path = snapController.FindPath(CurrentWaypoint.Location.Vector3, nextWaypoint.Location.Vector3);

            if (path is null)
            {
                return; //this kinda shouldnt happen
            }

            foreach (var coord in path)
            {
                lineRenderer.SetPosition(lineRenderer.positionCount++, HeightUtil.SetZ(coord, HeightUtil.PATH_SELECTED));
            }

            actionType = new ActionType("MoveToAction");
        }

        GameObject wpGameObject = Instantiate(waypointPrefab, new Vector3(nextWaypoint.Location.X, nextWaypoint.Location.Y, HeightUtil.WAYPOINT_SELECTED), Quaternion.identity);

        SpriteRenderer s = wpGameObject.GetComponent<SpriteRenderer>();

        s.color = new Color(0, 0, 0);

        lineRenderer.SetPosition(lineRenderer.positionCount++, HeightUtil.SetZ(nextWaypoint.Location.Vector3, HeightUtil.PATH_SELECTED));

        if (!preview)
        {
            WaypointViewController viewController = wpGameObject.GetComponent<WaypointViewController>();

            viewController.waypoint = new Waypoint(new Location(nextWaypoint.Location.Vector3, 0), actionType, new List<TriggerInfo>());

            this.Path.WaypointList.Add(viewController.waypoint);

            waypointViewControllers.Add(viewController);


            Vector2[] positions = new Vector2[lineRenderer.positionCount];
            for (var i = 0; i < lineRenderer.positionCount; i++)
            {
                positions[i] = lineRenderer.GetPosition(i);
            }
            edgeCollider.SetPoints(positions.ToList());

            CurrentWaypoint = nextWaypoint;
            CurrentLane = nextLane;
        }

    }

    public void MoveWaypoint(Waypoint waypoint, Vector2 position)
    {

    }

    public void Complete()
    {
        previewRenderer.positionCount = 0;
        building = false;
        entityWithPathController.submitPath(Path);
    }

    public void SetColor(Color color)
    {
        this.pathRenderer.startColor = this.pathRenderer.endColor = color;
        color = new Color(color.r, color.g, color.b, 0.5f);
        this.previewRenderer.startColor = this.previewRenderer.endColor = color;
        foreach (var waypoint in waypointViewControllers)
        {
            waypoint.setColor(color);
        }
    }

    public void OnMouseDown()
    {
        //find closest linerenderer position to the click
        double min = double.MaxValue;
        Vector2 location = Vector2.zero;
        double dist;
        int index = 0;
        for (int i = 0; i < pathRenderer.positionCount; i++)
        {
            if ((dist = SnapController.FastEuclideanDistance(Camera.main.ScreenToWorldPoint(Input.mousePosition), pathRenderer.GetPosition(i))) < min)
            {
                min = dist;
                location = pathRenderer.GetPosition(i);
                index = i;
            }
        }
        //check if there is a waypoint at the position of the linerenderer position
        //waypoints visual position and actual location can differ
        //always disregard waypoint indicators from snapController here; rely on linerenderer
        WaypointViewController nearestWaypoint = null;
        foreach (WaypointViewController waypoint in waypointViewControllers)
        {
            if (waypoint.waypoint.Location.X == location.x && waypoint.waypoint.Location.Y == location.y)
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