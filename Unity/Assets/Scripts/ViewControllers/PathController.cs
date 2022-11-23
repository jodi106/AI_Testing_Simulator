using Entity;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    private LineRenderer lr;
    public Path path { get; set; }
    private bool building;
    private IBaseEntityWithPathController entityController;
    private SnapController snapController;

    private void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();
        this.path = new Path();

        lr = gameObject.GetComponent<LineRenderer>();
        building = true;

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            if(building)
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
                GameObject waypoint = snapController.findNearestWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                addWaypoint(waypoint.transform.position);
            }
        });
    }

    public void setEntityController(IBaseEntityWithPathController controller)
    {
        this.entityController = controller;
        lr.SetPosition(0, controller.getPosition());
    }

    public void Update()
    {
        if (building)
        {
            GameObject waypoint = snapController.findNearestWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            lr.SetPosition(lr.positionCount - 1, waypoint.transform.position);
        }
    }
    public void addWaypoint(Vector2 wp)
    {
        //TODO get angle and fix waypoint
        Waypoint waypoint = new Waypoint(new Location(wp, 0), new ActionType("defaultAction"), new List<TriggerInfo>());
        this.path.EventList.Add(waypoint);
        List<GameObject> path = snapController.findPath(lr.GetPosition(lr.positionCount - 2), wp);
        foreach(GameObject go in path)
        {
            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 2, go.transform.position);
        }
    }

    public void complete()
    {
        building = false;
        entityController.submitPath(path);
    }
}
