using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// This class handles the camera movement, zoom, and map boundaries in a Unity scene.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float zoomStep, minCamSize, maxCamSize;

    [SerializeField]
    private float ScrollSpeed = 10;

    [SerializeField]
    public SpriteRenderer mapRenderer;

    [SerializeField]
    private float mapMinX, mapMaxX, mapMinY, mapMaxY;

    /// <summary>
    /// Called at runtime when the script instance is being loaded.
    /// Initializes the map renderer, event listeners, and calculates map edges.
    /// </summary>
    public void Awake()
    {
        mapRenderer = GameObject.Find("Map").GetComponent<SpriteRenderer>();

        EventManager.StartListening(typeof(MapPanAction), x =>
        {
            var action = new MapPanAction(x);
            PanCamera(action.origin);
        });

        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            var action = new MapChangeAction(x);

            if (action.name != "")
            {
                /// Switch Welcome Menu Off
                /// Switch Welcome Background Off
                WelcomeCanvas.SetActive(false);
                WelcomeBackground.SetActive(false);

                mapRenderer.sprite = Resources.Load<Sprite>("backgrounds/" + action.name);

                var map = GameObject.Find("Map");
                //Reset collider
                Destroy(map.GetComponent<BoxCollider2D>());
                map.AddComponent<BoxCollider2D>();
                //Recalculate Screen Edges
                recalulateEdges();
            } else
            {
                ///Set Welcome Menu On
                ///Set welcome Background On
                WelcomeCanvas.SetActive(true);
                WelcomeBackground.SetActive(true);

                //Set the new image as the new Map
                mapRenderer.sprite = null;
            }

            cam.orthographicSize = 10;
            cam.transform.position = ClampCamera(new Vector3(0, 0, -10));
        });

        recalulateEdges();
    }


    /// <summary>
    /// Recalculates the edges of the map.
    /// </summary>
    void recalulateEdges()
    {
        //Calculating the Edges for the Map(Background)
        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;
    }

    /// <summary>
    /// Update is called once per frame.
    /// Handles camera zooming and checks if the pointer is over a UI element.
    /// </summary>
    void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        else
        {
            cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
            if (cam.orthographicSize > maxCamSize) cam.orthographicSize = maxCamSize;
            if (cam.orthographicSize < minCamSize) cam.orthographicSize = minCamSize;
        }
    }

    /// <summary>
    /// Pans the camera to the specified origin.
    /// </summary>
    /// <param name="origin">The target origin to pan the camera to.</param>
    private void PanCamera(Vector3 origin)
    {
        //Set the actual camera to new position using ClampCameraFunction
        Vector3 diff = origin - cam.ScreenToWorldPoint(Input.mousePosition);
        cam.transform.position = ClampCamera(cam.transform.position + diff);
    }

    /// <summary>
    /// Clamps the camera position to the map boundaries.
    /// </summary>
    /// <param name="targetPosition">The target position to move the camera to.</param>
    /// <returns>The clamped camera position.</returns>
    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        //Moves the actual camera to target position
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;

        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, -10);
    }

    /// Map Clicks

    [SerializeField]
    private GameObject WelcomeCanvas;

    [SerializeField]
    private GameObject WelcomeBackground;

    /// <summary>
    /// Handles the "Home" button click event.
    /// </summary>
    public async void Home()
    {
        EventManager.TriggerEvent(new MapChangeAction(""));
    }

    /// <summary>
    /// Handles the "View Map" button click event.
    /// </summary>
    /// <param name="number">The map number to be displayed.</param>
    public void ViewMap(int number)
    {
        var mapName = "Town" + (number == 10 ? "10HD" : ("0" + number));
        EventManager.TriggerEvent(new MapChangeAction(mapName));
    }
}
