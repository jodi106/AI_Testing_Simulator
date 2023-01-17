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
    protected Boolean selected = true;
    protected Boolean expectingAction = false;
    protected Vector2 difference = Vector2.zero;
    protected Vector2 lastClickPos = Vector2.zero;
    protected SnapController snapController;
    protected MainController mainController;

    public void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();
        this.mainController = Camera.main.GetComponent<MainController>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.sprite = getSprite();
        sprite.color = new Color(1, 1, 1, 0.5f);
        defaultMaterial = sprite.material;
    }

    public abstract Sprite getSprite();

    public virtual void onChangePosition(Location location)
    {
        transform.position = HeightUtil.SetZ(location.Vector3, transform.position.z);
        transform.eulerAngles = new Vector3(0, 0, location.Rot);
    }

    public void onChangeType(VehicleCategory cat)
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

    public void select()
    {
        this.selected = true;
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.VEHICLE_SELECTED);
        sprite.material = selectionMaterial;
    }

    public void deselect()
    {
        this.selected = false;
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.VEHICLE_DESELECTED);
        sprite.material = defaultMaterial;
        if (expectingAction)
        {
            EventManager.TriggerEvent(new CancelPathSelectionAction());
        }
    }

    public void destroy()
    {
        Destroy(gameObject);
    }

    public Vector2 getPosition()
    {
        return gameObject.transform.position;
    }
    public void Update()
    {
        if (!placed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (getEntity().GetType() == typeof(Vehicle) || getEntity().GetType() == typeof(Ego))
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
            else if (getEntity().GetType() == typeof(Pedestrian))
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

        if (getEntity().GetType() == typeof(Vehicle) || getEntity().GetType() == typeof(Ego))
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
        else if (getEntity().GetType() == typeof(Pedestrian))
        {
            getEntity().setSpawnPoint(new Location(mousePosition.x, mousePosition.y, 0, 0));
        }

    }

    public void OnMouseDown()
    {
        difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        lastClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!placed)
        {
            placed = true;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
            this.registerEntity();
        }
        if (!selected)
        {
            mainController.setSelectedEntity(this);
        }
    }

    public abstract bool hasAction();
    public abstract void deleteAction();
    public abstract void triggerActionSelection();
    public abstract BaseEntity getEntity();
    public abstract void openEditDialog();
    protected abstract void registerEntity();
    public abstract void onChangeColor(Color c);
}