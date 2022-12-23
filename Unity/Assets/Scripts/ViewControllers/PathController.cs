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
    private IBaseEntityWithPathController entityController;
    private SnapController snapController;

    private bool building;

    public Path Path { get; set; }
    public Lane CurrentLane { get; set; }
    public AStarWaypoint CurrentWaypoint { get; set; }

    private void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();

        this.Path = new Path();

        waypoints = new List<WaypointViewController>();

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineRenderer.endWidth = 0.1f;

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
        for (var i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, HeightUtil.SetZ(lineRenderer.GetPosition(i), HeightUtil.PATH_SELECTED));
        }

        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.PATH_SELECTED);
        foreach (var waypoint in waypoints)
        {
            waypoint.gameObject.transform.position = HeightUtil.SetZ(waypoint.gameObject.transform.position, HeightUtil.WAYPOINT_SELECTED);
        }
    }

    public void Deselect()
    {
        for (var i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, HeightUtil.SetZ(lineRenderer.GetPosition(i), HeightUtil.PATH_DESELECTED));
        }
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.PATH_DESELECTED);
        foreach (var waypoint in waypoints)
        {
            waypoint.gameObject.transform.position = HeightUtil.SetZ(waypoint.gameObject.transform.position, HeightUtil.WAYPOINT_DESELECTED);
        }
    }



    public void Destroy()
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
        AddMoveToWaypoint(controller.getPosition()); //init with starting position of car
    }

    public void Update()
    {
        if (building)
        {
            var (nextLane, nextWaypoint) = snapController.FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            previewRenderer.SetPosition(0, HeightUtil.SetZ(CurrentWaypoint.Location.Vector3, HeightUtil.PATH_SELECTED));
            previewRenderer.positionCount = 1;

            if (nextLane.RoadId == CurrentLane.RoadId && nextLane.Id * CurrentLane.Id > 0 && // user wants to change lanes
                 (nextLane.Id == CurrentLane.Id + 1 || nextLane.Id == CurrentLane.Id - 1))   // lanes are next to eachother
            {
                previewRenderer.SetPosition(previewRenderer.positionCount++, HeightUtil.SetZ(nextWaypoint.Location.Vector3, HeightUtil.PATH_SELECTED));
            }
            else
            {
                var path = snapController.FindPath(CurrentWaypoint.Location.Vector3, nextWaypoint.Location.Vector3);

                foreach (var coord in path)
                {
                    previewRenderer.SetPosition(previewRenderer.positionCount++, HeightUtil.SetZ(coord, HeightUtil.PATH_SELECTED));
                }
            }
        }
    }

    public void AddMoveToWaypoint(Vector2 mousePosition)
    {

        var (nextLane, nextWaypoint) = snapController.FindLaneAndWaypoint(mousePosition);


        GameObject wpGameObject = Instantiate(waypointPrefab, new Vector3(nextWaypoint.Location.X, nextWaypoint.Location.Y, HeightUtil.WAYPOINT_SELECTED), Quaternion.identity);

        WaypointViewController viewController = wpGameObject.GetComponent<WaypointViewController>();

        if (Path.IsEmpty())
        {
            lineRenderer.SetPosition(lineRenderer.positionCount++, HeightUtil.SetZ(nextWaypoint.Location.Vector3, HeightUtil.PATH_SELECTED));
            viewController.waypoint = new Waypoint(new Location(nextWaypoint.Location.Vector3, 0), new ActionType("MoveToAction"), new List<TriggerInfo>()); //is this moveTo?

        }
        else if (nextLane.RoadId == CurrentLane.RoadId && nextLane.Id * CurrentLane.Id > 0 && // user wants to change lanes
               (nextLane.Id == CurrentLane.Id + 1 || nextLane.Id == CurrentLane.Id - 1)) // lanes are next to eachother
        {
            lineRenderer.SetPosition(lineRenderer.positionCount++, HeightUtil.SetZ(nextWaypoint.Location.Vector3, HeightUtil.PATH_SELECTED));

            viewController.waypoint = new Waypoint(new Location(nextWaypoint.Location.Vector3, 0), new ActionType("LaneChangeAction"), new List<TriggerInfo>());
        }
        else
        {
            var path = snapController.FindPath(CurrentWaypoint.Location.Vector3, nextWaypoint.Location.Vector3);

            foreach (var coord in path)
            {
                lineRenderer.SetPosition(lineRenderer.positionCount++, HeightUtil.SetZ(coord, HeightUtil.PATH_SELECTED));
            }

            viewController.waypoint = new Waypoint(new Location(nextWaypoint.Location.Vector3, 0), new ActionType("MoveToAction"), new List<TriggerInfo>());
        }

        this.Path.WaypointList.Add(viewController.waypoint);

        waypoints.Add(viewController);

        SpriteRenderer s = wpGameObject.GetComponent<SpriteRenderer>();

        s.color = new Color(0, 0, 0);

        Vector2[] positions = new Vector2[lineRenderer.positionCount];
        for (var i = 0; i < lineRenderer.positionCount; i++)
        {
            positions[i] = lineRenderer.GetPosition(i);
        }
        edgeCollider.SetPoints(positions.ToList());

        CurrentWaypoint = nextWaypoint;
        CurrentLane = nextLane;
    }

    public void MoveWaypoint(Waypoint waypoint, Vector2 position)
    {

    }

    public void Complete()
    {
        previewRenderer.positionCount = 0;
        building = false;
        entityController.submitPath(Path);
    }

    public void SetColor(Color color)
    {
        this.lineRenderer.startColor = this.lineRenderer.endColor = color;
        color = new Color(color.r, color.g, color.b, 0.5f);
        this.previewRenderer.startColor = this.previewRenderer.endColor = color;
        foreach (var waypoint in waypoints)
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