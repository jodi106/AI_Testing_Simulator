using Assets.Enums;
using Entity;
using System;
using UnityEngine;

public abstract class VehicleViewController : MonoBehaviour, IVehicleView, IBaseEntityController
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

    public void onChangePosition(Location l)
    {
        transform.position = new Vector3(l.Vector3.x, l.Vector3.y, transform.position.z) - (Vector3)difference;
    }

    public void onChangeType(VehicleCategory cat)
    {
        throw new NotImplementedException();
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
        if (expectingAction)
        {
            EventManager.TriggerEvent(new CancelPathSelectionAction());
        }
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
            var (_, waypoint) = snapController.FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (waypoint is not null)
            {
                difference = Vector2.zero;
                getEntity().setSpawnPoint(waypoint.Location);
                gameObject.transform.eulerAngles = new Vector3(0, 0, waypoint.Location.Rot);
            }
            else
            {
                getEntity().setSpawnPoint(new Location(mousePosition.x, mousePosition.y, 0, 0)); // Im not sure this is ok
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
        var (_, waypoint) = snapController.FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (waypoint is not null)
        {
            difference = Vector2.zero;
            getEntity().setSpawnPoint(waypoint.Location);
            gameObject.transform.eulerAngles = new Vector3(0, 0, waypoint.Location.Rot);
        }
        else
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
            this.registerVehicle();
        }
        if (!selected)
        {
            mainController.setSelectedEntity(this);
        }
    }

    public abstract bool hasAction();
    public abstract void deleteAction();
    public abstract void triggerActionSelection();
    public abstract void setColor(Color color);
    public abstract BaseEntity getEntity();
    public abstract void openEditDialog();
    protected abstract void registerVehicle();
}