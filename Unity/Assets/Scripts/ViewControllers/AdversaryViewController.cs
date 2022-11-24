using Assets.Enums;
using Entity;
using System.Collections;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AdversaryViewController : VehicleViewController, IBaseEntityWithPathController
{
    public GameObject pathPrefab;
    private Vehicle vehicle;
    private PathController pathController;
    private MainController mainController;
    public new void Awake()
    {
        base.Awake();

        this.mainController = Camera.main.GetComponent<MainController>();

        var vehiclePosition = new Location(transform.position.x, transform.position.y, 0, 0);
        var path = new Path();
        this.vehicle = new Vehicle(vehiclePosition, path, category: VehicleCategory.Car);
        this.vehicle.View = this;

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            expectingPath = false;
        });
    }

    public override void triggerActionSelection()
    {
        var pathGameObject = Instantiate(pathPrefab, gameObject.transform.position, Quaternion.identity);
        this.pathController = pathGameObject.GetComponent<PathController>();
        this.pathController.setEntityController(this);
        this.pathController.setColor(this.sprite.color);
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
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
            mainController.addVehicle(this.vehicle);
        }
        if (!selected)
        {
            EventManager.TriggerEvent(new ChangeSelectedEntityAction(this));
        }
    }

    public override bool hasAction()
    {
        if(this.vehicle.Path is null)
        {
            return false;
        } else
        {
            return this.vehicle.Path.EventList.Count != 0;
        }
    }

    public override void deleteAction()
    {
        this.vehicle.Path = null;
        Destroy(this.pathController.gameObject);
        this.pathController = null;
    }

    public override void setColor(Color color)
    {
        if (placed)
        {
            this.sprite.color = color;
        } else
        {
            this.sprite.color = new Color(color.r, color.g, color.b, 0.5f);
        }
        if(this.pathController is not null)
        {
            this.pathController.setColor(this.sprite.color);
        }
    }
}
