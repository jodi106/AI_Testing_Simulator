using Assets.Enums;
using Entity;
using System;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// An abstract base class for vehicle view controllers, providing shared functionality for handling vehicle positions, rotations, and selections.
/// </summary>
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


    /// <summary>
    /// Initializes the vehicle view controller, setting up necessary components, materials, and event listeners.
    /// </summary>
    public virtual void Awake()
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
            if (!placed && !EventSystem.current.IsPointerOverGameObject())
            {
                placed = true;
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
                this.registerEntity();
                mainController.setSelectedEntity(this);
            }
        });

        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            this.destroy();
        });

        EventManager.StartListening(typeof(EntityListEntryClickedAction), x =>
        {
            var action = new EntityListEntryClickedAction(x);
            if(action.entity == this.getEntity())
            {
                mainController.setSelectedEntity(this);
            }
        });
    }

    /// <summary>
    /// Returns the Sprite associated with the vehicle.
    /// </summary>
    /// <returns>The Sprite associated with the vehicle.</returns>
    public abstract Sprite getSprite();

    /// <summary>
    /// Gets the location of the vehicle.
    /// </summary>
    /// <returns>A Location object representing the vehicle's location.</returns>
    public Location getLocation()
    {
        return this.getEntity().SpawnPoint;
    }

    /// <summary>
    /// Initializes the vehicle view controller with the specified category and color.
    /// </summary>
    /// <param name="cat">The AdversaryCategory to initialize the vehicle with.</param>
    /// <param name="color">The Color to initialize the vehicle with.</param>
    public abstract void init(AdversaryCategory cat, Color color);

    /// <summary>
    /// Changes the position of the entity to the given coordinates.
    /// </summary>
    /// <param name="x">The X coordinate of the new position.</param>
    /// <param name="y">The Y coordinate of the new position.</param>
    public virtual void onChangePosition(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
        mainController.moveActionButtons(transform.position);
    }

    /// <summary>
    /// Changes the rotation of the entity to the given angle.
    /// </summary>
    /// <param name="angle">The new angle in degrees.</param>
    public virtual void onChangeRotation(float angle)
    {
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    /// <summary>
    /// Changes the category of the entity to the given category.
    /// </summary>
    /// <param name="cat">The new AdversaryCategory of the entity.</param>
    public virtual void onChangeCategory(AdversaryCategory cat)
    {
        mainController.refreshEntityList();
    }

    /// <summary>
    /// Changes the model of the entity to the given model.
    /// </summary>
    /// <param name="model">The new EntityModel of the entity.</param>
    public void onChangeModel(EntityModel model)
    {
        mainController.refreshEntityList();
    }

    /// <summary>
    /// Changes the ID of the entity to the given ID.
    /// </summary>
    /// <param name="id">The new ID of the entity.</param>
    public void onChangeID(string id)
    {
        mainController.refreshEntityList();
    }

    /// <summary>
    /// Selects the entity and changes its position and material.
    /// </summary>
    public virtual void select()
    {
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.VEHICLE_SELECTED);
        sprite.material = selectionMaterial;
    }

    /// <summary>
    /// Deselects the entity and changes its position and material.
    /// </summary>
    public virtual void deselect()
    {
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.VEHICLE_DESELECTED);
        sprite.material = defaultMaterial;
    }

    /// <summary>
    /// Destroys the entity.
    /// </summary>
    public abstract void destroy();

    /// <summary>
    /// Gets the position of the entity.
    /// </summary>
    /// <returns>The position of the entity.</returns>
    public Vector2 getPosition()
    {
        return gameObject.transform.position;
    }

    /// <summary>
    /// Updates the entity's position and rotation.
    /// </summary>
    public void Update()
    {
        if(getEntity() == null)
        {
            return; //not initialized yet
        }
        if (!placed)
        {

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                this.destroy();
                EventManager.TriggerEvent(new CancelPlacementAction());
            }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!this.shouldIgnoreWaypoints())
            {
                var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
                getEntity().setPosition(mousePosition.x, mousePosition.y);
            }

        }
    }

    /// <summary>
    /// Handles the OnMouseDrag event to move the entity.
    /// </summary>
    public void OnMouseDrag()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
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

    /// <summary>
    /// Handles the OnMouseDown event to select and place the entity.
    /// </summary>
    public void OnMouseDown()
    {
        if(EventSystem.current.IsPointerOverGameObject())
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
            this.registerEntity();
        }
        mainController.setSelectedEntity(this);
    }

    /// <summary>
    /// Returns whether the entity should ignore waypoints.
    /// </summary>
    /// <returns>True if the entity should ignore waypoints, false otherwise.</returns>
    public bool shouldIgnoreWaypoints()
    {
        return this.ignoreWaypoints;
    }

    /// <summary>
    /// Sets whether the entity should ignore waypoints.
    /// </summary>
    /// <param name="b">True to ignore waypoints, false otherwise.</param>
    public virtual void setIgnoreWaypoints(bool b)
    {
        this.ignoreWaypoints = b;
    }

    /// <summary>
    /// Gets the BaseEntity instance of the entity.
    /// </summary>
    /// <returns>The BaseEntity instance of the entity.</returns>
    public abstract BaseEntity getEntity();
    /// <summary>
    /// Opens the edit dialog for the entity.
    /// </summary>
    public abstract void openEditDialog();
    /// <summary>
    /// Registers the entity with the main controller.
    /// </summary>
    protected abstract void registerEntity();
    /// <summary>
    /// Changes the color of the entity.
    /// </summary>
    /// <param name="c">The new color of the entity.</param>
    public abstract void onChangeColor(Color c);
}