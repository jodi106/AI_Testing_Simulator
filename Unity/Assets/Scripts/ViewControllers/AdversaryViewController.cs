using Assets.Enums;
using Entity;
using ExportScenario.XMLBuilder;
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

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (!placed)
            {
                placed = true;
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
                mainController.addVehicle(this.vehicle);
            }
        });
    }

    public override void triggerActionSelection()
    {
        var pathGameObject = Instantiate(pathPrefab, gameObject.transform.position, Quaternion.identity);
        this.pathController = pathGameObject.GetComponent<PathController>();
        this.pathController.SetEntityController(this);
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
        var (_, waypoint) = snapController.FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (waypoint is not null)
        {
            difference = Vector2.zero;
            vehicle.SetSpawnPoint(waypoint.Location);
            gameObject.transform.eulerAngles = new Vector3(0, 0, waypoint.Location.Rot);
        }
        else
        {
            vehicle.SetSpawnPoint(new Location(mousePosition.x, mousePosition.y,0,0));
        }
    }

    public void Update()
    {
        if (!placed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var (_, waypoint) = snapController.FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (waypoint is not null)
            {
                difference = Vector2.zero;
                this.vehicle.SetSpawnPoint(waypoint.Location);
                gameObject.transform.eulerAngles = new Vector3(0, 0, waypoint.Location.Rot);
            }
            else
            {
                vehicle.SetSpawnPoint(new Location(mousePosition.x, mousePosition.y, 0, 0));
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
            return this.vehicle.Path.WaypointList.Count != 0;
        }
    }

    public new void destroy()
    {
        base.destroy();
        if(this.pathController is not null)
        {
            Destroy(this.pathController.gameObject);
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
