using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace scripts
{
    public class SidebarButtonHandler : MonoBehaviour
    {
        public Button sidebarButton;

        void Start()
        {

            ButtonManager.Instance.addSidebarButton(sidebarButton); 
            Button btn = sidebarButton.GetComponent<Button>();
            //btn.onClick.AddListener(() => TaskOnClick(btnText));
            btn.onClick.AddListener(() => selectRoadType(btn.name));
        }

        private void Update()
        {
            
        }

        void selectRoadType(string buttonName)
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
                default:
                    break;
            }
            ButtonManager.Instance.handleButtonClick(sidebarButton, selectedRoadType);

        }

        public void TaskOnClick(string btnText)
        {
            //Output this to console when Button1 or Button3 is clicked
            print("You have selected the '" + btnText + "' Tile!");
        }
    }


}