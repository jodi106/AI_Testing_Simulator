using UnityEngine;
using UnityEngine.EventSystems;

public class ActionButtonMouseOverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string toolTipText;

    public void OnPointerEnter(PointerEventData eventData) {
        var pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        pos = new Vector2(pos.x, Camera.main.pixelHeight - pos.y);
        MainController.moveToolTip(pos, Vector2.down, toolTipText);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MainController.hideToolTip();
    }
}