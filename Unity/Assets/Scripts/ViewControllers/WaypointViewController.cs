using Entity;
using System.Collections;
using UnityEngine;

public class WaypointViewController : MonoBehaviour
{
    // Use this for initialization
    private PathController pathController;
    public Waypoint wp { get; set; }

    public void setPathController(PathController pathController)
    {
        this.pathController = pathController;
    }

    void Awake()
    {

    }

    public void openEditDialog()
    {

    }

    // Update is called once per frame
    public void OnMouseDown()
    {
        openEditDialog();
    }
}
