using Assets.Enums;
using Entity;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// The WaypointViewController class handles the behavior of a waypoint in the game.
/// </summary
public class WaypointViewController : MonoBehaviour, IBaseController, IBaseView
{
    // Use this for initialization
    private PathController pathController;
    public Waypoint waypoint { get; set; }
    private SpriteRenderer sprite;
    private SnapController snapController;
    private MainController mainController;
    private WaypointSettingsPopupController settingsController;
    private bool ignoreWaypoints = false;
    private bool secondary = false;

    /// <summary>
    /// Sets the PathController for the WaypointViewController.
    /// </summary>
    /// <param name="pathController">The PathController to set.</param>
    public void setPathController(PathController pathController)
    {
        this.pathController = pathController;
    }

    /// <summary>
    /// Gets the PathController for the WaypointViewController.
    /// </summary>
    /// <returns>The PathController for the WaypointViewController.</returns>
    public PathController getPathController()
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
    /// Makes the waypoint entity a secondary waypoint.
    /// </summary>
    public void makeSecondary()
    {
        secondary = true;
        var c = this.sprite.color;
        this.sprite.color = new Color(c.r, c.g, c.b, 0.4f);
    }

    /// <summary>
    /// Returns whether the waypoint entity is a secondary waypoint.
    /// </summary>
    /// <returns>True if the waypoint entity is a secondary waypoint, false otherwise.</returns>
    public bool isSecondary()
    {
        return secondary;
    }

    /// <summary>
    /// Opens the edit dialog for the waypoint.
    /// </summary>
    public void openEditDialog()
    {
        this.settingsController = GameObject.Find("PopUps").transform.Find("WaypointSettingsPopUp").gameObject.GetComponent<WaypointSettingsPopupController>();
        this.settingsController.gameObject.SetActive(true);
        this.settingsController.open(this, pathController.adversaryViewController.getEntity(), mainController.info.Vehicles, mainController.warningPopupController);
    }

    /// <summary>
    /// Sets the color of the waypoint
    /// </summary>
    /// <param name="color">The new color for the waypoint entity.</param>
    public void setColor(Color color)
    {
        this.sprite.color = new Color(color.r, color.g, color.b, 1);
    }

    /// <summary>
    /// Handles the OnMouseDown event to select the waypoint.
    /// </summary>
    public void OnMouseDown()
    {
        if (MainController.freeze) return;
        if (snapController.IgnoreClicks && !pathController.isBuilding())
        {
            EventManager.TriggerEvent(new MouseClickAction(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
        else
        {
            if(!secondary) mainController.setSelectedEntity(this);
        }

    }

    /// <summary>
    /// Handles the OnMouseDrag event to move the waypoint.
    /// </summary>
    public void OnMouseDrag()
    {
        if (MainController.freeze) return;
        if (snapController.IgnoreClicks && !pathController.isBuilding())
        {
            EventManager.TriggerEvent(new MouseClickAction(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            return;
        }
        if(secondary)
        {
            return;
        }
        if (!this.shouldIgnoreWaypoints())
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
    public void select()
    {
        if (MainController.freeze) return;
        gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        pathController.select(true);
    }

    /// <summary>
    /// Deselects the waypoint and scales it down again.
    /// </summary>
    /// <param name="pathDeselected">If set to true, also deselects the waypoint's path.</param>
    public void deselect()
    {
        gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        pathController.deselect(true);
    }

    /// <summary>
    /// Removes the waypoint from its path and destroys the game object.
    /// </summary>
    public void destroy()
    {
        pathController.removeWaypoint(this);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Changes the position of the waypoint.
    /// </summary>
    /// <param name="x">The new x-coordinate.</param>
    /// <param name="y">The new y-coordinate.</param>
    public void onChangePosition(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }

    /// <summary>
    /// Gets Called when Rotation is Changed (Not implemented for Waypoint)
    /// </summary>
    /// <param name="angle"></param>
    public void onChangeRotation(float angle)
    {
        
    }

    /// <summary>
    /// Gets called when Color is Changed (Not implemented for Waypoint)
    /// </summary>
    /// <param name="c"></param>
    public void onChangeColor(Color c)
    {

    }

    /// <summary>
    /// Returns whether the waypoint should ignore other waypoints on its path.
    /// </summary>
    /// <returns>True if the waypoint should ignore other waypoints, false otherwise.</returns>
    public bool shouldIgnoreWaypoints()
    {
        return this.ignoreWaypoints;
    }

    /// <summary>
    /// Sets whether the waypoint should ignore other waypoints on its path.
    /// </summary>
    /// <param name="ignore">True to ignore other waypoints, false to not ignore them.</param>
    public void setIgnoreWaypoints(bool b)
    {
        this.ignoreWaypoints = b;
    }

    /// <summary>
    /// Returns the location of the waypoint.
    /// </summary>
    /// <returns>The location of the waypoint.</returns>
    public Location getLocation()
    {
        return this.waypoint.Location;
    }

}
