using Assets.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace scripts
{
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

        private void Awake()
        {
            instance = this;
        }

        public void AddSidebarButton(Button button)
        {
            sidebarButtonList.Add(button);
        }

        public void SetSelectedRoadType(RoadType roadType)
        {
            this.selectedRoadType = roadType;
        }

        public RoadType GetSelectedRoadType()
        {
            return this.selectedRoadType;
        }

        public void HandleButtonClick(Button button, RoadType roadType)
        {
            SetSelectedRoadType(roadType);
            RoadManager.Instance.CreateRoad();
        }
    }
}
