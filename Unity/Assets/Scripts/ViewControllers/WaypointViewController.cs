using Entity;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class WaypointViewController : MonoBehaviour
{
    // Use this for initialization
    private PathController pathController;
    public Waypoint waypoint { get; set; }
    private SpriteRenderer sprite;
    private SnapController snapController;

    public void setPathController(PathController pathController)
    {
        this.pathController = pathController;
    }

    void Awake()
    {
        this.sprite = gameObject.GetComponent<SpriteRenderer>();
        this.snapController = Camera.main.GetComponent<SnapController>();
    }

    public void openEditDialog()
    {

    }

    public void setColor(Color color)
    {
        this.sprite.color = color;
    }
    public void OnMouseDrag()
    {
        var (_, waypoint) = snapController.FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (waypoint is not null)
        {
            pathController.MoveWaypoint(this, waypoint.Location.Vector3);
        }
    }
}
