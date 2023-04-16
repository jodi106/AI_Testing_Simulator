using UnityEngine;
using UnityEngine.EventSystems;

public class ActionButtonMouseOverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string toolTipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var offset = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.25f , gameObject.transform.position.z);
        var pos = Camera.main.WorldToScreenPoint(offset);
        pos = new Vector2(pos.x, Camera.main.pixelHeight - pos.y);
        MainController.MoveToolTip(pos, Vector2.down, toolTipText);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MainController.HideToolTip();
    }
}