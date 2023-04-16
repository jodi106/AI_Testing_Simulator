using Assets.Enums;
using Entity;
using System.Collections;
using System.Drawing;
using System.Numerics;
using UnityEngine;
using Color = UnityEngine.Color;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// The WaypointViewController class handles the behavior of a waypoint in the game.
/// </summary
public class WaypointViewController : MonoBehaviour, IBaseController, IBaseView
{
    // Use this for initialization
    private PathController pathController;
    public Waypoint Waypoint { get; set; }
    private SpriteRenderer sprite;
    private SnapController snapController;
    private MainController mainController;
    private WaypointSettingsPopupController settingsController;
    // strategy field in Waypoint corresponds to this field and is kept in sync
    private bool ignoreWaypoints = false;
    private bool secondary = false;

    /// <summary>
    /// Initializes the Waypoint controller with the specified parameters.
    /// </summary>
    /// <param name="waypoint">The waypoint associated with this controller.</param>
    /// <param name="pathController">The controller of the Path that this waypoint is part of.</param>
    /// <param name="color">The color for this waypoint.</param>
    /// <param name="pathController">Indicates wether this waypoint should be secondary.</param>
    public void Init(Waypoint waypoint, PathController pathController, Color color, bool secondary)
    {
        this.Waypoint = waypoint;
        waypoint.View = this;
        this.pathController = pathController;
        this.ignoreWaypoints = waypoint.Strategy == WaypointStrategy.SHORTEST ? true : false;
        OnChangeColor(color);
        if (secondary)
        {
            this.MakeSecondary();
        }
    }

    /// <summary>
    /// Sets the PathController for the WaypointViewController.
    /// </summary>
    /// <param name="pathController">The PathController to set.</param>
    public void SetPathController(PathController pathController)
    {
        this.pathController = pathController;
    }

    /// <summary>
    /// Gets the PathController for the WaypointViewController.
    /// </summary>
    /// <returns>The PathController for the WaypointViewController.</returns>
    public PathController GetPathController()
    {
        return this.pathController;
    }

    /// <summary>
    /// Sets the MainController, SnapController and SpriteRenderer for the WaypointViewController at initialization.
    /// </summary>
    void Awake()
    {
        this.sprite = gameObject.GetComponent<SpriteRenderer>();
        this.snapController = Camera.main.GetComponent<SnapController>();
        this.mainController = Camera.main.GetComponent<MainController>();
    }

    /// <summary>
    /// Makes the waypoint a secondary waypoint.
    /// </summary>
    public void MakeSecondary()
    {
        secondary = true;
        var c = this.sprite.color;
        this.sprite.color = new Color(c.r, c.g, c.b, 0.4f);
    }

    /// <summary>
    /// Returns whether the waypoint is a secondary waypoint.
    /// </summary>
    /// <returns>True if the waypoint entity is a secondary waypoint, false otherwise.</returns>
    public bool IsSecondary()
    {
        return secondary;
    }

    /// <summary>
    /// Opens the edit dialog for the waypoint.
    /// </summary>
    public void OpenEditDialog()
    {
        this.settingsController = GameObject.Find("PopUps").transform.Find("WaypointSettingsPopUp").gameObject.GetComponent<WaypointSettingsPopupController>();
        this.settingsController.gameObject.SetActive(true);
        this.settingsController.Open(this, pathController.AdversaryViewController.GetEntity(), mainController.Info.Vehicles, mainController.warningPopupController);
    }

    /// <summary>
    /// Sets the color of the waypoint
    /// </summary>
    /// <param name="color">The new color for the waypoint entity.</param>
    public void SetColor(Color color)
    {
        this.sprite.color = new Color(color.r, color.g, color.b, 1);
    }

    /// <summary>
    /// Handles the OnMouseDown event to select the waypoint.
    /// </summary>
    public void OnMouseDown()
    {
        if (MainController.freeze) return;
        if (snapController.IgnoreClicks && !pathController.IsBuilding())
        {
            EventManager.TriggerEvent(new MouseClickAction(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
        else
        {
            if (!secondary) mainController.SetSelectedEntity(this);
        }

    }

    /// <summary>
    /// Handles the OnMouseDrag event to move the waypoint.
    /// </summary>
    public void OnMouseDrag()
    {
        if (MainController.freeze) return;
        if (snapController.IgnoreClicks && !pathController.IsBuilding())
        {
            EventManager.TriggerEvent(new MouseClickAction(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            return;
        }
        if (secondary)
        {
            return;
        }
        if (!this.IsIgnoringWaypoints())
        {
            var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (waypoint is not null)
            {
                pathController.MoveWaypoint(this, waypoint.X, waypoint.Y);
            }
        }
        else
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pathController.MoveWaypoint(this, position.x, position.y);
        }
    }

    /// <summary>
    /// Selects the waypoint and scales it up to make it distinctly visible.
    /// </summary>
    /// <param name="pathSelected">If set to true, also selects the waypoint's path.</param>
    public void Select()
    {
        if (MainController.freeze) return;
        gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        pathController.Select(true);
    }

    /// <summary>
    /// Deselects the waypoint and scales it down again.
    /// </summary>
    /// <param name="pathDeselected">If set to true, also deselects the waypoint's path.</param>
    public void Deselect()
    {
        gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        pathController.Deselect(true);
    }

    /// <summary>
    /// Removes the waypoint from its path and destroys the game object.
    /// </summary>
    public void Destroy()
    {
        pathController.RemoveWaypoint(this);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Changes the position of the waypoint GameObject.
    /// </summary>
    /// <param name="x">The new x-coordinate.</param>
    /// <param name="y">The new y-coordinate.</param>
    public void OnChangePosition(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }

    /// <summary>
    /// Gets Called when Rotation is Changed (Not implemented for Waypoint)
    /// </summary>
    /// <param name="angle"></param>
    public void OnChangeRotation(float angle)
    {

    }

    /// <summary>
    /// Gets called when Color is Changed.
    /// </summary>
    /// <param name="c"></param>
    public void OnChangeColor(Color color)
    {
        this.sprite.color = new Color(color.r, color.g, color.b, 1);
    }

    /// <summary>
    /// Returns whether the waypoint is ignoring waypoint indicators.
    /// </summary>
    /// <returns>True if the waypoint should ignore waypoint indicators, false otherwise.</returns>
    public bool IsIgnoringWaypoints()
    {
        return this.ignoreWaypoints;
    }

    /// <summary>
    /// Sets whether the waypoint should ignore waypoint indicators.
    /// </summary>
    /// <param name="ignore">True to ignore waypoint indicators, false to not ignore them.</param>
    public void ShouldIgnoreWaypoints(bool b)
    {
        ignoreWaypoints = b;
        Waypoint.Strategy = b ? WaypointStrategy.SHORTEST : WaypointStrategy.FASTEST;
        if(!ignoreWaypoints)
        {
            var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (waypoint is not null)
            {
                pathController.MoveWaypoint(this, waypoint.X, waypoint.Y);
            }
        }
        pathController.UpdateAdjacentPaths(this);
    }

    /// <summary>
    /// Returns the location of the waypoint.
    /// </summary>
    /// <returns>The location of the waypoint.</returns>
    public Location GetLocation()
    {
        return this.Waypoint.Location;
    }

}
