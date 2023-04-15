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
    private Adversary adversary;
    private PathController pathController;
    private AdversarySettingsPopupController adversarySettingsController;
    private static readonly double INITIAL_SPEED = 30;
    private static readonly double INITIAL_SPEED_PEDESTRIAN = 5;

    public override void Awake()
    {
        base.Awake();
        adversarySettingsController = GameObject.Find("PopUps").transform.Find("CarSettingsPopUp").gameObject.GetComponent<AdversarySettingsPopupController>();
        adversarySettingsController.gameObject.SetActive(true);
    }

    // act as constructor -- check for alternatives to set initial state
    // create vehicle here and register it after is is placed
    /// <summary>
    /// Initializes the Adversary entity with the specified category and color.
    /// </summary>
    /// <param name="cat">The category of the Adversary entity</param>
    /// <param name="color">The color of the Adversary entity</param>
    public override void Init(AdversaryCategory cat, Color color)
    {
        var vehiclePosition = new Location(transform.position.x, transform.position.y, 0, 0);
        var path = new Path();
        if (cat == AdversaryCategory.Pedestrian)
        {
            adversary = new Adversary(vehiclePosition, INITIAL_SPEED_PEDESTRIAN, cat, VehicleModelRepository.getDefaultModel(cat), path, color);
        }
        else
        {
            adversary = new Adversary(vehiclePosition, INITIAL_SPEED, cat, VehicleModelRepository.getDefaultModel(cat), path, color);
        }
        adversary.setView(this);
        OnChangeCategory(adversary.Category);
        OnChangeModel(adversary.Model);
        OnChangeColor(adversary.Color.ToUnityColor());
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
    /// <param name="adversary">The Adversary entity to be initialized</param>
    public void Init(Adversary adversary)
    {
        this.adversary = adversary;
        placed = true;
        this.adversary.setView(this);
        OnChangePosition(this.adversary.SpawnPoint.X, this.adversary.SpawnPoint.Y);
        OnChangeRotation(this.adversary.SpawnPoint.Rot);
        OnChangeCategory(this.adversary.Category);
        OnChangeModel(this.adversary.Model);
        OnChangeColor(this.adversary.Color.ToUnityColor());
        if (adversary.Path.WaypointList.Count > 0)
        {
            ignoreWaypoints = adversary.Path.WaypointList[0].Strategy == WaypointStrategy.SHORTEST ? true : false;
        }
        else
        {
            switch (this.adversary.Category)
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
        pathController = Instantiate(pathPrefab, Vector3.zero, Quaternion.identity).GetComponent<PathController>();
        pathController.Init(this, this.adversary.Color.ToUnityColor(), this.adversary.Path, false);
    }

    /// <summary>
    /// Moves the first waypoint of the PathController of the vehicle to the specified position.
    /// </summary>
    /// <param name="x">The x coordinate of the position</param>
    /// <param name="y">The y coordinate of the position</param>
    public override void OnChangePosition(float x, float y)
    {
        base.OnChangePosition(x, y);
        pathController?.MoveFirstWaypoint(x, y);
    }

    /// <summary>
    /// Gets the sprite for this vehicle.
    /// </summary>
    /// <returns>The sprite for this vehicle.</returns>
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("sprites/" + "vehicle");
    }

    /// <summary>
    /// Called when the category of this adversary changes.
    /// Changes the sprite of this adversary based on the new category and sets the size of the BoxCollider2D accordingly.
    /// </summary>
    /// <param name="cat">The new category of this adversary.</param>
    public override void OnChangeCategory(AdversaryCategory cat)
    {
        base.OnChangeCategory(cat);
        switch (cat)
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
    public BoxCollider2D GetCollider()
    {
        return gameObject.GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Selects this adversary and creates a path controller if none exists.
    /// </summary>
    public override void Select()
    {
        base.Select();
        pathController?.Select();
        snapController.IgnoreClicks = true;
    }

    /// <summary>
    /// Deselects this adversary and deselects its path controller.
    /// </summary>
    public override void Deselect()
    {
        base.Deselect();
        pathController?.Deselect();
        snapController.IgnoreClicks = false;
    }

    /// <summary>
    /// Removes this adversary from the main controller and destroys it.
    /// </summary>
    public override void Destroy()
    {
        mainController.RemoveAdversary(adversary);
        pathController?.Destroy();
        Destroy(gameObject);
        snapController.IgnoreClicks = false;
    }

    /// <summary>
    /// Called when the color of this adversary changes.
    /// Sets the color of the sprite and path controller accordingly and refreshes the entity list of the main controller.
    /// </summary>
    /// <param name="color">The new color of this adversary.</param>
    public override void OnChangeColor(Color color)
    {
        if (placed)
        {
            sprite.color = color;
        }
        else
        {
            sprite.color = new Color(color.r, color.g, color.b, 0.5f);
        }
        pathController?.SetColor(sprite.color);
        mainController.RefreshEntityList();
    }


    /// <summary>
    /// Gets the entity of this adversary.
    /// </summary>
    /// <returns>The entity of this adversary.</returns>
    public override BaseEntity GetEntity()
    {
        return adversary;
    }

    /// <summary>
    /// Opens the edit dialog for this adversary.
    /// </summary>
    public override void OpenEditDialog()
    {
        adversarySettingsController.Open(this, sprite.color, mainController.Info.EgoVehicle);
    }

    /// <summary>
    /// Registers this adversary entity with the main controller.
    /// </summary>
    protected override void RegisterEntity()
    {
        mainController.AddAdversary(adversary);
        EventManager.TriggerEvent(new CompletePlacementAction());
        //PathController must have position 0, otherwise edgecollider is not aligned
        pathController = Instantiate(pathPrefab, Vector3.zero, Quaternion.identity).GetComponent<PathController>();
        pathController.Init(this, adversary.Color.ToUnityColor(), adversary.Path, true);
    }

    /// <summary>
    /// Sets whether to ignore waypoints for this adversary and its path controller.
    /// </summary>
    /// <param name="b">The boolean value to set for ignoring waypoints.</param>
    public override void ShouldIgnoreWaypoints(bool b)
    {
        base.ShouldIgnoreWaypoints(b);
        if (pathController is not null)
        {
            if (!ignoreWaypoints)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var waypoint = snapController.FindWaypoint(mousePosition);
                if (waypoint is not null)
                {
                    difference = Vector2.zero;
                    GetEntity().setPosition(waypoint.X, waypoint.Y);
                    GetEntity().setRotation(waypoint.Rot);
                }
                else
                {
                    GetEntity().setPosition(mousePosition.x, mousePosition.y);
                    GetEntity().setRotation(0);
                }
                pathController.MoveFirstWaypoint(waypoint.X, waypoint.Y);
            }
            WaypointViewController firstWaypoint = pathController.GetFirstWaypointController();
            firstWaypoint.ShouldIgnoreWaypoints(b);
            pathController.UpdateAdjacentPaths(firstWaypoint);
        }
    }
}
