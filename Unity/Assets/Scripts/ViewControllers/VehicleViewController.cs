using Assets.Enums;
using Entity;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class VehicleViewController : MonoBehaviour, IVehicleView, IBaseEntityController
{
    public Material selectionMaterial;
    public GameObject pathPrefab;

    private Material defaultMaterial;
    private SpriteRenderer sprite;
    private Boolean placed = false;
    private Boolean selected = true;
    private Boolean expectingPath = false;
    public Vehicle vehicle { get; set; }
    private Vector2 difference = Vector2.zero;
    private Vector2 lastClickPos = Vector2.zero;

    private SnapController snapController;
    public void Awake()
    {
        this.snapController = Camera.main.GetComponent<SnapController>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0.5f);
        defaultMaterial = sprite.material;

        EventManager.StartListening(typeof(CancelPathSelectionAction), x => {
            expectingPath = false;
        });
    }

    public void OnMouseDrag()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x == lastClickPos.x && mousePosition.y == lastClickPos.y)
        {
            return;
        }
        GameObject waypoint = snapController.findNearestWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (waypoint is not null)
        {
            difference = Vector2.zero;
            vehicle.setPosition(waypoint.transform.position.x, waypoint.transform.position.y);
            gameObject.transform.eulerAngles = waypoint.transform.eulerAngles;
        }
        else
        {
            vehicle.setPosition(mousePosition.x, mousePosition.y);
        }
    }

    public void Update()
    {
        if (!placed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject waypoint = snapController.findNearestWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (waypoint is not null)
            {
                difference = Vector2.zero;
                vehicle.setPosition(waypoint.transform.position.x, waypoint.transform.position.y);
                gameObject.transform.eulerAngles = waypoint.transform.eulerAngles;
            }
            else
            {
                vehicle.setPosition(mousePosition.x, mousePosition.y);
            }
        }
    }

    public void OnMouseDown()
    {
        difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        lastClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

    public BaseEntity getEntity()
    {
        return vehicle;
    }

    public void destroy()
    {
        if (expectingPath)
        {
            EventManager.TriggerEvent(new CancelPathSelectionAction());
        }
        Destroy(gameObject);
    }

    public void triggerPathRequest()
    {
        var pathGameObject = Instantiate(pathPrefab, gameObject.transform.position, Quaternion.identity);
        PathController pathController = pathGameObject.GetComponent<PathController>();
        pathController.setEntityController(this);
        expectingPath = true;
    }

    public void submitPath(Path path)
    {
        vehicle.Path = path;
        expectingPath = false;
    }

    public Vector2 getPosition()
    {
        return gameObject.transform.position;
    }
}