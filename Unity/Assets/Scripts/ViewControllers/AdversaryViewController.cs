using Assets.Enums;
using Assets.Repos;
using Entity;
using UnityEngine;

public class AdversaryViewController : VehicleViewController
{
    public GameObject pathPrefab;
    private Adversary vehicle;
    private PathController pathController;
    private AdversarySettingsPopupController vehicleSettingsController;
    private static readonly double INITIAL_SPEED = 30;

    public PathController getPathController()
    {
        return this.pathController;
    }

    // act as constructor -- check for alternatives to set initial state
    // create vehicle here and register it after is is placed
    public override void init(VehicleCategory cat, Color color)
    {
        var vehiclePosition = new Location(transform.position.x, transform.position.y, 0, 0);
        var path = new Path();
        vehicle = new Adversary(vehiclePosition, INITIAL_SPEED, cat, VehicleModelRepository.getDefaultModel(cat), path);
        vehicle.setView(this);
        vehicleSettingsController = GameObject.Find("PopUps").transform.Find("CarSettingsPopUp").gameObject.GetComponent<AdversarySettingsPopupController>();
        vehicleSettingsController.gameObject.SetActive(true);
        vehicle.setCategory(cat);
        vehicle.setModel(VehicleModelRepository.getDefaultModel(cat));
        vehicle.setColor(color);
        switch (cat)
        {
            case VehicleCategory.Car:
            case VehicleCategory.Motorcycle:
                ignoreWaypoints = false;
                break;
            case VehicleCategory.Bike:
            case VehicleCategory.Pedestrian:
                ignoreWaypoints = true;
                break;
        }
    }

    // vehicle is passed as a parameter and is already registered with the main controller.
    // registerEntity is not called because placed is set to true.
    public void init(Adversary s)
    {
        vehicle = s;
        placed = true;
        vehicle.setView(this);
        onChangePosition(vehicle.SpawnPoint.X, vehicle.SpawnPoint.Y);
        onChangeCategory(vehicle.Category);
        onChangeModel(vehicle.Model);
        if(vehicle.Color is not null) onChangeColor(vehicle.Color.ToUnityColor());
        vehicleSettingsController = GameObject.Find("PopUps").transform.Find("CarSettingsPopUp").gameObject.GetComponent<AdversarySettingsPopupController>();
        vehicleSettingsController.gameObject.SetActive(true);
        switch (vehicle.Category)
        {
            case VehicleCategory.Car:
            case VehicleCategory.Motorcycle:
                ignoreWaypoints = false;
                break;
            case VehicleCategory.Bike:
            case VehicleCategory.Pedestrian:
                ignoreWaypoints = true;
                break;
        }
        if (s.Path is not null)
        {
            this.pathController = Instantiate(pathPrefab, Vector3.zero, Quaternion.identity).GetComponent<PathController>();
            this.pathController.Init(this, this.vehicle, false);
        }
    }

    public override void onChangePosition(float x, float y)
    {
        base.onChangePosition(x, y);
        pathController?.MoveFirstWaypoint(x, y);
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
            //PathController must have position 0, otherwise edgecollider is not aligned
            this.pathController = Instantiate(pathPrefab, Vector3.zero, Quaternion.identity).GetComponent<PathController>();
            this.pathController.Init(this, this.vehicle);
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
        mainController.removeSimulationEntity(vehicle);
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
        this.vehicleSettingsController.open(this, sprite.color, mainController.info.EgoVehicle);
    }
    protected override void registerEntity()
    {
        mainController.addSimulationEntity(this.vehicle);
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
