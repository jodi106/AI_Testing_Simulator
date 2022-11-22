using Entity;
using System.Collections;
using UnityEngine;
public class AdversaryViewController : VehicleViewController, IBaseEntityWithPathController
{
    public GameObject pathPrefab;
    private Vehicle vehicle;
    public new void Awake()
    {
        base.Awake();
        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            expectingPath = false;
        });
    }

    public void triggerPathRequest()
    {
        var pathGameObject = Instantiate(pathPrefab, gameObject.transform.position, Quaternion.identity);
        PathController pathController = pathGameObject.GetComponent<PathController>();
        pathController.setEntityController(this);
        expectingPath = true;
    }

    public void submitPath(Path path)
    {
        vehicle.Path = path;
        expectingPath = false;
    }

    public override BaseEntity getEntity()
    {
        return vehicle;
    }

    public void OnMouseDrag()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x == lastClickPos.x && mousePosition.y == lastClickPos.y)
        {
            return;
        }
        GameObject waypoint = snapController.findNearestWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (waypoint is not null)
        {
            difference = Vector2.zero;
            vehicle.setPosition(waypoint.transform.position.x, waypoint.transform.position.y);
            gameObject.transform.eulerAngles = waypoint.transform.eulerAngles;
        }
        else
        {
            vehicle.setPosition(mousePosition.x, mousePosition.y);
        }
    }

    public void Update()
    {
        if (!placed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject waypoint = snapController.findNearestWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (waypoint is not null)
            {
                difference = Vector2.zero;
                this.vehicle.setPosition(waypoint.transform.position.x, waypoint.transform.position.y);
                gameObject.transform.eulerAngles = waypoint.transform.eulerAngles;
            }
            else
            {
                this.vehicle.setPosition(mousePosition.x, mousePosition.y);
            }
        }
    }

    public void OnMouseDown()
    {
        difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        lastClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!placed)
        {
            placed = true;
            sprite.color = new Color(1, 1, 1, 1);
        }
        if (!selected)
        {
            EventManager.TriggerEvent(new ChangeSelectedEntityAction(this));
        }
    }

    public void setVehicle(Vehicle vehicle)
    {
        this.vehicle = vehicle;
    }
}
