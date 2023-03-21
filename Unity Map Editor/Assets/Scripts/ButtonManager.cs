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
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        public void addSidebarButton(Button button) {
            sidebarButtonList.Add(button);
        }

        public void setSelectedRoadType(RoadType roadType)
        {
            this.selectedRoadType = roadType;
            Debug.Log(selectedRoadType);

        }

        public RoadType getSelectedRoadType()
        {
            return this.selectedRoadType;
        }

        public void colorSelectedButton(Button button)
        {
            ColorBlock colors = button.colors;
            colors.selectedColor = Color.cyan;
            button.colors = colors;
        }

        public void handleButtonClick(Button button, RoadType roadType)
        {
            setSelectedRoadType(roadType);
            colorSelectedButton(button);
        }
    }
}
