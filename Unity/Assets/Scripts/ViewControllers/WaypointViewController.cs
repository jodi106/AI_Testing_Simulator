using Entity;
using System.Collections;
using UnityEngine;

public class WaypointViewController : MonoBehaviour
{
    // Use this for initialization
    private PathController pathController;
    public Waypoint waypoint { get; set; }
    private SpriteRenderer sprite;

    public void setPathController(PathController pathController)
    {
        this.pathController = pathController;
    }

    void Awake()
    {
        this.sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    public void openEditDialog()
    {

    }

    public void setColor(Color color)
    {
        this.sprite.color = color;
    }

    // Update is called once per frame
    public void OnMouseDown()
    {
        openEditDialog();
    }
}
