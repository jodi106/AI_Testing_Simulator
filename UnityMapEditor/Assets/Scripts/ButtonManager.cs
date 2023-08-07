using Assets.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


namespace scripts
{
    /// <summary>
    /// This class handles the buttons in the bottom bar where the user can select a road piece to drag onto the map
    /// </summary>
    public class ButtonManager : MonoBehaviour
    {
        public List<Button> sidebarButtonList { get; set; } = new List<Button>();
        private static ButtonManager instance;
        public static ButtonManager Instance
        {
            get
            {
                return instance;
            }
        }
        private RoadType selectedRoadType { get; set; }

        /// <summary>
        /// This method is called before the first frame and before the start method and initialized the button manager
        /// </summary>
        private void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// This method adds the button to the list of buttons
        /// </summary>
        /// <param name="button"> The button to be added </param>
        public void AddSidebarButton(Button button)
        {
            sidebarButtonList.Add(button);
        }

        /// <summary>
        /// This method sets the roadtype of the road that is being created
        /// </summary>
        /// <param name="roadType"> the roadtype that the piece should be assigned </param>
        public void SetSelectedRoadType(RoadType roadType)
        {
            this.selectedRoadType = roadType;
        }

        /// <summary>
        /// This method gets the road type of the selected road
        /// </summary>
        /// <returns> The Road type of the selected road </returns>
        public RoadType GetSelectedRoadType()
        {
            return this.selectedRoadType;
        }

        /// <summary>
        /// This method handles the button click on the buttons and creates a road.
        /// </summary>
        /// <param name="button"> The clicked button </param>
        /// <param name="roadType"> The roadtype the created piece should be assigned </param>
        public void HandleButtonClick(Button button, RoadType roadType)
        {
            if (!ScrollViewOpener.IsUserGuideOpen())
            {
                SetSelectedRoadType(roadType);
                RoadManager.Instance.CreateRoad();
            }
        }
    }
}
