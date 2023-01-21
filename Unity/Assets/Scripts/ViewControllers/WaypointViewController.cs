using Assets.Enums;
using Entity;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class WaypointViewController : MonoBehaviour, IBaseController, IBaseView
{
    // Use this for initialization
    private PathController pathController;
    public Waypoint waypoint { get; set; }
    private SpriteRenderer sprite;
    private SnapController snapController;
    private MainController mainController;

    public void setPathController(PathController pathController)
    {
        this.pathController = pathController;
    }

    void Awake()
    {
        this.sprite = gameObject.GetComponent<SpriteRenderer>();
        this.snapController = Camera.main.GetComponent<SnapController>();
        this.mainController = Camera.main.GetComponent<MainController>();
    }

    public void openEditDialog()
    {

    }

    public void setColor(Color color)
    {
        this.sprite.color = new Color(color.r, color.g, color.b, 1);
    }

    public void OnMouseDown()
    {
        if (!pathController.isBuilding())
        {
            mainController.setSelectedEntity(this);
        }
    }
    public void OnMouseDrag()
    {
        var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (waypoint is not null)
        {
            pathController.MoveWaypoint(this, waypoint);
        }
    }

    public void select()
    {
        gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        pathController.Select();
    }

    public void deselect()
    {
        gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        pathController.Deselect();
    }

    public Vector2 getPosition()
    {
        return gameObject.transform.position;
    }

    public void destroy()
    {
        pathController.removeWaypoint(this);
        Destroy(this.gameObject);
    }

    public void onChangePosition(Location pos)
    {
        transform.position = new Vector3(pos.X, pos.Y, transform.position.z);
    }

    public void onChangeColor(Color c)
    {

    }
}
