using Assets.Enums;
using Entity;
using UnityEngine;

public class PedestrianViewController : VehicleViewController, IBaseEntityWithPathController
{
    public GameObject pathPrefab;
    private Pedestrian pedestrian;
    private PathController pathController;
    private AdversarySettingsPopupController vehicleSettingsController;
    
    public new void Awake()
    {
        base.Awake();

        var pedestrianPosition = new Location(transform.position.x, transform.position.y, 0, 0);
        var path = new Path();
        this.pedestrian = new Pedestrian(pedestrianPosition, path, PedestrianType.Man);
        this.pedestrian.setView(this);
        //this.vehicleSettingsController = GameObject.Find("PopUps").transform.Find("CarSettingsPopUp").gameObject.GetComponent<VehicleSettingsPopupController>();
        //this.vehicleSettingsController.gameObject.SetActive(true);
        this.ignoreWaypoints = true;

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

    public override void onChangePosition(Location l)
    {
        base.onChangePosition(l);
        this.pathController?.MoveFirstWaypoint(l);
    }

    public override Sprite getSprite()
    {
        return Resources.Load<Sprite>("sprites/" + "pedestrian");
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
        var pathGameObject = Instantiate(pathPrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
        this.pathController = pathGameObject.GetComponent<PathController>();
        this.pathController.SetEntityController(this);
        this.pathController.SetColor(this.sprite.color);
    }

    public void submitPath(Path path)
    {
        pedestrian.Path = path;
    }

    public override bool hasAction()
    {
        if (this.pedestrian.Path is null)
        {
            return false;
        }
        else
        {
            return this.pedestrian.Path.WaypointList.Count != 0;
        }
    }

    public new void destroy()
    {
        base.destroy();
        mainController.removePedestrian(pedestrian);
        this.pathController?.Destroy();
        Destroy(gameObject);
    }

    public override void deleteAction()
    {
        this.pedestrian.Path = null;
        this.pathController?.Destroy();
        this.pathController = null;
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
        if (this.pathController is not null)
        {
            this.pathController.SetColor(this.sprite.color);
        }
        mainController.refreshEntityList();
    }

    public override BaseEntity getEntity()
    {
        return this.pedestrian;
    }

    public override void openEditDialog()
    {
        //this.vehicleSettingsController.open(this.vehicle);
    }
    protected override void registerEntity()
    {
        mainController.addPedestrian(this.pedestrian);
    }
}
