using UnityEngine;
using UnityEngine.EventSystems;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MapController : MonoBehaviour
{

    Vector3 origin = Vector3.zero;
    private float downClickTime;
    private SnapController snapController;

    private void Start()
    {
        snapController = Camera.main.GetComponent<SnapController>();
    }
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
            EventManager.TriggerEvent(new ChangeSelectedEntityAction(ChangeSelectedEntityAction.NONE));
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