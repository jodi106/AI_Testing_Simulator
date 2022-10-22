using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// PointerEventDrag.dragging example.


//
// Create a 2D Project and add a Canvas and an Image as a child. Position the Image in the center
// of the Canvas. Resize the Image to approximately a quarter of the height and width. Create a
// Resources folder and add a sprite. Set the sprite to the Image component. Then add this script
// to the Image. Then press the Play button. The Image should be clickable and moved with the
// mouse or trackpad.

public class Car : MonoBehaviour
{
    public void OnMouseDrag()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        transform.Translate(mousePosition);
    }
}