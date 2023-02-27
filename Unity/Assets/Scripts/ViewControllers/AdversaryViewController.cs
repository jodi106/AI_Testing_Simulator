using Assets.Enums;
using Assets.Repos;
using Entity;
using UnityEngine;

public class AdversaryViewController : VehicleViewController
{
    public GameObject pathPrefab;
    private Vehicle vehicle;
    private PathController pathController;
    private AdversarySettingsPopupController vehicleSettingsController;
    private static readonly double INITIAL_SPEED = 30;

    public PathController getPathController()
    {
        return this.pathController;
    }

    public new void Awake()
    {
        base.Awake();
        var vehiclePosition = new Location(transform.position.x, transform.position.y, 0, 0);
        var path = new Path();
        this.vehicle = new Vehicle(vehiclePosition, VehicleModelRepository.getDefaultCarModel(), path, category: VehicleCategory.Car, INITIAL_SPEED);
        this.vehicle.setView(this);
        this.vehicleSettingsController = GameObject.Find("PopUps").transform.Find("CarSettingsPopUp").gameObject.GetComponent<AdversarySettingsPopupController>();
        this.vehicleSettingsController.gameObject.SetActive(true);
    }

    // act as constructor -- check for alternatives to set initial state
    public override void init(VehicleCategory cat)
    {
        vehicle.setCategory(cat);
        vehicle.setModel(VehicleModelRepository.getDefaultModel(cat));
        switch (cat)
        {
            case VehicleCategory.Car:
            case VehicleCategory.Motorcycle:
                this.ignoreWaypoints = false;
                return;
            case VehicleCategory.Bike:
            case VehicleCategory.Pedestrian:
                this.ignoreWaypoints = true;
                return;
        }
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

    public override void onChangeCategory(VehicleCategory cat)
    {
        base.onChangeCategory(cat);
        switch(cat)
        {
            case VehicleCategory.Car:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "vehicle");
                return;
            case VehicleCategory.Bike:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "bike");
                return;
            case VehicleCategory.Pedestrian:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "pedestrian");
                return;
            case VehicleCategory.Motorcycle:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "motorcycle");
                return;
        }
    }

    public BoxCollider2D getCollider()
    {
        return gameObject.GetComponent<BoxCollider2D>();
    }

    public override void select()
    {
        base.select();
        if(this.pathController is null)
        {
            var pathGameObject = Instantiate(pathPrefab, new Vector3(0,0,-0.1f), Quaternion.identity);
            this.pathController = pathGameObject.GetComponent<PathController>();
            this.pathController.Path = this.vehicle.Path;
            this.pathController.SetEntityController(this);
            this.pathController.SetColor(this.sprite.color);
        }
        pathController?.select();
        snapController.IgnoreClicks = true;
    }

    public override void deselect()
    {
        base.deselect();
        pathController?.deselect();
        snapController.IgnoreClicks = false;
    }

    public override void destroy()
    {
        mainController.removeVehicle(vehicle);
        pathController?.Destroy();
        Destroy(gameObject);
        snapController.IgnoreClicks = false;
    }

    public override void onChangeColor(Color color)
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
        mainController.refreshEntityList();
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

    public override void setIgnoreWaypoints(bool b)
    {
        base.setIgnoreWaypoints(b);
        if(this.pathController is not null)
        {
            this.pathController.getFirstWaypointController().setIgnoreWaypoints(b);
        }
    }
}
