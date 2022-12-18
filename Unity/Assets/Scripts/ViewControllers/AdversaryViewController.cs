using Assets.Enums;
using Entity;
using UnityEngine;

public class AdversaryViewController : VehicleViewController, IBaseEntityWithPathController
{
    public GameObject pathPrefab;
    private Vehicle vehicle;
    private PathController pathController;
    public new void Awake()
    {
        base.Awake();

        var vehiclePosition = new Location(transform.position.x, transform.position.y, 0, 0);
        var path = new Path();
        this.vehicle = new Vehicle(vehiclePosition, path, category: VehicleCategory.Car);
        this.vehicle.setView(this);

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            expectingAction = false;
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

    public override Sprite getSprite()
    {
        return Resources.Load<Sprite>("sprites/" + "vehicle");
    }

    public override void triggerActionSelection()
    {
        var pathGameObject = Instantiate(pathPrefab, new Vector3(0,0,-0.1f), Quaternion.identity);
        this.pathController = pathGameObject.GetComponent<PathController>();
        this.pathController.SetEntityController(this);
        this.pathController.setColor(this.sprite.color);
        expectingAction = true;
    }

    public void submitPath(Path path)
    {
        vehicle.Path = path;
        expectingAction = false;
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
        this.pathController?.destroy();
        Destroy(gameObject);
    }

    public override void deleteAction()
    {
        this.vehicle.Path = null;
        this.pathController?.destroy();
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

    public override BaseEntity getEntity()
    {
        return this.vehicle;
    }
}
