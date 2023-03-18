using Entity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathController : MonoBehaviour
{
    public GameObject waypointPrefab;

    private LineRenderer pathRenderer;
    private LineRenderer previewRenderer;
    private SpriteRenderer previewSprite;
    private EdgeCollider2D edgeCollider;

    private LinkedList<(WaypointViewController, int)> waypointViewControllers;
    public AdversaryViewController adversaryViewController { get; private set; }
    private SnapController snapController;
    private MainController mainController;

    private bool building;

    public Path Path { get; set; }

    private void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();
        this.mainController = Camera.main.GetComponent<MainController>();

        waypointViewControllers = new LinkedList<(WaypointViewController, int)>();

        pathRenderer = gameObject.GetComponent<LineRenderer>();
        pathRenderer.startWidth = pathRenderer.endWidth = 0.1f;

        previewRenderer = gameObject.transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
        previewRenderer.startWidth = previewRenderer.endWidth = 0.1f;

        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
        previewSprite = gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.PATH_DESELECTED);

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (building)
            {
                var action = new MouseClickAction(x);
                var position = FindMouseTarget(action.position);

                CollisionType waypointColliderType = findCollisionType(position.Vector3Ser.ToVector3());
                CollisionType mouseColliderType = findCollisionType(action.position);

                if (position is not null)
                {
                    if (Path.IsEmpty() || (waypointColliderType == CollisionType.None && mouseColliderType == CollisionType.None))
                    {
                        AddMoveToWaypoint(position.Vector3Ser.ToVector3());
                        mainController.setSelectedEntity(waypointViewControllers.Last.Value.Item1);
                    }
                }
            }
        });
    }

    public bool shouldIgnoreWaypoints()
    {
        return adversaryViewController.shouldIgnoreWaypoints();
    }

    public bool isBuilding()
    {
        return building;
    }

    public void select(bool forward = false)
    {
        adjustHeights(true);
        previewSprite.enabled = true;
        building = true;
        if (forward)
        {
            adversaryViewController.select();
        }
    }

    public void adjustHeights(bool selected)
    {
        for (var i = 0; i < pathRenderer.positionCount; i++)
        {
            pathRenderer.SetPosition(i, HeightUtil.SetZ(pathRenderer.GetPosition(i), selected ? HeightUtil.PATH_SELECTED : HeightUtil.PATH_DESELECTED));
        }
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, selected ? HeightUtil.PATH_SELECTED : HeightUtil.PATH_DESELECTED);
        foreach (var (waypoint, _) in waypointViewControllers)
        {
            waypoint.gameObject.transform.position = HeightUtil.SetZ(waypoint.gameObject.transform.position, selected ? HeightUtil.WAYPOINT_SELECTED : HeightUtil.WAYPOINT_DESELECTED);
        }
    }

    public void deselect(bool forward = false)
    {
        adjustHeights(false);
        previewRenderer.positionCount = 0;
        previewSprite.enabled = false;
        building = false;
        if (forward)
        {
            adversaryViewController.deselect();
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

    public void Init(AdversaryViewController controller, Adversary v, bool building = true)
    {
        Path = v.Path;
        if(v.Color is not null) this.SetColor(v.Color.ToUnityColor());
        this.adversaryViewController = controller;
        this.building = building;
        if (v.Path.WaypointList.Count == 0)
        {
            AddMoveToWaypoint(controller.getPosition()); // have PathController create Waypoint
        }
        else
        {
            foreach (Waypoint w in v.Path.WaypointList)
            {
                var pos = w.Location.Vector3Ser.ToVector3();
                AddMoveToWaypoint(pos, w, false); //waypoint already exists, dont create it
            }
        }
        adjustHeights(false);
        waypointViewControllers.First.Value.Item1.gameObject.SetActive(false);
    }

    //Target of a click is either a waypoint or the mouse position itself, if waypoints are ignored
    public Location FindMouseTarget(Vector2 mousePosition)
    {
        Location waypoint;
        if (!this.adversaryViewController.shouldIgnoreWaypoints())
        {
            waypoint = snapController.FindWaypoint(mousePosition);
        }
        else
        {
            waypoint = new Location(mousePosition);
        }
        return waypoint;
    }

    CollisionType findCollisionType(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(HeightUtil.SetZ(position, -10), -Vector2.up);
        if (hit.collider == this.edgeCollider)
        {
            return CollisionType.Path;
        }
        if (hit.collider == adversaryViewController.getCollider())
        {
            return CollisionType.Vehicle;
        }
        foreach (var entry in waypointViewControllers)
        {
            var waypointController = entry.Item1;
            var collider = waypointController.gameObject.GetComponent<CircleCollider2D>();
            if (hit.collider == collider)
            {
                return CollisionType.Waypoint;
            }
        }
        return CollisionType.None;
    }

    /*
     * Only updates the preview renderer. Does not change the model in any way.
     */
    public void Update()
    {
        if (MainController.freeze) return;
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (building)
        {
            var target = FindMouseTarget(mousePosition);

            CollisionType targetColliderType = findCollisionType(target.Vector3Ser.ToVector3());
            CollisionType mouseColliderType = findCollisionType(mousePosition);

            if (targetColliderType == CollisionType.Path || mouseColliderType == CollisionType.Path)
            {
                previewSprite.enabled = true;
                previewSprite.transform.position = HeightUtil.SetZ(target.Vector3Ser.ToVector3(), -0.1f);
                previewRenderer.positionCount = 0;
                return;
            }
            if (targetColliderType == CollisionType.Vehicle ||
                mouseColliderType == CollisionType.Vehicle ||
                targetColliderType == CollisionType.Waypoint ||
                mouseColliderType == CollisionType.Waypoint)
            {
                previewSprite.enabled = false;
                previewRenderer.positionCount = 0;
                return;
            }

            previewRenderer.positionCount = 1;
            previewRenderer.SetPosition(0, HeightUtil.SetZ(waypointViewControllers.Last.Value.Item1.transform.position, HeightUtil.PATH_SELECTED));

            (var path, _) = snapController.FindPath(waypointViewControllers.Last.Value.Item1.transform.position, target.Vector3Ser.ToVector3(), this.adversaryViewController.shouldIgnoreWaypoints());
            path.RemoveAt(0);

            foreach (var coord in path)
            {
                previewRenderer.SetPosition(previewRenderer.positionCount++, HeightUtil.SetZ(coord, HeightUtil.PATH_SELECTED));
            }

            previewSprite.enabled = true;
            previewSprite.transform.position = HeightUtil.SetZ(target.Vector3Ser.ToVector3(), -0.1f);
        }
        else
        {
            var waypoint = snapController.FindWaypoint(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(HeightUtil.SetZ(mousePosition, -10), -Vector2.up);
            if (hit.collider == this.edgeCollider)
            {
                previewSprite.enabled = true;
                previewSprite.transform.position = HeightUtil.SetZ(waypoint.Vector3Ser.ToVector3(), -0.1f);
            }
            else
            {
                previewSprite.enabled = false;
            }
        }
    }

    /*
     * Add a Waypoint to the end of the path.
     * If the optional waypoint is passed, no Waypoint will be generated and appended to Path.
     * waypointViewControllers and pathRenderer are always in sync.
     * Path may have more Waypoints than there are controllers in waypointsViewControllers, if the controller is being generated from an existing Path.
     */
    public void AddMoveToWaypoint(Vector2 position, Waypoint waypoint = null, bool addLaneChanges = true)
    {
        var pathLen = 0;
        var laneChanges = new List<int>();
        var path = new List<Vector2>();

        if (waypointViewControllers.Count == 0)
        {
            pathRenderer.SetPosition(pathRenderer.positionCount++, HeightUtil.SetZ(position, HeightUtil.PATH_SELECTED));
        }
        else
        {
            (path, laneChanges) = snapController.FindPath(waypointViewControllers.Last.Value.Item1.transform.position, position, this.adversaryViewController.shouldIgnoreWaypoints());
            path.RemoveAt(0);
            pathLen = path.Count;

            if (path is null)
            {
                return; //this kinda shouldnt happen
            }

            foreach (var coord in path)
            {
                pathRenderer.SetPosition(pathRenderer.positionCount++, HeightUtil.SetZ(coord, HeightUtil.PATH_SELECTED));
            }
        }
        var used = 0;
        if(addLaneChanges)
        {
            used = addLaneChangeWaypoints(laneChanges, path);
        }
        WaypointViewController viewController;
        if (waypoint is not null)
        {
            viewController = createWaypointGameObject(position.x, position.y, false, waypoint);
        }
        else //waypoint is already in Path
        {
            viewController = createWaypointGameObject(position.x, position.y);
            this.Path.WaypointList.Add(viewController.waypoint);
        }
        waypointViewControllers.AddLast((viewController, pathLen - used));
        afterEdit();
    }

    /*
     * Add secondary waypoints to the Path and waypointViewControllers.
     * If a linked list node is passed then the generated waypoint and and waypointViewController will be inserted after the node.
     * Otherwise they are added to the end of both containers.
     */
    public int addLaneChangeWaypoints(List<int> laneChanges, List<Vector2> path, LinkedListNode<(WaypointViewController, int)>? node = null)
    {
        laneChanges.Sort();
        var used = 0;
        var pathLen = 0;
        foreach (var laneChange in laneChanges)
        {
            WaypointViewController viewController = null;
            if (laneChange != 0)
            {
                pathLen = laneChange - used;
                viewController = createWaypointGameObject(path[laneChange].x, path[laneChange].y, true);
                used += laneChange - used;
                if (node is not null)
                {
                    Path.WaypointList.Insert(Path.WaypointList.IndexOf(node.Value.Item1.waypoint) + 1, viewController.waypoint);
                    waypointViewControllers.AddAfter(node, (viewController, pathLen));
                    node = node.Next;
                }
                else
                {
                    this.Path.WaypointList.Add(viewController.waypoint);
                    waypointViewControllers.AddAfter(waypointViewControllers.Last, (viewController, pathLen));
                }
            }

            if (laneChange <= path.Count() - 3)
            {
                pathLen = 1;
                viewController = createWaypointGameObject(path[laneChange + 1].x, path[laneChange + 1].y, true);
                used += 1;
                if (node is not null)
                {
                    Path.WaypointList.Insert(Path.WaypointList.IndexOf(node.Value.Item1.waypoint) + 1, viewController.waypoint);
                    waypointViewControllers.AddAfter(node, (viewController, pathLen));
                    node = node.Next;
                }
                else
                {
                    this.Path.WaypointList.Add(viewController.waypoint);
                    waypointViewControllers.AddAfter(waypointViewControllers.Last, (viewController, pathLen));
                }
            }

        }

        return used;
    }

    WaypointViewController createWaypointGameObject(float x, float y, bool secondary = false, Waypoint w = null)
    {
        GameObject wpGameObject = Instantiate(waypointPrefab, new Vector3(x, y, HeightUtil.WAYPOINT_SELECTED), Quaternion.identity);
        WaypointViewController viewController = wpGameObject.GetComponent<WaypointViewController>();
        viewController.setPathController(this);
        viewController.setColor(pathRenderer.startColor);
        viewController.setIgnoreWaypoints(this.shouldIgnoreWaypoints());
        Waypoint waypoint;
        if (w is not null)
        {
            waypoint = w;
        }
        else
        {
            waypoint = generateWaypoint(new Location(new Vector3(x, y, 0), 0), new ActionType("MoveToAction"));
        }
        viewController.waypoint = waypoint;
        viewController.waypoint.View = viewController;
        if (secondary) viewController.makeSecondary();
        return viewController;
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

    /*
     * Given a waypointViewController, finds the first predecessor and successor which are not secondary waypoints
     * and optionally deletes all secondary waypoints along the path.
     * prevIndex indicates which pathRenderer position the previous non-secondary waypoint corresponds to.
     * This value is neccessary in order to restore the pathrenderer positions after an edit.
     */
    private (LinkedListNode<(WaypointViewController, int)>,
        LinkedListNode<(WaypointViewController, int)>,
        LinkedListNode<(WaypointViewController, int)>,
        int)
        getNeighbors(WaypointViewController waypointController, bool removeSecondaries = true)
    {
        LinkedListNode<(WaypointViewController, int)> prev = null, next = null, cur = null;
        int prevIndex = 0;
        for (LinkedListNode<(WaypointViewController, int)> wp = waypointViewControllers.First; wp != null; wp = wp.Next)
        {
            if (wp.Value.Item1 == waypointController)
            {
                cur = wp;
                prev = wp.Previous;
                if (removeSecondaries)
                {
                    while (prev != null && prev.Value.Item1.isSecondary())
                    {
                        var tmp = prev.Previous;
                        removeWaypoint(prev.Value.Item1, false);
                        Destroy(prev.Value.Item1.gameObject);
                        prevIndex -= prev.Value.Item2;
                        prev = tmp;
                    }
                }
                next = wp.Next;
                if (removeSecondaries)
                {
                    while (next != null && next.Value.Item1.isSecondary())
                    {
                        var tmp = next.Next;
                        removeWaypoint(next.Value.Item1, false);
                        Destroy(next.Value.Item1.gameObject);
                        next = tmp;
                    }
                }
                break;
            }
            else
            {
                prevIndex += wp.Value.Item2;
            }
        }
        return (prev, cur, next, prevIndex);
    }

    public void MoveWaypoint(WaypointViewController waypointController, float x, float y)
    {
        var (prev, cur, next, prevIndex) = getNeighbors(waypointController);

        waypointController.waypoint.setPosition(x, y);

        List<Vector2> prevPath = new List<Vector2>();
        List<Vector2> nextPath = new List<Vector2>();

        bool ignoreWaypoints = waypointController.shouldIgnoreWaypoints();

        var offset = 0;
        var usedPrev = 0;
        var usedNext = 0;
        if (prev != null)
        {
            var laneChanges = new List<int>();
            (prevPath, laneChanges) = snapController.FindPath(prev.Value.Item1.waypoint.Location.Vector3Ser.ToVector3(), new Vector3(x, y, 0), ignoreWaypoints || prev.Value.Item1.shouldIgnoreWaypoints());
            prev.Value.Item1.waypoint.setPosition(prevPath[0].x, prevPath[0].y);
            prevPath.RemoveAt(0);
            offset = offset + prevPath.Count - cur.Value.Item2;
            usedPrev = addLaneChangeWaypoints(laneChanges, prevPath, prev);
        }
        if (next != null)
        {
            var laneChanges = new List<int>();
            (nextPath, laneChanges) = snapController.FindPath(new Vector3(x, y, 0), next.Value.Item1.waypoint.Location.Vector3Ser.ToVector3(), ignoreWaypoints || next.Value.Item1.shouldIgnoreWaypoints());
            next.Value.Item1.waypoint.setPosition(nextPath[nextPath.Count - 1].x, nextPath[nextPath.Count - 1].y);
            nextPath.RemoveAt(0);
            offset = offset + nextPath.Count - next.Value.Item2;
            usedNext = addLaneChangeWaypoints(laneChanges, nextPath, cur);
        }

        Vector2[] positions = new Vector2[pathRenderer.positionCount + offset];

        for (var i = 0; i <= prevIndex; i++)
        {
            positions[i] = pathRenderer.GetPosition(i);
        }

        for (var i = 0; i < prevPath.Count; i++)
        {
            positions[prevIndex + i + 1] = prevPath[i];
        }

        positions[prevIndex + prevPath.Count] = new Vector2(x, y);

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

        cur.Value = (cur.Value.Item1, prevPath.Count - usedPrev);
        if (next is not null)
        {
            next.Value = (next.Value.Item1, nextPath.Count - usedNext);
        }

        waypointController.waypoint.setPosition(x, y);
        mainController.moveActionButtons(new Vector3(x, y, 0));
        afterEdit();
    }

    public void removeWaypoint(WaypointViewController controller, bool restorePath = true)
    {
        var (prev, cur, next, prevIndex) = getNeighbors(controller, restorePath);

        // dont allow destruction of first waypoint
        if (prev == null)
        {
            return;
        }

        var used = 0;
        if (restorePath)
        {
            if (next != null)
            {
                bool ignoreWaypoints = controller.shouldIgnoreWaypoints();
                var (path, laneChanges) = snapController.FindPath(prev.Value.Item1.waypoint.Location.Vector3Ser.ToVector3(), next.Value.Item1.waypoint.Location.Vector3Ser.ToVector3(), ignoreWaypoints || next.Value.Item1.shouldIgnoreWaypoints());
                path.RemoveAt(0);
                used = addLaneChangeWaypoints(laneChanges, path, prev);
                var offset = path.Count - cur.Value.Item2 - next.Value.Item2;

                Vector2[] positions = new Vector2[pathRenderer.positionCount + offset];

                for (var i = 0; i <= prevIndex; i++)
                {
                    positions[i] = pathRenderer.GetPosition(i);
                }

                for (var i = 0; i < path.Count; i++)
                {
                    positions[prevIndex + i + 1] = path[i];
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
                next.Value = (next.Value.Item1, path.Count - used);

            }
            else
            {
                pathRenderer.positionCount -= cur.Value.Item2;
            }
        }
        else
        {
            // secondary waypoints were not destroyed, therefore add the index of cur to the next, possibly secondary waypoint
            if (next is not null) next.Value = (next.Value.Item1, next.Value.Item2 + cur.Value.Item2);
        }

        Path.WaypointList.Remove(controller.waypoint);
        waypointViewControllers.Remove(cur);
        afterEdit();
    }

    public void afterEdit()
    {
        if (adversaryViewController.shouldIgnoreWaypoints())
        {
            if (waypointViewControllers.Count >= 2)
            {
                var adversary = adversaryViewController.getEntity();
                var direction = waypointViewControllers.First.Next.Value.Item1.getLocation().Vector3Ser.ToVector3();
                Vector3 vectorToTarget = direction - adversary.SpawnPoint.Vector3Ser.ToVector3();
                vectorToTarget = HeightUtil.SetZ(vectorToTarget, 0);
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                adversary.setRotation(angle);
            }
            else
            {
                adversaryViewController.getEntity().setRotation(0);
            }
        }
        resetEdgeCollider();
    }

    public void MoveFirstWaypoint(float x, float y)
    {
        var first = this.waypointViewControllers.First.Value.Item1.getLocation();
        //prevent stackoverflow from onChangeLocation callback
        if (first.X != x || first.Y != y)
        {
            this.MoveWaypoint(this.waypointViewControllers.First.Value.Item1, x, y);
        }
    }

    public void SetColor(Color color)
    {
        this.pathRenderer.startColor = this.pathRenderer.endColor = color;
        color = new Color(color.r, color.g, color.b, 0.5f);
        this.previewRenderer.startColor = this.previewRenderer.endColor = color;
        this.previewSprite.color = color;
        foreach (var (waypoint, _) in waypointViewControllers)
        {
            waypoint.setColor(color);
        }
    }

    public void OnMouseDown()
    {
        if (MainController.freeze) return;
        if (snapController.IgnoreClicks && !building)
        {
            EventManager.TriggerEvent(new MouseClickAction(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            return;
        }
        //find closest linerenderer position to the click
        double min = double.MaxValue;
        Vector2 location = Vector2.zero;
        double dist = double.MaxValue;
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
            viewController.waypoint.View = viewController;

            var cur = waypointViewControllers.First;
            var curPathIndex = 0;
            while (index - cur.Next.Value.Item2 > 0)
            {
                index -= cur.Next.Value.Item2;
                cur = cur.Next;
                curPathIndex++;
            }
            var next = cur.Next;
            waypointViewControllers.AddAfter(cur, (viewController, index));
            next.Value = (next.Value.Item1, next.Value.Item2 - index);

            this.Path.WaypointList.Insert(curPathIndex + 1, viewController.waypoint);

            MoveWaypoint(viewController, viewController.waypoint.Location.X, viewController.waypoint.Location.Y); // fix paths / deleting waypoint may make A* necessary
            afterEdit();

            mainController.setSelectedEntity(viewController);
        }
    }

    //TODO find better solution
    public WaypointViewController getFirstWaypointController()
    {
        return this.waypointViewControllers.First.Value.Item1;
    }
}

//Collider type for a given position. None means position is over map collider
enum CollisionType
{
    Path,
    Vehicle,
    Waypoint,
    None
}