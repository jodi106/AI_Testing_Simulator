using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Enums;

namespace scripts
{
    /// <summary>
    /// This class will handle the buttons of the bottom bar when dragging a piece onto the map
    /// </summary>
    public class SidebarButtonHandler : MonoBehaviour, IPointerDownHandler
    {
        public Button SidebarButton;

        /// <summary>
        /// This method is called before the first frame update. This will add the button for the road piece to the list of buttons
        /// </summary>
        void Start()
        {
            ButtonManager.Instance.AddSidebarButton(SidebarButton);
        }


        /// <summary>
        /// This method reacts to a mouse button down event on a button. 
        /// This is necessary, because we want to click, hold and instantly drag a new road. 
        /// With a "onClickEvent" we would have to perform an entire click.
        /// </summary>
        /// <param name="eventData"> the event that is triggered, which, in this context, is a click</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                SelectRoadType(gameObject.name);
            }
        }

        /// <summary>
        /// This method sets the selected road type according to the button pressed
        /// </summary>
        /// <param name="buttonName"> the name of the button </param>
        void SelectRoadType(string buttonName)
        {
            RoadManager.Instance.DeselectRoad();
            var selectedRoadType = RoadType.None;
            switch (buttonName)
            {
                case "Straight":
                    selectedRoadType = RoadType.StraightRoad;
                    break;
                case "Turn":
                    selectedRoadType = RoadType.Turn;
                    break;
                case "Turn15":
                    selectedRoadType = RoadType.Turn15;
                    break;
                case "ThreeWayIntersection":
                    selectedRoadType = RoadType.ThreeWayIntersection;
                    break;
                case "FourWayIntersection":
                    selectedRoadType = RoadType.FourWayIntersection;
                    break;
                case "ParkingBottom":
                    selectedRoadType = RoadType.ParkingBottom;
                    break;
                case "ParkingTop":
                    selectedRoadType = RoadType.ParkingTop;
                    break;
                case "ParkingTopBottom":
                    selectedRoadType = RoadType.ParkingTopAndBottom;
                    break;
                case "Crosswalk":
                    selectedRoadType = RoadType.Crosswalk;
                    break;
                case "FourWayRoundAbout":
                    selectedRoadType = RoadType.FourWayRoundAbout;
                    break;
                case "ThreeWayRoundAbout":
                    selectedRoadType = RoadType.ThreeWayRoundAbout;
                    break;
                case "ThreeWayRoundAboutAdvanced":
                    selectedRoadType = RoadType.ThreeWayRoundAboutAdvanced;
                    break;
                case "FourWayRoundAboutAdvanced":
                    selectedRoadType = RoadType.FourWayRoundAboutAdvanced;
                    break;
                default:
                    break;
            }
            ButtonManager.Instance.HandleButtonClick(SidebarButton, selectedRoadType);
        }
    }
}