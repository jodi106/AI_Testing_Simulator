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
    protected Boolean expectingPath = false;
    protected Vector2 difference = Vector2.zero;
    protected Vector2 lastClickPos = Vector2.zero;
    protected SnapController snapController;

    public void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0.5f);
        defaultMaterial = sprite.material;
    }

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
        sprite.transform.Translate(0, 0, -0.1f);
        sprite.material = selectionMaterial;
    }

    public void deselect()
    {
        this.selected = false;
        sprite.transform.Translate(0, 0, 0.1f);
        sprite.material = defaultMaterial;
        if (expectingPath)
        {
            EventManager.TriggerEvent(new CancelPathSelectionAction());
        }
    }

    public void destroy()
    {
        if (expectingPath)
        {
            EventManager.TriggerEvent(new CancelPathSelectionAction());
        }
        Destroy(gameObject);
    }

    public Vector2 getPosition()
    {
        return gameObject.transform.position;
    }

    public abstract BaseEntity getEntity();

    public abstract bool hasAction();

    public abstract void deleteAction();

}