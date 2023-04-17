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
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("sprites/" + "ego");
    }


    /// <summary>
    /// Selects the current object and the Destination.
    /// Signals the SnapController to ignore clicks.
    /// </summary>
    public override void Select()
    {
        base.Select();
        this.destination.Select();
        snapController.IgnoreClicks = true;
    }


    /// <summary>
    /// Deselects the current object and the Destination.
    /// Signals the SnapController not to ignore clicks.
    /// </summary>
    public override void Deselect()
    {
        base.Deselect();
        destination?.Deselect();
        snapController.IgnoreClicks = false;
    }

    /// <summary>
    /// Set the Destination of the Ego vehicle.
    /// </summary>
    /// <param name="destination">The destination location to submit.</param>
    public void SubmitDestination(Location destination)
    {
        ego.Destination = destination;
    }

    /// <summary>
    /// Destroys the Ego GameObject and the Destination.
    /// </summary>
    public override void Destroy()
    {
        this.mainController.SetEgo(null);
        destination?.Destroy();
        Destroy(gameObject);
    }


    /// <summary>
    /// Changes the color of the current the Ego GameObject and its associated destination.
    /// </summary>
    /// <param name="color">The color to change to.</param>
    public override void OnChangeColor(Color color)
    {
        if (placed)
        {
            this.sprite.color = color;
        }
        else
        {
            this.sprite.color = new Color(color.r, color.g, color.b, 0.5f);
        }
        this.destination?.SetColor(color);
        mainController.RefreshEntityList();
    }

    /// <summary>
    /// Initializes the controller with the given AdversaryCategory and color.
    /// </summary>
    /// <param name="cat">The AdversaryCategory to assign to the Ego object.</param>
    /// <param name="color">The color to assign to the Ego object.</param>
    public override void Init(AdversaryCategory cat, Color color)
    {
        egoSettingsController = GameObject.Find("PopUps").transform.Find("EgoSettingsPopUp").gameObject.GetComponent<EgoSettingsPopupController>();
        egoSettingsController.gameObject.SetActive(true);
        var egoPosition = new Location(transform.position.x, transform.position.y, 0, 0);
        ego = new Ego(egoPosition, VehicleModelRepository.getDefaultCarModel(), AdversaryCategory.Car, INITIAL_SPEED, color);
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
    /// Initializes the controller with the given Ego instance.
    /// </summary>
    /// <param name="ego">The Ego instance to initialize the object with.</param>
    public void Init(Ego ego)
    {
        this.ego = ego;
        placed = true;
        ego.setView(this);
        OnChangePosition(ego.SpawnPoint.X, ego.SpawnPoint.Y);
        OnChangeRotation(ego.SpawnPoint.Rot);
        OnChangeCategory(ego.Category);
        OnChangeModel(ego.Model);
        OnChangeColor(ego.Color.ToUnityColor());
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
        this.destination.Init(this, sprite.color, true);
    }

    /// <summary>
    /// Changes the sprite according to the new category.
    /// </summary>
    /// <param name="cat">The new category of the Ego vehicle.</param>
    public override void OnChangeCategory(AdversaryCategory cat)
    {
        base.OnChangeCategory(cat);
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
    /// Returns the BaseEntity representing the current Ego vehicle.
    /// </summary>
    /// <returns>The BaseEntity instance of the current object.</returns>
    public override BaseEntity GetEntity()
    {
        return this.ego;
    }

    /// <summary>
    /// Opens the edit dialog for the current object.
    /// </summary>
    public override void OpenEditDialog()
    {
        this.egoSettingsController.Open(this, sprite.color);
    }

    /// <summary>
    /// Registers the current object as the Ego in the main controller.
    /// </summary>
    protected override void RegisterEntity()
    {
        mainController.SetEgo(this.ego);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var destinationGameObject = Instantiate(DestinationPrefab, new Vector3(mousePosition.x, mousePosition.y, -0.1f), Quaternion.identity);
        this.destination = destinationGameObject.GetComponent<DestinationController>();
        this.destination.Init(this, this.sprite.color);
        EventManager.TriggerEvent(new CompletePlacementAction());
    }
}
