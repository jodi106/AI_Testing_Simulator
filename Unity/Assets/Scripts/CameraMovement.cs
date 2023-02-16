using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    //Function Awake is called at Run
    public void Awake()
    {
        mapRenderer = GameObject.Find("Map").GetComponent<SpriteRenderer>();

        //Calculating the Edges for the Map(Background)
        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;

        EventManager.StartListening(typeof(MapPanAction), x =>
        {
            var action = new MapPanAction(x);
            PanCamera(action.origin);
        });
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //Verify if the Camera Size is in valid range
        if (cam.orthographicSize < maxCamSize && cam.orthographicSize > minCamSize)
        {
            if (cam.orthographic)
            {
                //If the camera can be resized simply (the normal size of the camera is changed)
                //Normal Case
                cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
            }
            else
            {
                //If the camera can't be resized simply (increase the field of view)
                //(Exception Case)
                cam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
            }
        }
        //Verify if Camera is in max Position
        //If yes , you can only zoom in (YOU CAN'T ZOOM OUT)
        if (cam.orthographicSize == maxCamSize && (Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed)>0)
        {
            if (cam.orthographic)
            {
                //If the camera can be resized simply (the normal size of the camera is changed)
                //Normal Case
                cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
            }
            else
            {
                //If the camera can't be resized simply (increase the field of view)
                //(Exception Case)
                cam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
            }
        }
        //Verify if Camera is in min Position
        //If yes , you can only zoom out (YOU CAN'T ZOOM IN)
        if (cam.orthographicSize == minCamSize && Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed<0)
        {
            if (cam.orthographic)
            {
                //If the camera can be resized simply (the normal size of the camera is changed)
                //Normal Case
                cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
            }
            else
            {
                //If the camera can't be resized simply (increase the field of view)
                //(Exception Case)
                cam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
            }
        }
    }

    private void PanCamera(Vector3 origin)
    {
        //Set the actual camera to new position using ClampCameraFunction
        Vector3 diff = origin - cam.ScreenToWorldPoint(Input.mousePosition);
        cam.transform.position = ClampCamera(cam.transform.position + diff);
    }

    public void ZoomIn()
    {
        //Simple Function for Only Zoom In
        //Currently Not In Use
        float newSize = cam.orthographicSize - zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    public void ZoomOut()
    {
        //Simple Function for Zoom Out
        //Currently Not In Use
        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

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

        return new Vector3(newX, newY, targetPosition.z);
    }

    /// Map Clicks

    [SerializeField]
    private GameObject WelcomeCanvas;

    [SerializeField]
    private GameObject WelcomeBackground;

    public void Home()
    {

        ///Set Welcome Menu On
        ///Set welcome Background On
        WelcomeCanvas.SetActive(true);
        WelcomeBackground.SetActive(true);

        //Set the new image as the new Map
        mapRenderer.sprite = null;
        EventManager.TriggerEvent(new MapChangeAction(""));
    }

    public void ViewMap(int number)
    {
        /// Switch Welcome Menu Off 
        /// Switch Welcome Background Off
        WelcomeCanvas.SetActive(false);
        WelcomeBackground.SetActive(false);

        var mapName = "Town" + (number == 10 ? "10HD" : ("0" + number));

        mapRenderer.sprite = Resources.Load<Sprite>("backgrounds/" + mapName);

        var map = GameObject.Find("Map");
        //Reset collider
        Destroy(map.GetComponent<BoxCollider2D>());
        map.AddComponent<BoxCollider2D>();
        //Recalculate Screen Edges
        Awake();
        EventManager.TriggerEvent(new MapChangeAction(mapName));
    }
}
