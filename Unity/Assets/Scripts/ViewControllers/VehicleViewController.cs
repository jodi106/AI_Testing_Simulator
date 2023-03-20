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
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.VEHICLE_DESELECTED);

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

    public abstract void init(VehicleCategory cat, Color color);

    public virtual void onChangePosition(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
        mainController.moveActionButtons(transform.position);
    }

    public virtual void onChangeRotation(float angle)
    {
        transform.eulerAngles = new Vector3(0, 0, angle);
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
        if(getEntity() == null)
        {
            return; //not initialized yet
        }
        if (!placed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!this.shouldIgnoreWaypoints())
            {
                var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (waypoint is not null)
                {
                    difference = Vector2.zero;
                    getEntity().setPosition(waypoint.X, waypoint.Y);
                    gameObject.transform.eulerAngles = new Vector3(0, 0, waypoint.Rot);
                }
                else
                {
                    getEntity().setPosition(mousePosition.x, mousePosition.y); // Im not sure this is ok
                }
            }
            else
            {
                getEntity().setPosition(mousePosition.x, mousePosition.y);
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
                getEntity().setPosition(waypoint.X, waypoint.Y);
                getEntity().setRotation(waypoint.Rot);
            }
            else
            {
                getEntity().setPosition(mousePosition.x, mousePosition.y);
                getEntity().setRotation(0);
            }
        }
        else
        {
            //rotation is fixed by PathController
            getEntity().setPosition(mousePosition.x, mousePosition.y);
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

    public abstract BaseEntity getEntity();
    public abstract void openEditDialog();
    protected abstract void registerEntity();
    public abstract void onChangeColor(Color c);
}