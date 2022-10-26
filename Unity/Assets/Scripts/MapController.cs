using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MapController : MonoBehaviour
{

    Vector3 origin = Vector3.zero;
    private void OnMouseDown()
    {
        origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        EventManager.TriggerEvent(new MapPanAction(origin, origin));
    }
}