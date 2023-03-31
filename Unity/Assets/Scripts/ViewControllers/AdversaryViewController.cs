using Assets.Enums;
using Assets.Repos;
using Entity;
using UnityEngine;


/// <summary>
/// Controller for the Adversary entity in the scene.
/// </summary>
public class AdversaryViewController : VehicleViewController
{
    public GameObject pathPrefab;
    private Adversary vehicle;
    private PathController pathController;
    private AdversarySettingsPopupController vehicleSettingsController;
    private static readonly double INITIAL_SPEED = 30;
    private static readonly double INITIAL_SPEED_PEDESTRIAN = 5;


    /// <summary>
    /// Returns the current PathController of the vehicle.
    /// </summary>
    /// <returns>The current PathController of the vehicle</returns>
    public PathController getPathController()
    {
        return this.pathController;
    }

    // act as constructor -- check for alternatives to set initial state
    // create vehicle here and register it after is is placed
    /// <summary>
    /// Initializes the Adversary entity with the specified category and color.
    /// </summary>
    /// <param name="cat">The category of the Adversary entity</param>
    /// <param name="color">The color of the Adversary entity</param>
    public override void init(AdversaryCategory cat, Color color)
    {
        var vehiclePosition = new Location(transform.position.x, transform.position.y, 0, 0);
        var path = new Path();
        if (cat == AdversaryCategory.Pedestrian)
        {
            vehicle = new Adversary(vehiclePosition, INITIAL_SPEED_PEDESTRIAN, cat, VehicleModelRepository.getDefaultModel(cat), path);
        }
        else
        {
            vehicle = new Adversary(vehiclePosition, INITIAL_SPEED, cat, VehicleModelRepository.getDefaultModel(cat), path);
        }
        vehicle.setView(this);
        vehicleSettingsController = GameObject.Find("PopUps").transform.Find("CarSettingsPopUp").gameObject.GetComponent<AdversarySettingsPopupController>();
        vehicleSettingsController.gameObject.SetActive(true);
        vehicle.setCategory(cat);
        vehicle.setModel(VehicleModelRepository.getDefaultModel(cat));
        vehicle.setColor(color);
        switch (cat)
        {
            case AdversaryCategory.Car:
            case AdversaryCategory.Motorcycle:
                ignoreWaypoints = false;
                break;
            case AdversaryCategory.Bike:
            case AdversaryCategory.Pedestrian:
                ignoreWaypoints = true;
                break;
        }
    }

    // vehicle is passed as a parameter and is already registered with the main controller.
    // registerEntity is not called because placed is set to true.
    /// <summary>
    /// Initializes the Adversary entity with the specified entity.
    /// </summary>
    /// <param name="s">The Adversary entity to be initialized</param>
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
            case AdversaryCategory.Car:
            case AdversaryCategory.Motorcycle:
                ignoreWaypoints = false;
                break;
            case AdversaryCategory.Bike:
            case AdversaryCategory.Pedestrian:
                ignoreWaypoints = true;
                break;
        }
        if (s.Path is not null)
        {
            this.pathController = Instantiate(pathPrefab, Vector3.zero, Quaternion.identity).GetComponent<PathController>();
            this.pathController.Init(this, this.vehicle, false);
        }
    }

    /// <summary>
    /// Moves the first waypoint of the PathController of the vehicle to the specified position.
    /// </summary>
    /// <param name="x">The x coordinate of the position</param>
    /// <param name="y">The y coordinate of the position</param>
    public override void onChangePosition(float x, float y)
    {
        base.onChangePosition(x, y);
        pathController?.MoveFirstWaypoint(x, y);
    }

    /// <summary>
    /// Gets the sprite for this vehicle.
    /// </summary>
    /// <returns>The sprite for this vehicle.</returns>
    public override Sprite getSprite()
    {
        return Resources.Load<Sprite>("sprites/" + "vehicle");
    }

    /// <summary>
    /// Called when the category of this adversary changes.
    /// Changes the sprite of this adversary based on the new category and sets the size of the BoxCollider2D accordingly.
    /// </summary>
    /// <param name="cat">The new category of this adversary.</param>
    public override void onChangeCategory(AdversaryCategory cat)
    {
        base.onChangeCategory(cat);
        switch(cat)
        {
            case AdversaryCategory.Car:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "vehicle");
                break;
            case AdversaryCategory.Bike:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "bike");
                break;
            case AdversaryCategory.Pedestrian:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "pedestrian");
                break;
            case AdversaryCategory.Motorcycle:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "motorcycle");
                break;
        }
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(sprite.sprite.bounds.size.x, sprite.sprite.bounds.size.y);
    }

    /// <summary>
    /// Gets the BoxCollider2D component of this adversary.
    /// </summary>
    /// <returns>The BoxCollider2D component of this adversary.</returns>
    public BoxCollider2D getCollider()
    {
        return gameObject.GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Selects this adversary and creates a path controller if none exists.
    /// </summary>
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

    /// <summary>
    /// Deselects this adversary and deselects its path controller.
    /// </summary>
    public override void deselect()
    {
        base.deselect();
        pathController?.deselect();
        snapController.IgnoreClicks = false;
    }

    /// <summary>
    /// Removes this adversary from the main controller and destroys it.
    /// </summary>
    public override void destroy()
    {
        mainController.removeAdversary(vehicle);
        pathController?.Destroy();
        Destroy(gameObject);
        snapController.IgnoreClicks = false;
    }

    /// <summary>
    /// Called when the color of this adversary changes.
    /// Sets the color of the sprite and path controller accordingly and refreshes the entity list of the main controller.
    /// </summary>
    /// <param name="color">The new color of this adversary.</param>
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


    /// <summary>
    /// Gets the entity of this adversary.
    /// </summary>
    /// <returns>The entity of this adversary.</returns>
    public override BaseEntity getEntity()
    {
        return this.vehicle;
    }

    /// <summary>
    /// Opens the edit dialog for this adversary.
    /// </summary>
    public override void openEditDialog()
    {
        this.vehicleSettingsController.open(this, sprite.color, mainController.info.EgoVehicle);
    }

    /// <summary>
    /// Registers this adversary entity with the main controller.
    /// </summary>
    protected override void registerEntity()
    {
        mainController.addAdversary(this.vehicle);
        EventManager.TriggerEvent(new CompletePlacementAction());
    }

    /// <summary>
    /// Sets whether to ignore waypoints for this adversary and its path controller.
    /// </summary>
    /// <param name="b">The boolean value to set for ignoring waypoints.</param>
    public override void setIgnoreWaypoints(bool b)
    {
        base.setIgnoreWaypoints(b);
        if(this.pathController is not null)
        {
            this.pathController.getFirstWaypointController().setIgnoreWaypoints(b);
        }
    }
}
