using UnityEngine;
public class DestinationController : MonoBehaviour
{
    public Material selectionMaterial;

    // Use this for initialization
    private EgoViewController ego;
    private SpriteRenderer sprite;
    private SnapController snapController;
    private bool placed;

    public void setEgo(EgoViewController ego)
    {
        this.ego = ego;
    }

    void Awake()
    {
        this.sprite = gameObject.GetComponent<SpriteRenderer>();
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.WAYPOINT_SELECTED);
        this.snapController = Camera.main.GetComponent<SnapController>();

        placed = false;

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            if (!placed)
            {
                this.Destroy();
            }
        });

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (!placed)
            {
                placed = true;
            }
        });
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void setColor(Color color)
    {
        this.sprite.color = color;
    }

    public void Update()
    {
        if (!placed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (waypoint is not null)
            {
                gameObject.transform.position = new Vector3(waypoint.X, waypoint.Y, HeightUtil.WAYPOINT_SELECTED);
            }
        }
    }

    public void OnMouseDown()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!placed)
        {
            placed = true;
        }
    }
    public void deselect()
    {
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.WAYPOINT_DESELECTED);
    }


    public void select()
    {
        gameObject.transform.position = HeightUtil.SetZ(gameObject.transform.position, HeightUtil.WAYPOINT_SELECTED);
    }

        public void OnMouseDrag()
    {
        var waypoint = snapController.FindWaypoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (waypoint is not null)
        {
            gameObject.transform.position = new Vector3(waypoint.X, waypoint.Y, HeightUtil.WAYPOINT_SELECTED);
            ego.submitDestination(waypoint);
        }
    }
}
