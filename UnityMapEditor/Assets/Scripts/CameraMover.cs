using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMover : MonoBehaviour
{
    public float dragSpeed = 200f;
    public float zoomSpeed = 500f;
    public float minZoom = 1f;
    public float maxZoom = 5000f;

    private Camera mainCamera;
    private Vector3 lastMousePosition;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

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

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float newZoom = mainCamera.orthographicSize - scrollInput * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);

    }

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
