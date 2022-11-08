using Models;
using UnityEngine;

public class PathController : MonoBehaviour
{
    private LineRenderer lr;
    public Path path { get; set; }
    private bool building;
    int len = 1;
    private IBaseEntityController entityController;

    private void Awake()
    {
        lr = gameObject.GetComponent<LineRenderer>();
        building = true;

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            if(building)
            {
                Destroy(gameObject);
            }
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
            lr.SetPosition(len, Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
