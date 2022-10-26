using Assets.Enums;
using Entities;
using System;
using UnityEngine;

public class VehicleViewController : MonoBehaviour, IVehicleView
{

    private SpriteRenderer sprite;
    private Boolean placed = false;
    public Vehicle vehicle { get; set; } = new Vehicle();
    Vector2 difference = Vector2.zero;

    public void Awake()
    {
        vehicle.View = this; 
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0.5f);
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
    }

    public void onChangePosition(Coord3D v)
    {
        transform.position = new Vector3(v.X, v.Y, -0.1f) - (Vector3) difference;
    }

    public void onChangeType(VehicleCategory cat)
    {
        throw new NotImplementedException();
    }
}