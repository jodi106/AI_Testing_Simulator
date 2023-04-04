using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Linq;
using UnityEngine.UIElements;
using Entity;

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

        // List of all roads
        public List<RoadPiece> roadList = new();

        // selected Road
        public RoadPiece selectedRoad = null;

        // selected Object, not necessarily a road
        GameObject selectedObject;

        //boolean for road validation. If false, certain functions are disabled
        public bool inValidPosition = true;

        // used to check whether the user is currently dragging. So no other object can be selected during a drag. 
        private bool isDragging = false;

        /*
         * Sets the instance, so other classes can refer
         */
        private void Awake()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            // This condition checks, whether a tile is being dragged. 
            if (Input.GetMouseButton(0))
            {
                DragAndDropRoad();
            }

            // This condition checks, when the user releases the drag and checks the road position
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                validateRoadPosition();
            }

            // These conditions checks, whether the user wants to rotate the piece clockwise or counter-clockwise
            if (Input.GetKeyDown(KeyCode.E))
            {
                rotateRoadPiece(-90);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                rotateRoadPiece(90);
            }

            // This condition checks, whether the user wants to lock a road piece. This can only be applied, when a road is selected. 
            if (Input.GetKeyDown(KeyCode.L) && selectedRoad != null)
            {
                selectedRoad.setIsLocked(!selectedRoad.getIsLocked());
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteRoad();
            }

            // This condition checks, whether the user wants to deselect the road he has clicked. 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DeselectRoad();
            }
        }

        /*
         * This method will add every created road piece to the list of roads. This way, snapping can be implemented because the positions of roads can be determined. 
         */
        public void AddRoadToList(RoadPiece road)
        {
            roadList.Add(road);
        }

        /*
         * Removes a road from the Roads List
         */
        public void RemoveRoadFromList(RoadPiece road)
        {
            roadList.Remove(road);
        }
        /* 
         * This method creates a new road piece. It gets the selected road type from the sidebar and creates the corresponding prefab on the screen. 
         */
        public void createRoad()
        {
            // Creates the new roadpiece
            var roadPiece = PrefabManager.Instance.GetPieceOfType(ButtonManager.Instance.getSelectedRoadType());
            var newRoadPiece = Instantiate(roadPiece, GetWorldPositionFromMouseClick(), Quaternion.identity);

            // Sets the valid position to false (As it will always spawn on the sidebar), adds the road to the list of roads and selects the road automatically upon creation, allowing the user to instantly drag it)
            inValidPosition = false;
            SelectRoad(newRoadPiece);
        }

        /*
         * This method deletes the selected road
         */
        public void DeleteRoad()
        {
            if (selectedRoad != null && !selectedRoad.getIsLocked())
            {
                // We have to destroy the selectedObject, as the selectedRoad is not a GameObject, but a RoadPiece. The effect will be the same, though. 
                RemoveRoadFromList(selectedRoad);
                DeselectRoad();
                Destroy(selectedObject);

                // To be implemented, as soon as references are implemented. VERY IMPORTANT!
                // RemoveReferences();
            }
        }

        /*
         * This method drags a road across the screen. When the user drags a piece, it will follow the cursor of the mouse. This is only the case, if the road piece is not locked
         */
        private void DragAndDropRoad()
        {
            // This will only check the object at the start of dragging and not check every frame. Prevents the user from going too fast and "Losing" the road. 
            if (isDragging == false)
            {
                selectedObject = GetClickedObject();
            }

            // This will, if a road is selected, set dragging to true, deselect previous roads and select the new road. The road is then dragged with the mouse
            if (selectedObject != null)
            {
                if (selectedObject.GetComponent<RoadPiece>() != null)
                {
                    DeselectRoad();
                    SelectRoad(selectedObject.GetComponent<RoadPiece>());

                    if (!selectedRoad.getIsLocked())
                    {
                        isDragging = true;
                        selectedRoad.transform.position = GetWorldPositionFromMouseClick();
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            SnapRoadPiece();
                        }
                        validateRoadPosition();
                    }

                }
            }
        }


        public RoadPiece GetNearestNeighbor()
        {
            float nearestDistance = -1;
            RoadPiece closestRoad = null;
            if(roadList.Count > 1)
            {
                foreach (RoadPiece road in roadList)
                {
                    if ((nearestDistance == -1 || Vector3.Distance(selectedRoad.transform.position, road.transform.position) < nearestDistance) && road != selectedRoad)
                    {
                        nearestDistance = Vector3.Distance(selectedRoad.transform.position, road.transform.position);
                        if (nearestDistance < 300.00) {
                            closestRoad = road;
                        }
                        else
                        {
                            closestRoad = null;
                        }
                       
                        
                    }
                }
            }

            return closestRoad;
        }
       
        /*
         * Method that will set position of selected road piece according to the nearest road piece. 
         */
        public void SnapRoadPiece()
        {
            RoadPiece nearestNeighbor = GetNearestNeighbor();
            if (nearestNeighbor != null)
            {
                float distanceX = nearestNeighbor.transform.position.x - selectedRoad.transform.position.x;
                float distanceY = nearestNeighbor.transform.position.y - selectedRoad.transform.position.y;

                // Snapping to left or right
                if (Math.Abs(distanceX) > Math.Abs(distanceY))
                {
                    // Snapping to left
                    if (distanceX > 0)
                    {
                        selectedRoad.transform.position = new Vector3(nearestNeighbor.transform.position.x - nearestNeighbor.GetComponent<Renderer>().bounds.size.x / 2 - selectedObject.GetComponent<Renderer>().bounds.size.x / 2, nearestNeighbor.transform.position.y, 0);
                    }
                    // Snapping to right
                    else
                    {
                        selectedRoad.transform.position = new Vector3(nearestNeighbor.transform.position.x + nearestNeighbor.GetComponent<Renderer>().bounds.size.x / 2 + selectedObject.GetComponent<Renderer>().bounds.size.x / 2, nearestNeighbor.transform.position.y, 0);
                    }
                }
                // Snapping to top or bottom
                else
                {
                    // Snapping to top
                    if (distanceY > 0)
                    {
                        selectedRoad.transform.position = new Vector3(nearestNeighbor.transform.position.x, nearestNeighbor.transform.position.y - nearestNeighbor.GetComponent<Renderer>().bounds.size.y / 2 - selectedObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
                    }
                    // Snapping to bottom
                    else
                    {
                        selectedRoad.transform.position = new Vector3(nearestNeighbor.transform.position.x, nearestNeighbor.transform.position.y + nearestNeighbor.GetComponent<Renderer>().bounds.size.y / 2 + selectedObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
                    }
                }
            }
        }

        /*
         * This sets the selected road and will change the color of the piece accordingly. Also, it checks the current Road Position. 
         */
        public void SelectRoad(RoadPiece road)
        {
            if (selectedRoad == null)
            {
                selectedRoad = road;
                validateRoadPosition();
                colorRoadPiece(selectedRoad.getIsLocked() ? SelectionColor.lockedSelected : SelectionColor.selected);
            }
        }

        /*
         * This deselect any road currently selected and changes the color accordingly. 
         */
        public void DeselectRoad()
        {
            if (selectedRoad != null)
            {
                colorRoadPiece(selectedRoad.getIsLocked() ? SelectionColor.locked : SelectionColor.normal);
                selectedRoad = null;
                isDragging = false;
            }
        }

        /*
         * This rotates the selected roadpiece by a given rotation. Only if the piece is not locked
         */
        private void rotateRoadPiece(int rotation)
        {
            if (selectedRoad != null && !selectedRoad.getIsLocked())
            {
                selectedRoad.transform.Rotate(new Vector3(0, 0, rotation));
            }
        }


        /*
         * This will get the clicked object on the screen. Currently only works for GameObjects. 
         */
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

        /*
         * This translates px into world position, so the user can only click inside the camera frame
         */
        private Vector3 GetWorldPositionFromMouseClick()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.nearClipPlane;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            return worldPosition;
        }

        /*
         * This method validates the road position. Currently always true, as not implemented yet. 
         */
        private void validateRoadPosition()
        {
            inValidPosition = true;
        }

        /*
         * This will color the road piece dependend on the status the road currently has. 
         */
        public void colorRoadPiece(SelectionColor sColor)
        {
            Color color = new Color();
            switch (sColor)
            {
                case SelectionColor.normal:
                    color = Color.white;
                    break;
                case SelectionColor.selected:
                    color = new Color(0.49f, 0.85f, 1f, 1f);
                    break;
                case SelectionColor.invalid:
                    color = new Color(1f, 0.67f, 0.72f, 1f);
                    break;
                case SelectionColor.snapped:
                    color = new Color(0.72f, 1f, 0.65f, 1f);
                    break;
                case SelectionColor.locked:
                    color = new Color(1f, 1f, 1f, 0.5f);
                    break;
                case SelectionColor.lockedSelected:
                    color = new Color(0f, 0.707f, 1f, 1f);
                    break;
                case SelectionColor.lockedSnapped:
                    color = new Color(0f, 0.7f, 0f, 1f);
                    break;
                default:
                    break;
            }

            selectedRoad.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
