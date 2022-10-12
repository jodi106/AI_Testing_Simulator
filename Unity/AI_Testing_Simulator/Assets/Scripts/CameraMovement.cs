using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private float mapMinX,mapMaxX,mapMinY,mapMaxY;

    private Vector3 dragOrigin;

    [SerializeField]
    private Text debugger;

    public void Awake()
    {
        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cam.orthographic)
        {
            cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
        }
        else
        {
            cam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
        }

        PanCamera();
        print_mouse_position();
    }

    private void PanCamera()
    {
        if(Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(0))
        {
            Vector3 diffrence = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            /// print("origin " + dragOrigin + " new Position " + cam.ScreenToWorldPoint(Input.mousePosition) + " =diffrence " + diffrence);

            cam.transform.position = ClampCamera(cam.transform.position + diffrence);

            ///print("Camera Position: " + cam.transform.position);
        }
    }

    public void ZoomIn()
    {
        /// print("ZoomIn");

        float newSize = cam.orthographicSize - zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    public void ZoomOut()
    {
        /// print("ZoomOut");

        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    public void print_mouse_position()
    {

        debugger.text = "Mouse Coords: (" + Input.mousePosition.x + " , " + Input.mousePosition.y + ")"; 
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
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

    /// Button Clicks
    
    [SerializeField]
    private GameObject WelcomeCanvas;

    [SerializeField]
    private GameObject WelcomeBackground;

    [SerializeField]
    private GameObject EditorCanvas;

    [SerializeField]
    private GameObject EditorBackgroundMap1;

    [SerializeField]
    private GameObject EditorBackgroundMap2;

    [SerializeField]
    private GameObject EditorBackgroundMap3;

    [SerializeField]
    private GameObject EditorBackgroundMap4;

    [SerializeField]
    private GameObject EditorBackgroundMap5;

    [SerializeField]
    private GameObject EditorBackgroundMap6;

    public void Home()
    {
        /// Switch Editor Off
        /// Switch Editor Background Off
        EditorCanvas.SetActive(false);
        EditorBackgroundMap1.SetActive(false);
        EditorBackgroundMap2.SetActive(false);
        EditorBackgroundMap3.SetActive(false);
        EditorBackgroundMap4.SetActive(false);
        EditorBackgroundMap5.SetActive(false);
        EditorBackgroundMap6.SetActive(false);

        ///Set Welcome Menu On
        ///Set welcome Background On
        WelcomeCanvas.SetActive(true);
        WelcomeBackground.SetActive(true);

        mapRenderer = GameObject.Find("MainBackground").GetComponent<SpriteRenderer>();
        Awake();
    }

    public void ViewMap1()
    {
        /// Switch Welcome Menu Off 
        /// Switch Welcome Background Off
        WelcomeCanvas.SetActive(false);
        WelcomeBackground.SetActive(false);
        
        /// Switch Editor On
        /// Switch Editor Background On
        EditorCanvas.SetActive(true);
        EditorBackgroundMap1.SetActive(true);

        

        mapRenderer = GameObject.Find("MapBackgroundTown1").GetComponent<SpriteRenderer>();
        Awake();
    }

    public void ViewMap2()
    {
        /// Switch Welcome Menu Off 
        /// Switch Welcome Background Off
        WelcomeCanvas.SetActive(false);
        WelcomeBackground.SetActive(false);

        /// Switch Editor On
        /// Switch Editor Background On
        EditorCanvas.SetActive(true);
        EditorBackgroundMap2.SetActive(true);

        mapRenderer = GameObject.Find("MapBackgroundTown2").GetComponent<SpriteRenderer>();
        Awake();
    }

    public void ViewMap3()
    {
        /// Switch Welcome Menu Off 
        /// Switch Welcome Background Off
        WelcomeCanvas.SetActive(false);
        WelcomeBackground.SetActive(false);

        /// Switch Editor On
        /// Switch Editor Background On
        EditorCanvas.SetActive(true);
        EditorBackgroundMap3.SetActive(true);

        mapRenderer = GameObject.Find("MapBackgroundTown3").GetComponent<SpriteRenderer>();
        Awake();
    }

    public void ViewMap4()
    {
        /// Switch Welcome Menu Off 
        /// Switch Welcome Background Off
        WelcomeCanvas.SetActive(false);
        WelcomeBackground.SetActive(false);

        /// Switch Editor On
        /// Switch Editor Background On
        EditorCanvas.SetActive(true);
        EditorBackgroundMap4.SetActive(true);

        mapRenderer = GameObject.Find("MapBackgroundTown4").GetComponent<SpriteRenderer>();
        Awake();
    }

    public void ViewMap5()
    {
        /// Switch Welcome Menu Off 
        /// Switch Welcome Background Off
        WelcomeCanvas.SetActive(false);
        WelcomeBackground.SetActive(false);

        /// Switch Editor On
        /// Switch Editor Background On
        EditorCanvas.SetActive(true);
        EditorBackgroundMap5.SetActive(true);

        mapRenderer = GameObject.Find("MapBackgroundTown5").GetComponent<SpriteRenderer>();
        Awake();
    }

    public void ViewMap6()
    {
        /// Switch Welcome Menu Off 
        /// Switch Welcome Background Off
        WelcomeCanvas.SetActive(false);
        WelcomeBackground.SetActive(false);

        /// Switch Editor On
        /// Switch Editor Background On
        EditorCanvas.SetActive(true);
        EditorBackgroundMap6.SetActive(true);

        mapRenderer = GameObject.Find("MapBackgroundTown10").GetComponent<SpriteRenderer>();
        Awake();
    }
}
