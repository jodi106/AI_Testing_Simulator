using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

namespace scripts
{
    public class RoadManager : MonoBehaviour
    {

        private static RoadManager instance;
        public static RoadManager Instance
        {
            get
            {
                return instance;
            }
        }

        public List<RoadPiece> roadList = new();

        public RoadPiece selectedRoad = null;

        private bool isDragging = false;

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
            DoDragTile();

            if(Input.GetMouseButtonDown(1) && selectedRoad != null && isDragging)
            {
                isDragging = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape) && selectedRoad != null)
            {
                DeselectRoad(); 
            }
        }

        void createRoad()
        {
            var roadPiece = PrefabManager.Instance.GetPieceOfType(ButtonManager.Instance.getSelectedRoadType());
            var newRoadPiece = Instantiate(roadPiece, GetWorldPositionFromMouseClick(), Quaternion.identity);
            AddRoadToList(newRoadPiece);
            SelectRoad(newRoadPiece);
        }

        public void AddRoadToList(RoadPiece road)
        {
            roadList.Add(road);
        }

        private void DoDragTile()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var go = GetClickedObject();
                if (go != null)
                {
                    if (go.GetComponent<RoadPiece>() != null)
                    {
                        DeselectRoad();
                        SelectRoad(go.GetComponent<RoadPiece>());
                        isDragging = true; 
                    }
                }

                else if (selectedRoad == null)
                {
                    createRoad();
                }
            }

            if (selectedRoad != null && isDragging)

            {
                selectedRoad.transform.position = GetWorldPositionFromMouseClick();
                
            }
        }

        public void SelectRoad(RoadPiece road)
        {
            if(selectedRoad == null)
            {
                selectedRoad = road; 
                selectedRoad.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
        }
        public void DeselectRoad()
        {
           if(selectedRoad != null)
            {
                selectedRoad.GetComponent<SpriteRenderer>().color = Color.white;
                selectedRoad = null;
                isDragging = false;
            }            
        }

        public void rotateRoadPiece()
        {
            
        }

        private GameObject GetClickedObject()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }

            return null;
        }

        private Vector3 GetWorldPositionFromMouseClick()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.nearClipPlane;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            return worldPosition;
        } 
    }
      
}
