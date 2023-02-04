using UnityEngine;
using UnityEngine.EventSystems;

public class MapController : MonoBehaviour
{

    Vector3 origin = Vector3.zero;
    private float downClickTime;

    private void OnMouseDown()
    {
        origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (shouldIgnoreMouseAction())
        {
            return;
        }
        downClickTime = Time.time;
    }

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

    private void OnMouseDrag()
    {
        if (shouldIgnoreMouseAction())
        {
            return;
        }
        EventManager.TriggerEvent(new MapPanAction(origin));
    }

    public bool shouldIgnoreMouseAction() {
        return EventSystem.current.IsPointerOverGameObject();
    }
}