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
        var waypoint = snapController.FindWaypoint(mousePosition);

        if (waypoint == null)
        {
            Debug.Log("Invalid mouse position, mouse probably not on road!");
            return;
        }

        LineRenderer lineRenderer;

        if (preview && !Path.IsEmpty())
        {
            lineRenderer = previewRenderer;
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, HeightUtil.SetZ(waypointViewControllers.Last.Value.Item1.transform.position, HeightUtil.PATH_SELECTED));
        }
        else
        {
            lineRenderer = pathRenderer;
        }

        //var actionType = new ActionType("MoveToAction");

        var pathLen = 0;

        if (Path.IsEmpty())
        {
            lineRenderer.SetPosition(lineRenderer.positionCount++, HeightUtil.SetZ(waypoint.Vector3, HeightUtil.PATH_SELECTED));
        }
        else
        {
            var path = new List<Vector2>();
            (path, _) = snapController.FindPath(waypointViewControllers.Last.Value.Item1.transform.position, waypoint.Vector3);

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

            GameObject wpGameObject = Instantiate(waypointPrefab, new Vector3(waypoint.X, waypoint.Y, HeightUtil.WAYPOINT_SELECTED), Quaternion.identity);

            WaypointViewController viewController = wpGameObject.GetComponent<WaypointViewController>();
            viewController.setPathController(this);
            viewController.setColor(pathRenderer.startColor);
            viewController.waypoint = generateWaypoint(new Location(waypoint.Vector3, 0), new ActionType("MoveToAction"));
            viewController.waypoint.setView(viewController);
            this.Path.WaypointList.Add(viewController.waypoint);

            waypointViewControllers.AddLast((viewController, pathLen));
            resetEdgeCollider();
        }

    }

    Waypoint generateWaypoint(Location loc, ActionType actionType)
    {
        List<TriggerInfo> triggersLaneChange = new List<TriggerInfo>();
        var locationTrigger = new Location(0, 0, 0, 0); // var locationTrigger = nextWaypoint.Location;
        if (!Path.IsEmpty())
        {
            locationTrigger = waypointViewControllers.Last.Value.Item1.waypoint.Location;
        }
        triggersLaneChange.Add(new TriggerInfo("DistanceCondition", null, "lessThan", 20, locationTrigger)); // TODO change 20
        return new Waypoint(loc, actionType, triggersLaneChange);
    }

    void resetEdgeCollider()
    {
        Vector2[] positions = new Vector2[pathRenderer.positionCount];
        for (var i = 0; i < pathRenderer.positionCount; i++)
        {
            positions[i] = pathRenderer.GetPosition(i);
        }
        edgeCollider.SetPoints(positions.ToList());
    }

    public void MoveWaypoint(WaypointViewController waypointController, Location location)
    {
        LinkedListNode<(WaypointViewController, int)> prev = null, next = null, cur = null;
        int prevIndex = 0;
        for (LinkedListNode<(WaypointViewController, int)> wp = waypointViewControllers.First; wp != null; wp = wp.Next)
        {
            if (wp.Value.Item1 == waypointController)
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

        waypointController.waypoint.setLocation(location);

        List<Vector2> prevPath = new List<Vector2>();
        List<Vector2> nextPath = new List<Vector2>();

        var offset = 0;
        if (prev != null)
        {
            (prevPath, _) = snapController.FindPath(prev.Value.Item1.waypoint.Location.Vector3, location.Vector3);
            prevPath.RemoveAt(prevPath.Count - 1);
            offset = offset + prevPath.Count - cur.Value.Item2;
        }
        if (next != null)
        {
            (nextPath, _) = snapController.FindPath(location.Vector3, next.Value.Item1.waypoint.Location.Vector3);
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

        positions[prevIndex + prevPath.Count] = location.Vector3;

        for (var i = 0; i < nextPath.Count; i++)
        {
            positions[prevIndex + prevPath.Count + i + 1] = nextPath[i];
        }

        for (var i = prevIndex + prevPath.Count + nextPath.Count + 1; i < pathRenderer.positionCount + offset; i++)
        {
            positions[i] = pathRenderer.GetPosition(i - offset);
        }

        pathRenderer.positionCount = pathRenderer.positionCount + offset;
        for (var i = 0; i < pathRenderer.positionCount; i++)
        {
            pathRenderer.SetPosition(i, new Vector3(positions[i].x, positions[i].y, transform.position.z));
        }

        cur.Value = (cur.Value.Item1, prevPath.Count);
        if (next is not null)
        {
            next.Value = (next.Value.Item1, nextPath.Count);
        }

        edgeCollider.SetPoints(positions.ToList());
    }

    public void removeWaypoint(WaypointViewController controller)
    {
        LinkedListNode<(WaypointViewController, int)> prev = null, next = null, cur = null;
        int prevIndex = 0;
        for (LinkedListNode<(WaypointViewController, int)> wp = waypointViewControllers.First; wp != null; wp = wp.Next)
        {
            if (wp.Value.Item1.waypoint == controller.waypoint)
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

        // dont allow destruction of first waypoint
        if (prev == null)
        {
            return;
        }

        if (next != null)
        {
            (var path, _) = snapController.FindPath(prev.Value.Item1.waypoint.Location.Vector3, next.Value.Item1.waypoint.Location.Vector3);
            path.RemoveAt(path.Count - 1);
            var offset = path.Count - cur.Value.Item2 - next.Value.Item2;

            Vector2[] positions = new Vector2[pathRenderer.positionCount + offset];

            for (var i = 0; i < prevIndex; i++)
            {
                positions[i] = pathRenderer.GetPosition(i);
            }

            for (var i = 0; i < path.Count; i++)
            {
                positions[prevIndex + i] = path[i];
            }

            for (var i = prevIndex + path.Count; i < pathRenderer.positionCount + offset; i++)
            {
                positions[i] = pathRenderer.GetPosition(i - offset);
            }

            pathRenderer.positionCount = pathRenderer.positionCount + offset;
            for (var i = 0; i < pathRenderer.positionCount; i++)
            {
                pathRenderer.SetPosition(i, new Vector3(positions[i].x, positions[i].y, transform.position.z));
            }
            next.Value = (next.Value.Item1, path.Count);

            edgeCollider.SetPoints(positions.ToList());

        }
        else
        {
            pathRenderer.positionCount -= cur.Value.Item2;
        }

        //TODO: fix path object

        waypointViewControllers.Remove(cur);
    }

    public void MoveFirstWaypoint(Location location)
    {
        this.MoveWaypoint(this.waypointViewControllers.First.Value.Item1, location);
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
            nearestWaypoint.select();
        }
        else
        {
            GameObject wpGameObject = Instantiate(waypointPrefab, new Vector3(location.x, location.y, HeightUtil.WAYPOINT_SELECTED), Quaternion.identity);

            WaypointViewController viewController = wpGameObject.GetComponent<WaypointViewController>();
            viewController.setPathController(this);
            viewController.setColor(pathRenderer.startColor);
            viewController.waypoint = generateWaypoint(new Location(location, 0), new ActionType("MoveToAction"));
            viewController.waypoint.setView(viewController);

            var cur = waypointViewControllers.First;
            while (index - cur.Next.Value.Item2 > 0)
            {
                index -= cur.Next.Value.Item2;
                cur = cur.Next;
            }
            var next = cur.Next;
            waypointViewControllers.AddAfter(cur, (viewController, index));
            next.Value = (next.Value.Item1, next.Value.Item2 - index);

            //TODO: insert into path
            //this.Path.WaypointList.Add(viewController.waypoint);
        }
    }
}