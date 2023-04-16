using UnityEngine;

/// <summary>
/// Controller for the Destination of the Ego vehicle.
/// </summary>
public class DestinationController : MonoBehaviour
{
    public Material selectionMaterial;

    // Use this for initialization
    private EgoViewController ego;
    private SpriteRenderer sprite;
    private SnapController snapController;
    private bool placed;

    void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.WAYPOINT_SELECTED);
        snapController = Camera.main.GetComponent<SnapController>();

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (!placed)
            {
                placed = true;
                var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                ego.SubmitDestination(waypoint);
            }
        });
    }

    /// <summary>
    /// Initialize the controller with an EgoViewController and a color.
    /// </summary>
    /// <param name="ego">The EgoViewController to be initialized with.</param>
    /// <param name="color">The color to set the waypoint to.</param>
    /// <param name="placed">Whether the waypoint has already been placed or not.</param>
    public void Init(EgoViewController ego, Color color, bool placed = false)
    {
        this.ego = ego;
        this.placed = placed;
    }

    /// <summary>
    /// Destroys the GameObject.
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets the color of the Destination GameObject.
    /// </summary>
    /// <param name="color">The color to set the Destination GameObject to.</param>
    public void SetColor(Color color)
    {
        sprite.color = color;
    }

    /// <summary>
    /// Returns whether the Destination has been placed or not.
    /// </summary>
    /// <returns>Whether the Destination has been placed or not.</returns>
    public bool IsPlaced()
    {
        return placed;
    }

    /// <summary>
    /// Updates the position of the Destination based on the mouse position.
    /// </summary>
    public void Update()
    {
        if (!placed)
        {
            var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (waypoint is not null)
            {
                gameObject.transform.position = new Vector3(waypoint.X, waypoint.Y, HeightUtil.WAYPOINT_SELECTED);
            }
        }
    }

    /// <summary>
    /// Called when the Destination GameObject is clicked.
    /// Places the vehicle if it is not placed yet.
    /// </summary>
    public void OnMouseDown()
    {
        if (!placed)
        {
            placed = true;
        }
    }

    /// <summary>
    /// Deselects the waypoint.
    /// </summary>
    public void Deselect()
    {
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.WAYPOINT_DESELECTED);
    }

    /// <summary>
    /// Selects the waypoint.
    /// </summary>
    public void Select()
    {
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.WAYPOINT_SELECTED);
    }

    /// <summary>
    /// Called when the Destination GameObject is being dragged.
    /// </summary>
    public void OnMouseDrag()
    {
        var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (waypoint is not null)
        {
            gameObject.transform.position = new Vector3(waypoint.X, waypoint.Y, HeightUtil.WAYPOINT_SELECTED);
            ego.SubmitDestination(waypoint);
        }
    }
}
