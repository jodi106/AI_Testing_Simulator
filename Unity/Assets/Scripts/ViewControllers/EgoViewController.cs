using Assets.Enums;
using Assets.Repos;
using Entity;
using System;
using UnityEngine;

/// <summary>
/// Controller for the Adversary entity in the scene.
/// </summary>
public class EgoViewController : VehicleViewController
{
    public GameObject DestinationPrefab;
    private Ego ego;
    private DestinationController destination;
    private EgoSettingsPopupController egoSettingsController;
    private static readonly double INITIAL_SPEED = 10;


    /// <summary>
    /// Calls the Awake Method of the superclass.
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        egoSettingsController = GameObject.Find("PopUps").transform.Find("EgoSettingsPopUp").gameObject.GetComponent<EgoSettingsPopupController>();
        egoSettingsController.gameObject.SetActive(true);
    }


    /// <summary>
    /// Gets the corresponding Sprite for the Ego Vehicle
    /// </summary>
    /// <returns>Sprite of the Ego</returns>
    public override Sprite getSprite()
    {
        return Resources.Load<Sprite>("sprites/" + "ego");
    }


    /// <summary>
    /// Selects the current object and creates a destination if not already present.
    /// </summary>
    public override void select()
    {
        base.select();
        this.destination.select();
        snapController.IgnoreClicks = true;
    }


    /// <summary>
    /// Deselects the current object and its destination.
    /// </summary>
    public override void deselect()
    {
        base.deselect();
        destination?.deselect();
        snapController.IgnoreClicks = false;
    }

    /// <summary>
    /// Submits the selected destination location.
    /// </summary>
    /// <param name="destination">The destination location to submit.</param>
    public void submitDestination(Location destination)
    {
        ego.Destination = destination;
        EventManager.TriggerEvent(new CompletePlacementAction());
    }

    /// <summary>
    /// Destroys the current object and its associated destination.
    /// </summary>
    public override void destroy()
    {
        this.mainController.setEgo(null);
        destination?.Destroy();
        Destroy(gameObject);
    }


    /// <summary>
    /// Changes the color of the current object and its associated destination.
    /// </summary>
    /// <param name="color">The color to change to.</param>
    public override void onChangeColor(Color color)
    {
        if (placed)
        {
            this.sprite.color = color;
        }
        else
        {
            this.sprite.color = new Color(color.r, color.g, color.b, 0.5f);
        }
        this.destination?.setColor(color);
        mainController.refreshEntityList();
    }

    /// <summary>
    /// Initializes the Ego object with the given AdversaryCategory and color.
    /// </summary>
    /// <param name="cat">The AdversaryCategory to assign to the Ego object.</param>
    /// <param name="color">The color to assign to the Ego object.</param>
    public override void init(AdversaryCategory cat, Color color)
    {
        egoSettingsController = GameObject.Find("PopUps").transform.Find("EgoSettingsPopUp").gameObject.GetComponent<EgoSettingsPopupController>();
        egoSettingsController.gameObject.SetActive(true);
        var egoPosition = new Location(transform.position.x, transform.position.y, 0, 0);
        ego = new Ego(egoPosition, VehicleModelRepository.getDefaultCarModel(), AdversaryCategory.Car, INITIAL_SPEED, color); // TODO initial speed: different default later?
        ego.setView(this);
        ego.setCategory(cat);
        switch (cat)
        {
            case AdversaryCategory.Car:
            case AdversaryCategory.Motorcycle:
                ignoreWaypoints = false;
                return;
            case AdversaryCategory.Bike:
            case AdversaryCategory.Pedestrian:
                ignoreWaypoints = true;
                return;
        }
    }

    /// <summary>
    /// Initializes the Ego object with the given Ego instance.
    /// </summary>
    /// <param name="ego">The Ego instance to initialize the object with.</param>
    public void init(Ego ego)
    {
        this.ego = ego;
        placed = true;
        ego.setView(this);
        onChangePosition(ego.SpawnPoint.X, ego.SpawnPoint.Y);
        onChangeRotation(ego.SpawnPoint.Rot);
        onChangeCategory(ego.Category);
        onChangeModel(ego.Model);
        onChangeColor(ego.Color.ToUnityColor());
        switch (ego.Category)
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
        this.destination = Instantiate(DestinationPrefab, ego.Destination.Vector3Ser.ToVector3(), Quaternion.identity).GetComponent<DestinationController>();
        this.destination.init(this, sprite.color, true);
    }

    /// <summary>
    /// Changes the category of the current object and updates its sprite accordingly.
    /// </summary>
    /// <param name="cat">The new AdversaryCategory to assign to the current object.</param>
    public override void onChangeCategory(AdversaryCategory cat)
    {
        base.onChangeCategory(cat);
        switch (cat)
        {
            case AdversaryCategory.Car:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "vehicle");
                return;
            case AdversaryCategory.Bike:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "bike");
                return;
            case AdversaryCategory.Pedestrian:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "pedestrian");
                return;
            case AdversaryCategory.Motorcycle:
                sprite.sprite = Resources.Load<Sprite>("sprites/" + "motorcycle");
                return;
        }
    }

    /// <summary>
    /// Returns the BaseEntity representing the current Ego object.
    /// </summary>
    /// <returns>The BaseEntity instance of the current object.</returns>
    public override BaseEntity getEntity()
    {
        return this.ego;
    }

    /// <summary>
    /// Opens the edit dialog for the current object.
    /// </summary>
    public override void openEditDialog()
    {
        this.egoSettingsController.open(this, sprite.color);
    }

    /// <summary>
    /// Registers the current object as the Ego in the main controller.
    /// </summary>
    protected override void registerEntity()
    {
        mainController.setEgo(this.ego);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var destinationGameObject = Instantiate(DestinationPrefab, new Vector3(mousePosition.x, mousePosition.y, -0.1f), Quaternion.identity);
        this.destination = destinationGameObject.GetComponent<DestinationController>();
        this.destination.init(this, this.sprite.color);
    }
}
