using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class will allow the user to move the camera freely to create a larger editor area, and to zoom in and out
/// </summary>
public class CameraMover : MonoBehaviour
{
    public float dragSpeed = 200f;
    public float zoomSpeed = 500f;
    public float minZoom = 1f;
    public float maxZoom = 5000f;

    private Camera mainCamera;
    private Vector3 lastMousePosition;

    /// <summary>
    /// This mehtod is called before the first frame and retrieves the Camera
    /// </summary>
    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    /// <summary>
    /// This method is called with every frame and will check for input of the user that could trigger camera effects, such as Zooming and moving
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (!EventSystem.current.IsPointerOverGameObject() && !IsPointerOverGameObject()) // Check if mouse is over UI object
            {
                lastMousePosition = Input.mousePosition;
            }
        }

        if (Input.GetMouseButton(2))
        {
            if (!EventSystem.current.IsPointerOverGameObject() && !IsPointerOverGameObject())
            {
                Vector3 deltaMousePosition = lastMousePosition - Input.mousePosition;
                Vector3 panMovement = new Vector3(deltaMousePosition.x, deltaMousePosition.y, 0f) * dragSpeed * Time.deltaTime;
                transform.Translate(panMovement, Space.World);
                lastMousePosition = Input.mousePosition;
            }
        }

        // Zoom in
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            float newZoom = mainCamera.orthographicSize - 0.1f * zoomSpeed;
            mainCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
        // Zoom out
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            float newZoom = mainCamera.orthographicSize + 0.1f * zoomSpeed;
            mainCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }

    /// <summary>
    /// This method will check, whether the users cursor is currently over a gameobject. If there is a gameobject with a collider, then this method will return true
    /// </summary>
    /// <returns> True, if the mouse of the user is currently over a gameobject with a collider </returns>
    private bool IsPointerOverGameObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Collider>() != null)
            {
                return true;
            }
        }

        return false;
    }
}
