using Models;
using UnityEngine;

public class PathController : MonoBehaviour
{
    private LineRenderer lr;
    public Path path { get; set; }
    private bool building;
    int len = 1;
    private IBaseEntityController entityController;
    private SnapController snapController;

    private void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();

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
            var action = new MouseClickAction(x);
            GameObject waypoint = snapController.findNearestWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            addWaypoint(waypoint.transform.position);
        });
    }

    public void setEntityController(IBaseEntityController controller)
    {
        this.entityController = controller;
        lr.SetPosition(0, controller.getPosition());
    }

    public void Update()
    {
        if (building)
        {
            GameObject waypoint = snapController.findNearestWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            lr.SetPosition(len, waypoint.transform.position);
        }
    }
    //TODO: change to Waypoint model
    public void addWaypoint(Vector2 wp)
    {
        //path.addWaypoint(wp) ...
        lr.positionCount++;
        lr.SetPosition(lr.positionCount, wp);
        len++;
    }

    public void complete()
    {
        building = false;
        entityController.submitPath(path);
    }
}
