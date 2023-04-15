using UnityEngine;

/// <summary>
/// Controller for a destination waypoint in the scene.
/// </summary>
public class DestinationController : MonoBehaviour
{
    public Material selectionMaterial;

    // Use this for initialization
    private EgoViewController ego;
    private SpriteRenderer sprite;
    private SnapController snapController;
    private bool placed;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
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
    /// Destroys the destination waypoint.
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets the color of the waypoint.
    /// </summary>
    /// <param name="color">The color to set the waypoint to.</param>
    public void SetColor(Color color)
    {
        sprite.color = color;
    }

    /// <summary>
    /// Returns whether the waypoint has been placed or not.
    /// </summary>
    /// <returns>Whether the waypoint has been placed or not.</returns>
    public bool IsPlaced()
    {
        return placed;
    }

    /// <summary>
    /// Updates the position of the waypoint based on the mouse position.
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
    /// Called when the destination waypoint is clicked.
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
    /// Called when the destination waypoint is being dragged.
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
