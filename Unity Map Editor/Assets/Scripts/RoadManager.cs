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

        public RoadPiece lastClickedRoad;

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
            if (Input.GetMouseButtonUp(0))
            {
                createRoad();
            }
            DoDragTile();
        }

        void createRoad()
        {
            var roadPiece = PrefabManager.Instance.GetPieceOfType(ButtonManager.Instance.getSelectedRoadType());
            var pos = new Vector3(0, 0, 1);
            var newRoadPiece = Instantiate(roadPiece, pos, Quaternion.identity);
            AddRoadToList(newRoadPiece);
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
                    Debug.Log($" Hit : {go.name}");
                    if (go.GetComponent<RoadPiece>() != null)
                    {
                        lastClickedRoad = go.GetComponent<RoadPiece>();
                        //lastClickedRoad.GetComponent<SpriteRenderer>().color = selectedColor;
                    }
                    else
                    {
                        Debug.Log($" Has no A Tile comp attached");
                    }
                }
            }

            if (lastClickedRoad != null)
            {
                lastClickedRoad.transform.position = GetWorldPositionFromMouseClick();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                lastClickedRoad.GetComponent<SpriteRenderer>().color = Color.white;
                lastClickedRoad = null;
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
