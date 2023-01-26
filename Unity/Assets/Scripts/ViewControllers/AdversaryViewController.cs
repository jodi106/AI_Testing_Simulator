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
    public new void Awake()
    {
        base.Awake();

        var vehiclePosition = new Location(transform.position.x, transform.position.y, 0, 0);
        var path = new Path();
        this.vehicle = new Vehicle(vehiclePosition, VehicleModelRepository.getDefaultCarModel(), path, category: VehicleCategory.Car);
        this.vehicle.setView(this);
        this.vehicleSettingsController = GameObject.Find("PopUps").transform.Find("CarSettingsPopUp").gameObject.GetComponent<AdversarySettingsPopupController>();
        this.vehicleSettingsController.gameObject.SetActive(true);
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

    public override void onChangeType(VehicleCategory cat)
    {
        base.onChangeType(cat);
        switch(cat)
        {
            case VehicleCategory.Car:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "vehicle");
                return;
            case VehicleCategory.Bike:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "bike");
                return;
            case VehicleCategory.Motorcycle:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "motorcycle");
                return;
        }
    }

    public new void select()
    {
        base.select();
        pathController?.adjustHeights(true);
    }

    public new void deselect()
    {
        base.deselect();
        pathController?.adjustHeights(false);
    }

    //public override void triggerActionSelection()
    //{
    //    var pathGameObject = Instantiate(pathPrefab, new Vector3(0,0,-0.1f), Quaternion.identity);
    //    this.pathController = pathGameObject.GetComponent<PathController>();
    //    this.pathController.SetEntityController(this);
    //    this.pathController.SetColor(this.sprite.color);
    //    snapController.ignoreClicks = true;
    //}

    public new void destroy()
    {
        base.destroy();
        mainController.removeVehicle(vehicle);
        snapController.ignoreClicks = false;
        this.pathController?.Destroy();
        Destroy(gameObject);
    }

    //public override void deleteAction()
    //{
    //    this.vehicle.Path = null;
    //    this.pathController?.Destroy();
    //    this.pathController = null;
    //}

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
}
