using Assets.Enums;
using Dtos;
using Models;
using System;
using UnityEngine;

public class VehicleViewController : MonoBehaviour, IVehicleView, IBaseEntityController
{
    public Material selectionMaterial;
    public Material defaultMaterial;

    private SpriteRenderer sprite;
    private Boolean placed = false;
    private Boolean selected = true;
    public VehicleModel vehicle { get; set; } = new VehicleModel();
    Vector2 difference = Vector2.zero;

    public void Awake()
    {
        vehicle.View = this;
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0.5f);
        defaultMaterial = sprite.material;
    }

    public void OnMouseDrag()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vehicle.setPosition(mousePosition.x, mousePosition.y);
    }

    public void Update()
    {
        if (!placed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vehicle.setPosition(mousePosition.x, mousePosition.y);
        }
    }

    public void OnMouseDown()
    {
        difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (!placed)
        {
            placed = true;
            sprite.color = new Color(1, 1, 1, 1);
        }
        if (!selected)
        {
            EventManager.TriggerEvent(new ChangeSelectedEntityAction(this));
        }
    }

    public void onChangePosition(Coord3D v)
    {
        transform.position = new Vector3((float)v.X, (float)v.Y, transform.position.z) - (Vector3)difference;
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
    }

    public BaseModel getEntity()
    {
        return vehicle;
    }

    public void destroy()
    {
        Destroy(gameObject);
    }
}