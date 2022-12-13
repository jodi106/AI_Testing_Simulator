using Entity;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private LineRenderer previewRenderer;
    public Path path { get; set; }
    private bool building;
    private IBaseEntityWithPathController entityController;
    private SnapController snapController;

    private void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();
        this.path = new Path();

        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.SetWidth(0.1f, 0.1f);

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
                var (_, waypoint) = snapController.FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                AddWaypoint(waypoint.Location.Vector3);
            }
        });
    }

    public void SetEntityController(IBaseEntityWithPathController controller)
    {
        this.entityController = controller;
        AddWaypoint(controller.getPosition());
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
                    Debug.Log(location.Vector3.ToString());
                    previewRenderer.SetPosition(i, location.Vector3);
                    i++;
                }
            }
        }
    }
    public void AddWaypoint(Vector2 wp)
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

                Debug.Log(position.ToString());

                lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
            }
        } else
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(0, wp);
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
        this.lineRenderer.SetColors(color, color);
        color = new Color(color.r, color.g, color.b, 0.5f);
        this.previewRenderer.SetColors(color, color);
    }
}
