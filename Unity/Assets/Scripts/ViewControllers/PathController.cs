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

    private LinkedList<(WaypointViewController, int)> waypointViewControllers;
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

        waypointViewControllers = new LinkedList<(WaypointViewController, int)>();

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
        foreach (var (waypoint, _) in waypointViewControllers)
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

        foreach (var (waypoint, _) in waypointViewControllers)
        {
            waypoint.gameObject.transform.position = HeightUtil.SetZ(waypoint.gameObject.transform.position, HeightUtil.WAYPOINT_DESELECTED);
        }
    }

    public void Destroy()
    {
        foreach (var (wp, _) in waypointViewControllers)
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

        if (nextLane == null || nextWaypoint == null)
        {
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
        }
        else
        {
            lineRenderer = pathRenderer;
        }

        var actionType = new ActionType("");

        var pathLen = 0;

        if (Path.IsEmpty())
        {
            actionType = new ActionType("MoveToAction");
            lineRenderer.SetPosition(lineRenderer.positionCount++, HeightUtil.SetZ(nextWaypoint.Location.Vector3, HeightUtil.PATH_SELECTED));
        }
        else
        {
            var path = new List<Vector2>();
            (path, actionType) = snapController.FindPath(CurrentWaypoint.Location.Vector3, nextWaypoint.Location.Vector3);

            path.RemoveAt(0);
            pathLen = path.Count;

            if (path is null)
            {
                return; //this kinda shouldnt happen
            }

            foreach (var coord in path)
            {
                lineRenderer.SetPosition(lineRenderer.positionCount++, HeightUtil.SetZ(coord, HeightUtil.PATH_SELECTED));
            }

        }

        if (!preview)
        {

            GameObject wpGameObject = Instantiate(waypointPrefab, new Vector3(nextWaypoint.Location.X, nextWaypoint.Location.Y, HeightUtil.WAYPOINT_SELECTED), Quaternion.identity);
            SpriteRenderer s = wpGameObject.GetComponent<SpriteRenderer>();
            s.color = pathRenderer.startColor;

            WaypointViewController viewController = wpGameObject.GetComponent<WaypointViewController>();

            viewController.waypoint = new Waypoint(new Location(nextWaypoint.Location.Vector3, 0), actionType, new List<TriggerInfo>());
            viewController.setPathController(this);

            this.Path.WaypointList.Add(viewController.waypoint);

            waypointViewControllers.AddLast((viewController, pathLen));

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

    public void MoveWaypoint(WaypointViewController waypointController, Vector2 position)
    {
        LinkedListNode<(WaypointViewController, int)> prev = null, next = null, cur = null;
        int prevIndex = 0;
        for (LinkedListNode<(WaypointViewController, int)> wp = waypointViewControllers.First; wp != null; wp = wp.Next)
        {
            if (wp.Value.Item1.waypoint == waypointController.waypoint)
            {
                cur = wp;
                prev = wp.Previous;
                next = wp.Next;
                break;
            }
            else
            {
                prevIndex += wp.Value.Item2;
            }
        }

        if (cur == null)
        {
            return;
        }
        else
        {
            cur.Value.Item1.waypoint.Location = new Location(position);
            cur.Value.Item1.transform.position = position;
        }

        List<Vector2> prevPath = new List<Vector2>();
        List<Vector2> nextPath = new List<Vector2>();

        var offset = 0;
        if (prev != null)
        {
            (prevPath,_) = snapController.FindPath(prev.Value.Item1.waypoint.Location.Vector3, position);
            prevPath.RemoveAt(0);
            offset = offset + prevPath.Count - cur.Value.Item2;
        }
        if (next != null)
        {
            (nextPath,_) = snapController.FindPath(position, next.Value.Item1.waypoint.Location.Vector3);
            nextPath.RemoveAt(0);
            offset = offset + nextPath.Count - next.Value.Item2;
        }

        Vector2[] positions = new Vector2[pathRenderer.positionCount + offset];

        for (var i = 0; i < prevIndex; i++)
        {
            positions[i] = pathRenderer.GetPosition(i);
        }

        for (var i = 0; i < prevPath.Count; i++)
        {
            positions[prevIndex + i] = prevPath[i];
        }

        for (var i = 0; i < nextPath.Count; i++)
        {
            positions[prevIndex + prevPath.Count + i] = nextPath[i];
        }

        for (var i = prevIndex + prevPath.Count + nextPath.Count; i < pathRenderer.positionCount + offset; i++)
        {
            positions[i] = pathRenderer.GetPosition(i - offset);
        }

        pathRenderer.positionCount = pathRenderer.positionCount + offset;
        for (var i = 0; i < pathRenderer.positionCount; i++)
        {
            pathRenderer.SetPosition(i, positions[i]);
        }

        cur.Value = (cur.Value.Item1, prevPath.Count);
        if (next is not null)
        {
            next.Value = (next.Value.Item1, nextPath.Count);
        }

        //TODO: recompute edgecollider
    }


    public void MoveFirstWaypoint(Vector2 position)
    {
        this.MoveWaypoint(this.waypointViewControllers.First.Value.Item1, position);
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
        foreach (var (waypoint, _) in waypointViewControllers)
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
        foreach (var (waypoint, _) in waypointViewControllers)
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