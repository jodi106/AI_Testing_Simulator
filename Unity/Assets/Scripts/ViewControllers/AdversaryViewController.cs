using Assets.Enums;
using Assets.Repos;
using Entity;
using UnityEngine;

public class AdversaryViewController : VehicleViewController, IBaseEntityWithPathController
{
    public GameObject pathPrefab;
    private Vehicle vehicle;
    private PathController pathController;
    private VehicleSettingsPopupController vehicleSettingsController;
    public new void Awake()
    {
        base.Awake();

        var vehiclePosition = new Location(transform.position.x, transform.position.y, 0, 0);
        var path = new Path();
        this.vehicle = new Vehicle(vehiclePosition, VehicleModelRepository.getDefaultCarModel(), path, category: VehicleCategory.Car);
        this.vehicle.setView(this);
        this.vehicleSettingsController = GameObject.Find("PopUps").transform.Find("CarSettingsPopUp").gameObject.GetComponent<VehicleSettingsPopupController>();
        this.vehicleSettingsController.gameObject.SetActive(true);

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
                this.registerEntity();
            }
        });
    }

    public override void onChangePosition(Location location)
    {
        base.onChangePosition(location);
        this.pathController?.MoveFirstWaypoint(location);
    }

    public override Sprite getSprite()
    {
        return Resources.Load<Sprite>("sprites/" + "vehicle");
    }

    public new void select()
    {
        base.select();
        this.pathController?.Select();
    }

    public new void deselect()
    {
        base.deselect();
        this.pathController?.Deselect();
    }

    public override void triggerActionSelection()
    {
        var pathGameObject = Instantiate(pathPrefab, new Vector3(0,0,-0.1f), Quaternion.identity);
        this.pathController = pathGameObject.GetComponent<PathController>();
        this.pathController.SetEntityController(this);
        this.pathController.SetColor(this.sprite.color);
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
        mainController.removeVehicle(vehicle);
        this.pathController?.Destroy();
        Destroy(gameObject);
    }

    public override void deleteAction()
    {
        this.vehicle.Path = null;
        this.pathController?.Destroy();
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
            this.pathController.SetColor(this.sprite.color);
        }
    }

    public override BaseEntity getEntity()
    {
        return this.vehicle;
    }

    public override void openEditDialog()
    {
        this.vehicleSettingsController.open(this, sprite.color);
    }
    protected override void registerEntity()
    {
        mainController.addVehicle(this.vehicle);
    }
}
