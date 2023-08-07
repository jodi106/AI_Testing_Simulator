using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Enums;
using System.Diagnostics;

namespace scripts
{
    /// <summary>
    /// This class handles all actions performed by the user on the sidebar. This includes rotating, locking, deleting and adding traffic signs and lights
    /// </summary>
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

        /// <summary>
        /// This method is called before the first frame update and will initialize all buttons and input fields. 
        /// </summary>
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


        /// <summary>
        /// This method is called every frame and will call a method for setting the visibility of the sidebar and a method to preset their values
        /// </summary>
        void Update()
        {
            SetVisibilityOfActionBar();
            SetPreselectedValues();
        }



        /// <summary> 
        /// This method will toggle the visibility of the Action bar. When a road piece is selected, then the sidebar is visible. If a straight road is selected,  
        /// it will show a dropdown to select a road sign in addition. If an intersection is selected, then the sidebar will additionally show a checkbox to activate 
        /// traffic lights
        /// </summary>
        public void SetVisibilityOfActionBar()
        {
            if (ScrollViewOpener.IsUserGuideOpen())
            {
                ActionBar.GetComponent<CanvasGroup>().interactable = false;
            }
            else if (RoadManager.Instance.SelectedRoad == null && RoadManager.Instance.SelectedRoads == null)
            {
                ActionBar.GetComponent<CanvasGroup>().alpha = 0;
                ActionBar.GetComponent<CanvasGroup>().interactable = false;
            }
            else
            {
                if (RoadManager.Instance.SelectedRoad.RoadType != RoadType.StraightRoad)
                {
                    SignDropdown.GetComponent<CanvasGroup>().alpha = 0;
                    SignDropdown.GetComponent<CanvasGroup>().interactable = false;
                }
                else
                {
                    SignDropdown.GetComponent<CanvasGroup>().alpha = 1;
                    SignDropdown.GetComponent<CanvasGroup>().interactable = true;
                }
                if (RoadManager.Instance.SelectedRoad.RoadType == RoadType.FourWayIntersection || RoadManager.Instance.SelectedRoad.RoadType == RoadType.ThreeWayIntersection)
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

        /// <summary>
        /// This method will preset the values of the sidebar, based on the state of the road piece
        /// </summary>
        public void SetPreselectedValues()
        {
            if (RoadManager.Instance.SelectedRoad != null)
            {
                int value = 0;
                switch (RoadManager.Instance.SelectedRoad.TrafficSign)
                {
                    case TrafficSign.None:
                        value = 0;
                        break;
                    case TrafficSign.Stop:
                        value = 1;
                        break;
                    case TrafficSign.Yield:
                        value = 2;
                        break;
                    case TrafficSign.Limit30:
                        value = 3;
                        break;
                    case TrafficSign.Limit60:
                        value = 4;
                        break;
                    case TrafficSign.Limit90:
                        value = 5;
                        break;
                    default:
                        break;
                }
                SignDropdown.value = value;

                if (RoadManager.Instance.SelectedRoad.TrafficSign == TrafficSign.TrafficLight)
                {
                    TrafficLight.isOn = true;
                }
                else
                {
                    TrafficLight.isOn = false;
                }
            }
        }

        /// <summary>
        /// This will call the rotate method in Road Manager to rotate the piece counter-clockwise
        /// </summary>
        public void RotateLeft()
        {
            RoadManager.Instance.RotateCounterClockwise();
        }

        /// <summary>
        /// This will call the rotate method in Road Manager to rotate the piece clockwise
        /// </summary>
        public void RotateRight()
        {
            RoadManager.Instance.RotateClockwise();
        }

        /// <summary>
        /// This will call the lock method in Road Manager to lock or unlock a road piece or group
        /// </summary>
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

        /// <summary>
        /// This will call the delete method in Road Manager to delete a road or group
        /// </summary>
        public void DeleteRoad()
        {
            RoadManager.Instance.DeleteRoad(RoadManager.Instance.SelectedRoad);
        }

        /// <summary>
        /// This method selects the traffic sign that the user has selected in the dropdown menu from the sidebar
        /// </summary>
        /// <param name="value"> the selected value in the dropdown menu </param>
        public void SelectTrafficSign(int value)
        {
            if (RoadManager.Instance.SelectedRoad.RoadType == RoadType.StraightRoad)
            {

                if (RoadManager.Instance.SelectedRoad.transform.childCount < 1)
                {
                    switch (value)
                    {
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
                else
                {
                    GameObject go = RoadManager.Instance.SelectedRoad.transform.GetChild(RoadManager.Instance.SelectedRoad.transform.childCount - 1).gameObject;

                    switch (value)
                    {
                        case 0:
                            go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/StraightShort");
                            RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.None;
                            break;
                        case 1:
                            go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/StopRoadShort");
                            RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.Stop;
                            break;
                        case 2:
                            go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/YieldRoadShort");
                            RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.Yield;
                            break;
                        case 3:
                            go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Limit30RoadShort");
                            RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.Limit30;
                            break;
                        case 4:
                            go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Limit60RoadShort");
                            RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.Limit60;
                            break;
                        case 5:
                            go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Limit90RoadShort");
                            RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.Limit90;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// This method will toggle the traffic light on intersections
        /// </summary>
        /// <param name="On"> the value of the checkbox - true, if traffic light selected</param>
        public void ToggleTrafficLight(bool On)
        {
            if (On)
            {
                if (RoadManager.Instance.SelectedRoad.RoadType == RoadType.ThreeWayIntersection)
                {
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/TrafficLight3Way");
                    RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.TrafficLight;
                }
                else if (RoadManager.Instance.SelectedRoad.RoadType == RoadType.FourWayIntersection)
                {
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/TrafficLight4Way");
                    RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.TrafficLight;
                }

            }
            else
            {
                if (RoadManager.Instance.SelectedRoad.RoadType == RoadType.ThreeWayIntersection)
                {
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/3-Way Intersection");
                    RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.None;
                }
                else if (RoadManager.Instance.SelectedRoad.RoadType == RoadType.FourWayIntersection)
                {
                    RoadManager.Instance.SelectedRoad.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/4-Way Intersection");
                    RoadManager.Instance.SelectedRoad.TrafficSign = TrafficSign.None;

                }
            }

        }
    }

}