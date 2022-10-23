using System;
using UnityEngine;

public class Car : MonoBehaviour
{

    private SpriteRenderer renderer;
    private Boolean placed = false;

    public void Start()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnMouseDrag()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        transform.Translate(mousePosition);
    }

    public void Update()
    {
        if (!placed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);
        }
    }

    public void OnMouseDown()
    {
        if (!placed)
        {
            placed = true;
            renderer.color = new Color(1, 1, 1, 1);
            EventManager.TriggerEvent(new VehicleMovedAction((Vector2)Input.mousePosition, this));
        }
    }

}