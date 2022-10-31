using UnityEngine;
using UnityEngine.EventSystems;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MapController : MonoBehaviour
{

    Vector3 origin = Vector3.zero;
    private float downClickTime;
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        downClickTime = Time.time;
    }

    private void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        EventManager.TriggerEvent(new MapPanAction(origin));
    }
}