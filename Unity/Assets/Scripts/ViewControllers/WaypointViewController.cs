using Entity;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class WaypointViewController : MonoBehaviour, IBaseEntityController
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
        this.sprite.color = color;
    }

    public void OnMouseDown()
    {
        mainController.setSelectedEntity(this);
    }
    public void OnMouseDrag()
    {
        var (_, waypoint) = snapController.FindLaneAndWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (waypoint is not null)
        {
            pathController.MoveWaypoint(this, waypoint.Location.Vector3);
        }
    }

    public void select()
    {
        gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    public void deselect()
    {
        gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    public Vector2 getPosition()
    {
        return gameObject.transform.position;
    }

    public BaseEntity getEntity()
    {
        return waypoint;
    }

    public void destroy()
    {
        Destroy(this.gameObject);
    }

    //create subinterface for action related methods
    public bool hasAction()
    {
        return false;
    }

    public void deleteAction()
    {
    }

    public void triggerActionSelection()
    {
    }
}
