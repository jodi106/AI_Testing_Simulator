using Assets.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


namespace scripts
{
    /// <summary>
    /// 
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
        /// 
        /// </summary>
        private void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"> </param>
        public void AddSidebarButton(Button button)
        {
            sidebarButtonList.Add(button);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roadType"> </param>
        public void SetSelectedRoadType(RoadType roadType)
        {
            this.selectedRoadType = roadType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> </returns>
        public RoadType GetSelectedRoadType()
        {
            return this.selectedRoadType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"> </param>
        /// <param name="roadType"> </param>
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
