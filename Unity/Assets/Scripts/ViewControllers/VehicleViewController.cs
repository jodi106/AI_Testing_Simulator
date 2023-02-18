using Assets.Enums;
using Entity;
using System;
using UnityEngine;

public abstract class VehicleViewController : MonoBehaviour, IBaseEntityController, IBaseEntityView
{
    public Material selectionMaterial;
    private Material defaultMaterial;

    protected SpriteRenderer sprite;
    protected Boolean placed = false;
    protected Vector2 difference = Vector2.zero;
    protected Vector2 lastClickPos = Vector2.zero;
    protected SnapController snapController;
    protected MainController mainController;
    protected bool ignoreWaypoints;

    public void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();
        this.mainController = Camera.main.GetComponent<MainController>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.sprite = getSprite();
        sprite.color = new Color(1, 1, 1, 0.5f);
        defaultMaterial = sprite.material;
        ignoreWaypoints = false;

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (!placed)
            {
                placed = true;
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
                this.registerEntity();
                mainController.setSelectedEntity(this);
            }
        });

        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            var action = new MapChangeAction(x);
            if(action.name == "")
            {
                this.destroy();
            }
        });
    }

    public abstract Sprite getSprite();

    public Location getLocation()
    {
        return this.getEntity().SpawnPoint;
    }

    public virtual void onChangePosition(Location location)
    {
        transform.position = HeightUtil.SetZ(location.Vector3, transform.position.z);
        transform.eulerAngles = new Vector3(0, 0, location.Rot);
        mainController.moveActionButtons(transform.position);
    }

    public virtual void onChangeCategory(VehicleCategory cat)
    {
        mainController.refreshEntityList();
    }

    public void onChangeModel(EntityModel model)
    {
        mainController.refreshEntityList();
    }

    public void onChangeID(string id)
    {
        mainController.refreshEntityList();
    }

    public virtual void select()
    {
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.VEHICLE_SELECTED);
        sprite.material = selectionMaterial;
    }

    public virtual void deselect()
    {
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.VEHICLE_DESELECTED);
        sprite.material = defaultMaterial;
    }

    public abstract void destroy();

    public Vector2 getPosition()
    {
        return gameObject.transform.position;
    }
    public void Update()
    {
        if (!placed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!this.shouldIgnoreWaypoints())
            {
                var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (waypoint is not null)
                {
                    difference = Vector2.zero;
                    getEntity().setSpawnPoint(waypoint);
                    gameObject.transform.eulerAngles = new Vector3(0, 0, waypoint.Rot);
                }
                else
                {
                    getEntity().setSpawnPoint(new Location(mousePosition.x, mousePosition.y, 0, 0)); // Im not sure this is ok
                }
            }
            else
            {
                getEntity().setSpawnPoint(new Location(mousePosition.x, mousePosition.y, 0, 0));
            }

        }
    }
    public void OnMouseDrag()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x == lastClickPos.x && mousePosition.y == lastClickPos.y)
        {
            return;
        }

        if (!this.shouldIgnoreWaypoints())
        {
            var waypoint = snapController.FindWaypoint(mousePosition);
            if (waypoint is not null)
            {
                difference = Vector2.zero;
                getEntity().setSpawnPoint(waypoint);
            }
            else
            {
                getEntity().setSpawnPoint(new Location(mousePosition.x, mousePosition.y, 0, 0));
            }
        }
        else
        {
            getEntity().setSpawnPoint(new Location(mousePosition.x, mousePosition.y, 0, 0));
        }
        mainController.setSelectedEntity(this);
    }
    public void OnMouseDown()
    {
        if (snapController.IgnoreClicks)
        {
            EventManager.TriggerEvent(new MouseClickAction(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            return;
        }
        difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        lastClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!placed)
        {
            placed = true;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
            this.registerEntity();
        }
        mainController.setSelectedEntity(this);
    }
    public bool shouldIgnoreWaypoints()
    {
        return this.ignoreWaypoints;
    }

    public virtual void setIgnoreWaypoints(bool b)
    {
        this.ignoreWaypoints = b;
    }

    public void alignVehicle(Vector3 direction)
    {
        Vector3 vectorToTarget = direction - transform.position;
        vectorToTarget = HeightUtil.SetZ(vectorToTarget, 0);
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Vector3 rot = new Vector3(0, 0, angle);
        transform.eulerAngles = rot;
    }

    public void resetVehicleAlignment()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public abstract BaseEntity getEntity();
    public abstract void openEditDialog();
    protected abstract void registerEntity();
    public abstract void onChangeColor(Color c);
}