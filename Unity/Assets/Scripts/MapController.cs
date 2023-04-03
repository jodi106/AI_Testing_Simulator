using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// MapController class handles map-related interactions such as panning, clicking, and dragging.
/// </summary>
public class MapController : MonoBehaviour
{

    Vector3 origin = Vector3.zero;
    private float downClickTime;

    /// <summary>
    /// Called when the mouse button is pressed down.
    /// </summary>
    private void OnMouseDown()
    {
        origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (shouldIgnoreMouseAction())
        {
            return;
        }
        downClickTime = Time.time;
    }

    /// <summary>
    /// Called when the mouse button is released.
    /// </summary>
    private void OnMouseUp()
    {
        if (shouldIgnoreMouseAction())
        {
            return;
        }
        if (Time.time - downClickTime <= 0.3f)
        {
            EventManager.TriggerEvent(new MouseClickAction(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
    }

    /// <summary>
    /// Called when the mouse is being dragged.
    /// </summary>
    private void OnMouseDrag()
    {
        if (shouldIgnoreMouseAction())
        {
            return;
        }
        EventManager.TriggerEvent(new MapPanAction(origin));
    }

    /// <summary>
    /// Determines if the mouse action should be ignored based on whether the pointer is over a game object.
    /// </summary>
    /// <returns>Returns true if the pointer is over a game object, false otherwise.</returns>
    public bool shouldIgnoreMouseAction() {
        return EventSystem.current.IsPointerOverGameObject();
    }
}