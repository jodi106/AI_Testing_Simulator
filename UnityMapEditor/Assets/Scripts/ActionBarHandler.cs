using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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




        // Start is called before the first frame update
        void Start()
        {
            LeftRotate.onClick.AddListener(RotateLeft);
            RightRotate.onClick.AddListener(RotateRight);
            Lock.onClick.AddListener(LockRoad);
            Delete.onClick.AddListener(DeleteRoad);

        }

        // Update is called once per frame
        void Update()
        {
            if (RoadManager.Instance.SelectedRoad == null && RoadManager.Instance.SelectedRoads == null)
            {
                ActionBar.GetComponent<CanvasGroup>().alpha = 0;
                ActionBar.GetComponent<CanvasGroup>().interactable = false;

            }
            else
            {
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
    }

}
