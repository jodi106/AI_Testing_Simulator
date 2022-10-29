using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MapController : MonoBehaviour
{

    Vector3 origin = Vector3.zero;
    private float downClickTime;
    private void OnMouseDown()
    {
        origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        downClickTime = Time.time;
    }

    private void OnMouseUp()
    {
        if (Time.time - downClickTime <= 0.3f)
        {
            EventManager.TriggerEvent(new ChangeSelectedEntityAction(ChangeSelectedEntityAction.NONE));
        }
    }

    private void OnMouseDrag()
    {
        EventManager.TriggerEvent(new MapPanAction(origin));
    }
}