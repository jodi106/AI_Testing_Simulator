using Assets.Enums;
using Assets.Repos;
using Entity;
using System;
using UnityEngine;


public class EgoViewController : VehicleViewController
{
    public GameObject DestinationPrefab;
    private Ego ego;
    private DestinationController destination;
    private EgoSettingsPopupController egoSettingsController;
    private static readonly double INITIAL_SPEED = 10;
    public new void Awake()
    {
        base.Awake();
    }

    public override Sprite getSprite()
    {
        return Resources.Load<Sprite>("sprites/" + "ego");
    }

    public override void select()
    {
        base.select();
        if(this.destination is null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var destinationGameObject = Instantiate(DestinationPrefab, new Vector3(mousePosition.x, mousePosition.y, -0.1f), Quaternion.identity);
            this.destination = destinationGameObject.GetComponent<DestinationController>();
            this.destination.init(this, this.sprite.color);
        }
        this.destination?.select();
        snapController.IgnoreClicks = true;
    }

    public override void deselect()
    {
        base.deselect();
        destination?.deselect();
        snapController.IgnoreClicks = false;
    }

    public void submitDestination(Location destination)
    {
        ego.Destination = destination;
        EventManager.TriggerEvent(new CompletePlacementAction());
    }

    public override void destroy()
    {
        this.mainController.setEgo(null);
        destination?.Destroy();
        Destroy(gameObject);
    }

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

    public override void init(AdversaryCategory cat, Color color)
    {
        egoSettingsController = GameObject.Find("PopUps").transform.Find("EgoSettingsPopUp").gameObject.GetComponent<EgoSettingsPopupController>();
        egoSettingsController.gameObject.SetActive(true);
        var egoPosition = new Location(transform.position.x, transform.position.y, 0, 0);
        ego = new Ego(egoPosition, VehicleModelRepository.getDefaultCarModel(), AdversaryCategory.Car, INITIAL_SPEED); // TODO initial speed: different default later?
        ego.setView(this);
        ego.setCategory(cat);
        ego.setColor(color);
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

    public void init(Ego ego)
    {
        this.ego = ego;
        placed = true;
        ego.setView(this);
        onChangePosition(ego.SpawnPoint.X, ego.SpawnPoint.Y);
        onChangeCategory(ego.Category);
        onChangeModel(ego.Model);
        onChangeColor(ego.Color.ToUnityColor());
        egoSettingsController = GameObject.Find("PopUps").transform.Find("EgoSettingsPopUp").gameObject.GetComponent<EgoSettingsPopupController>();
        egoSettingsController.gameObject.SetActive(true);
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
        if (ego.Destination is not null)
        {
            this.destination = Instantiate(DestinationPrefab, ego.Destination.Vector3Ser.ToVector3(), Quaternion.identity).GetComponent<DestinationController>();
            this.destination.init(this, sprite.color, true);
        }
    }

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

    public override BaseEntity getEntity()
    {
        return this.ego;
    }

    public override void openEditDialog()
    {
        this.egoSettingsController.open(this, sprite.color);
    }

    protected override void registerEntity()
    {
        mainController.setEgo(this.ego);
    }
}
