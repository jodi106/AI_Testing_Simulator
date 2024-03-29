using Assets.Enums;
using Entity;
using System;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// An abstract base class for a BaseEntity providing shared functionality for handling positions, rotations, and selections.
/// </summary>
public abstract class VehicleViewController : MonoBehaviour, IBaseEntityController, IBaseEntityView
{
    public Material selectionMaterial;
    private Material defaultMaterial;

    protected SpriteRenderer sprite;
    protected bool placed = false;
    protected Vector2 difference = Vector2.zero;
    protected Vector2 lastClickPos = Vector2.zero;
    protected SnapController snapController;
    protected MainController mainController;
    protected bool ignoreWaypoints;


    public virtual void Awake()
    {
        snapController = Camera.main.GetComponent<SnapController>();
        mainController = Camera.main.GetComponent<MainController>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.sprite = GetSprite();
        sprite.color = new Color(1, 1, 1, 0.5f);
        defaultMaterial = sprite.material;
        ignoreWaypoints = false;
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.VEHICLE_DESELECTED);

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (!placed && !EventSystem.current.IsPointerOverGameObject())
            {
                placed = true;
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
                RegisterEntity();
                mainController.SetSelectedEntity(this);
            }
        });

        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            Destroy();
        });

        EventManager.StartListening(typeof(EntityListEntryClickedAction), x =>
        {
            var action = new EntityListEntryClickedAction(x);
            if (action.Entity == GetEntity())
            {
                mainController.SetSelectedEntity(this);
            }
        });
    }

    /// <summary>
    /// Returns the Sprite associated with the vehicle.
    /// </summary>
    /// <returns>The Sprite associated with the vehicle.</returns>
    public abstract Sprite GetSprite();

    /// <summary>
    /// Gets the location of the vehicle.
    /// </summary>
    /// <returns>A Location object representing the vehicle's location.</returns>
    public Location GetLocation()
    {
        return GetEntity().SpawnPoint;
    }

    /// <summary>
    /// Initializes the vehicle view controller with the specified category and color.
    /// </summary>
    /// <param name="cat">The AdversaryCategory to initialize the vehicle with.</param>
    /// <param name="color">The Color to initialize the vehicle with.</param>
    public abstract void Init(AdversaryCategory cat, Color color);

    /// <summary>
    /// Changes the position of the entity to the given coordinates.
    /// </summary>
    /// <param name="x">The X coordinate of the new position.</param>
    /// <param name="y">The Y coordinate of the new position.</param>
    public virtual void OnChangePosition(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
        mainController.MoveActionButtons(transform.position);
    }

    /// <summary>
    /// Changes the rotation of the entity to the given angle.
    /// </summary>
    /// <param name="angle">The new angle in degrees.</param>
    public virtual void OnChangeRotation(float angle)
    {
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    /// <summary>
    /// Changes the category of the entity to the given category.
    /// </summary>
    /// <param name="cat">The new AdversaryCategory of the entity.</param>
    public virtual void OnChangeCategory(AdversaryCategory cat)
    {
        mainController.RefreshEntityList();
    }

    /// <summary>
    /// Changes the model of the entity to the given model.
    /// </summary>
    /// <param name="model">The new EntityModel of the entity.</param>
    public void OnChangeModel(EntityModel model)
    {
        mainController.RefreshEntityList();
    }

    /// <summary>
    /// Changes the ID of the entity to the given ID.
    /// </summary>
    /// <param name="id">The new ID of the entity.</param>
    public void OnChangeID(string id)
    {
        mainController.RefreshEntityList();
    }

    /// <summary>
    /// Selects the entity and changes its position and material.
    /// </summary>
    public virtual void Select()
    {
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.VEHICLE_SELECTED);
        sprite.material = selectionMaterial;
    }

    /// <summary>
    /// Deselects the entity and changes its position and material.
    /// </summary>
    public virtual void Deselect()
    {
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.VEHICLE_DESELECTED);
        sprite.material = defaultMaterial;
    }

    /// <summary>
    /// Destroys the entity.
    /// </summary>
    public abstract void Destroy();

    /// <summary>
    /// Gets the position of the entity.
    /// </summary>
    /// <returns>The position of the entity.</returns>
    public Vector2 GetPosition()
    {
        return gameObject.transform.position;
    }

    /// <summary>
    /// Updates the entity's position and rotation.
    /// </summary>
    public void Update()
    {
        if (GetEntity() == null)
        {
            return; //not initialized yet
        }
        if (!placed)
        {

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy();
                EventManager.TriggerEvent(new CancelPlacementAction());
            }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!IsIgnoringWaypoints())
            {
                var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (waypoint is not null)
                {
                    difference = Vector2.zero;
                    GetEntity().setPosition(waypoint.X, waypoint.Y);
                    GetEntity().setRotation(waypoint.Rot);
                }
                else
                {
                    GetEntity().setPosition(mousePosition.x, mousePosition.y);
                    GetEntity().setRotation(0);
                }
            }
            else
            {
                GetEntity().setPosition(mousePosition.x, mousePosition.y);
            }

        }
    }

    /// <summary>
    /// Handles the OnMouseDrag event to move the entity.
    /// </summary>
    public void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x == lastClickPos.x && mousePosition.y == lastClickPos.y)
        {
            return;
        }

        if (!IsIgnoringWaypoints())
        {
            var waypoint = snapController.FindWaypoint(mousePosition);
            if (waypoint is not null)
            {
                difference = Vector2.zero;
                GetEntity().setPosition(waypoint.X, waypoint.Y);
                GetEntity().setRotation(waypoint.Rot);
            }
            else
            {
                GetEntity().setPosition(mousePosition.x, mousePosition.y);
                GetEntity().setRotation(0);
            }
        }
        else
        {
            //rotation is fixed by PathController
            GetEntity().setPosition(mousePosition.x, mousePosition.y);
        }
        mainController.SetSelectedEntity(this);
    }

    /// <summary>
    /// Handles the OnMouseDown event to select and place the entity.
    /// </summary>
    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
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
            RegisterEntity();
        }
        mainController.SetSelectedEntity(this);
    }

    /// <summary>
    /// Returns whether the entity should ignore waypoints.
    /// </summary>
    /// <returns>True if the entity should ignore waypoints, false otherwise.</returns>
    public bool IsIgnoringWaypoints()
    {
        return ignoreWaypoints;
    }

    /// <summary>
    /// Sets whether the entity should ignore waypoints.
    /// </summary>
    /// <param name="b">True to ignore waypoints, false otherwise.</param>
    public virtual void ShouldIgnoreWaypoints(bool b)
    {
        ignoreWaypoints = b;
    }

    /// <summary>
    /// Gets the BaseEntity instance of the entity.
    /// </summary>
    /// <returns>The BaseEntity instance of the entity.</returns>
    public abstract BaseEntity GetEntity();
    /// <summary>
    /// Opens the edit dialog for the entity.
    /// </summary>
    public abstract void OpenEditDialog();
    /// <summary>
    /// Registers the entity with the main controller.
    /// </summary>
    protected abstract void RegisterEntity();
    /// <summary>
    /// Changes the color of the entity.
    /// </summary>
    /// <param name="c">The new color of the entity.</param>
    public abstract void OnChangeColor(Color c);
}