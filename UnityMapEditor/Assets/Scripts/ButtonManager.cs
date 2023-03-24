using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace scripts
{
    public class ButtonManager : MonoBehaviour
    {

        public List<Button> sidebarButtonList = new List<Button>();

        private static ButtonManager instance; 
        public static ButtonManager Instance { get
            {
                return instance; 
            } 
        }
        private RoadType selectedRoadType; 

        private void Awake()
        {
            instance = this; 
        }

        public void addSidebarButton(Button button) {
            sidebarButtonList.Add(button);
        }

        public void setSelectedRoadType(RoadType roadType)
        {
            this.selectedRoadType = roadType;
        }

        public RoadType getSelectedRoadType()
        {
            return this.selectedRoadType;
        }

        public void handleButtonClick(Button button, RoadType roadType)
        {
            setSelectedRoadType(roadType);
            RoadManager.Instance.createRoad(); 
        }
    }
}
