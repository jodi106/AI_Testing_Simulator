using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

namespace scripts
{
    public class ActionBarHandler : MonoBehaviour
    {
        public GameObject ActionBar;
        public Button LeftRotate;
        public Button RightRotate;
        public Button Lock;
        public Button Delete;

        public Image LockImage;

        public TMP_Dropdown SignDropdown; 
        public Toggle TrafficLight; 

        public RoadPiece SelectedRoadClone { get; set; } = null; 



        // Start is called before the first frame update
        void Start()
        {
            LeftRotate.onClick.AddListener(RotateLeft);
            RightRotate.onClick.AddListener(RotateRight);
            Lock.onClick.AddListener(LockRoad);
            Delete.onClick.AddListener(DeleteRoad);

            SignDropdown.onValueChanged.AddListener(SelectTrafficSign); 
           
             TrafficLight.isOn = false;
            TrafficLight.onValueChanged.AddListener(ToggleTrafficLight);
        }


        // Update is called once per frame
        void Update()
        { 
            SetVisibilityOfActionBar();
            
        }

        public void SetVisibilityOfActionBar()
        {
            if(SelectedRoadClone == null)
            {
                SelectedRoadClone = RoadManager.Instance.SelectedRoad; 
            }
            else
            {
                if(SelectedRoadClone != RoadManager.Instance.SelectedRoad)
                {

                }
            }
            if (RoadManager.Instance.SelectedRoad == null && RoadManager.Instance.SelectedRoads == null)
            {
                ActionBar.GetComponent<CanvasGroup>().alpha = 0;
                ActionBar.GetComponent<CanvasGroup>().interactable = false;
            }
            else
            { 
                if(RoadManager.Instance.SelectedRoad.RoadType != RoadType.StraightRoad){
                    SignDropdown.GetComponent<CanvasGroup>().alpha = 0; 
                    SignDropdown.GetComponent<CanvasGroup>().interactable = false;
                }else{
                    SignDropdown.GetComponent<CanvasGroup>().alpha = 1; 
                    SignDropdown.GetComponent<CanvasGroup>().interactable = true; 
                }
                if(RoadManager.Instance.SelectedRoad.RoadType == RoadType.FourWayIntersection || RoadManager.Instance.SelectedRoad.RoadType == RoadType.ThreeWayIntersection)
                {
                    TrafficLight.GetComponent<CanvasGroup>().alpha = 1; 
                    TrafficLight.GetComponent<CanvasGroup>().interactable = true;
                }
                else
                {
                    TrafficLight.GetComponent<CanvasGroup>().alpha = 0; 
                    TrafficLight.GetComponent<CanvasGroup>().interactable = false;
                }
                ActionBar.GetComponent<CanvasGroup>().alpha = 1;
                ActionBar.GetComponent<CanvasGroup>().interactable = true;

            }


            if (RoadManager.Instance.SelectedRoad?.IsLocked == true)
            {
                LockImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/lock-open");
            }
            else if (RoadManager.Instance.SelectedRoad?.IsLocked == false)
            {
                LockImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/lock");
            }

        }
        public void RotateLeft()
        {
            RoadManager.Instance.RotateClockwise();
        }
        public void RotateRight()
        {
            RoadManager.Instance.RotateCounterClockwise();
        }
        public void LockRoad()
        {
            bool locked = !RoadManager.Instance.SelectedRoad.IsLocked;
            if (RoadManager.Instance.SelectedRoads != null)
            {
                foreach (RoadPiece road in RoadManager.Instance.SelectedRoads)
                {
                    RoadManager.Instance.LockRoad(road, locked);
                }
            }
            else
            {
                RoadManager.Instance.LockRoad(RoadManager.Instance.SelectedRoad, locked);
            }
        }
        public void DeleteRoad()
        {
            RoadManager.Instance.DeleteRoad(RoadManager.Instance.SelectedRoad);
        }

        public void SelectTrafficSign(int value){
            switch(value){
                case 0: 
                     RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/StraightLong");
                     RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.None; 
                     break; 
                case 1:  
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/StopRoad");
                    RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.Stop; 
                    break;
                case 2: 
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/YieldRoad");
                    RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.Yield; 
                    break;
                case 3: 
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Limit30Road");
                    RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.Limit30; 
                    break;
                case 4: 
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Limit60Road");
                    RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.Limit60; 
                    break;
                case 5: 
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Limit90Road");
                    RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.Limit90;
                    break;
                default:
                    break;
            }
        }
    
        public void ToggleTrafficLight(bool On)
        {
            if (On)
            {
                if(RoadManager.Instance.SelectedRoad.RoadType == RoadType.ThreeWayIntersection)
                {
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/TrafficLight3Way");
                }
                else
                {
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/TrafficLight4Way");
                }
                RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.TrafficLight;
            }
            else
            {
                if(RoadManager.Instance.SelectedRoad.RoadType == RoadType.ThreeWayIntersection)
                {
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/3-Way Intersection");
                }
                else
                {
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/4-Way Intersection");
                }
                RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.None;
            }
            
        }
    }

}