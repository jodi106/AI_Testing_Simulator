using Entity;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathController : MonoBehaviour
{
    private LineRenderer lr;
    private LineRenderer previewRenderer;
    public Path path { get; set; }
    private bool building;
    private IBaseEntityWithPathController entityController;
    private SnapController snapController;

    private void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();
        this.path = new Path();

        lr = gameObject.GetComponent<LineRenderer>();
        lr.SetWidth(0.1f, 0.1f);
        previewRenderer = gameObject.transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
        previewRenderer.SetWidth(0.1f, 0.1f);
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
        addWaypoint(controller.getPosition());
    }

    public void Update()
    {
        if (building)
        {
            GameObject waypoint = snapController.findNearestWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (lr.positionCount > 0)
            {
                List<GameObject> path = snapController.findPath(lr.GetPosition(lr.positionCount - 1), waypoint.transform.position);
                if(path is null)
                {
                    previewRenderer.positionCount = 0;
                    return;
                }
                previewRenderer.positionCount = path.Count;
                int i = 0;
                foreach (GameObject go in path)
                {
                    previewRenderer.SetPosition(i, go.transform.position);
                    i++;
                }
            }
        }
    }
    public void addWaypoint(Vector2 wp)
    {
        //TODO get angle and fix waypoint
        Waypoint waypoint = new Waypoint(new Location(wp, 0), new ActionType("defaultAction"), new List<TriggerInfo>());
        this.path.EventList.Add(waypoint);
        if (lr.positionCount > 0)
        {
            List<GameObject> path = snapController.findPath(lr.GetPosition(lr.positionCount - 1), wp);
            foreach (GameObject go in path)
            {
                lr.positionCount++;
                Vector3 position = new Vector3(go.transform.position.x, go.transform.position.y, -0.1f);
                lr.SetPosition(lr.positionCount - 1, position);
            }
        } else
        {
            lr.positionCount++;
            lr.SetPosition(0, wp);
        }
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
        this.lr.SetColors(color, color);
        color = new Color(color.r, color.g, color.b, 0.5f);
        this.previewRenderer.SetColors(color, color);
    }
}
