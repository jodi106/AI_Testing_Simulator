using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace scripts
{
    public class MouseController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
           
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetHoverState(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetHoverState(false);
        }

        /**
         * Method to define what should happen, if the mouse is hovering over a UI element 
         */
        public void SetHoverState(bool isHovering)
        {
            if (isHovering)
            {
                Debug.Log("Mouse is hovering over UI element: " + gameObject.name);
            }
            else
            {
                Debug.Log("Mouse is no longer hovering over UI element: " + gameObject.name);
            }
        }


    }

}