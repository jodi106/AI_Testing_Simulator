
using Assets.Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace scripts
{
    /* 
    * This class is the heart of the program. It contains all roads created, selected RoadPieces and operations for rotating and snapping. 
    * It manages all RoadPieces and their interactions with other pieces and the Unity Editor. 
    */
    /// <summary>
    /// 
    /// </summary>
    public class RoadManager : MonoBehaviour
    {

        // This is a static instance of this class, so other classes can access its methods and properties, such as adding a roadPiece to the List of roads
        private static RoadManager instance;
        public static RoadManager Instance
        {
            get
            {
                return instance;
            }
        }

        // Enumeration of all Side and Bottom Panels
        public GameObject Sidebar;
        public GameObject Bottombar;

        // This List is a List of all roads currently created in the Editor. 
        public List<RoadPiece> RoadList = new List<RoadPiece>();

        // This represents the GameObject of the SelectedRoad, which is retrieved when clicking on the screen
        public GameObject SelectedObject { get; set; }
        // This property describes the currently selected road
        public RoadPiece SelectedRoad { get; set; } = null;

        // This List describes the currently selected roads in case of group selection of roads. 
        public List<RoadPiece> SelectedRoads { get; set; } = null;
        // This property describes the initial position of a group in case of group selection and dragging.  
        public Vector3 InitialPositionOfGroup { get; set; } = new Vector3();
        // This List describes the roads that have been chosen by the user to deselect them from the selected group
        public List<RoadPiece> CtrlDeselectedRoads { get; set; } = new List<RoadPiece>();

        // This boolean is used to check whether the user is currently dragging. So no other object can be selected during a drag. 
        private bool IsDragging { get; set; } = false;

        // This boolean indicates, that a new road has been created
        private bool NewRoadCreated { get; set; } = false;

        // This boolean checks, whether the selected road is snapped to another piece
        private bool IsSnapped { get; set; } = false;
        // This boolean checks, whether the selected roads, so the selected group of roads, are snapped
        public bool IsSnappedGroup { get; set; } = false;

        // This property saves the currently selected AnchorPoints when stretching for attaching the stretched roads to this AnchorPoint
        private VirtualAnchor StretchingAnchor { get; set; }
        // This boolean checks, whether the cursor of the user is in the area to stretch a RoadPiece. 
        private bool IsInStretchingArea { get; set; } = false;
        // This property describes the distance at which new roads should be added when stretching
        private float StretchingDistance { get; set; } = 3.78f * 5;
        // This boolean checks, whether the user is currently stretching a RoadPiece, which disabled some functionality. 
        private bool IsStretching { get; set; } = false;

        // This constant describes the distance at which RoadPieces should snap to each other
        private const float SNAPPING_DISTANCE = 200;
        // This contant describes the angle at which RoadPieces should be rotated on user input
        private const float ROTATING_ANGLE = 15f;

        /* 
        * The Awake method is a "Monobehavior" method from Unity, which is automaticlly called when instantiated. 
        * This method initialize an instance for other classes to gain access to its properties and methods
        */
        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// The Update method is a "Monobehavior" method from Unity, which is automaticlly called every frame. 
        ///  This method will constantly check the user inputs from mouse and keyboard to check whether the user is interacting with a Road Piece
        /// </summary>
        void Update()
        {
            if (!ScrollViewOpener.IsUserGuideOpen())
            {
                // This condition checks, whether the user has selected a road to then check the stretching position
                if (SelectedRoad != null)
                {
                    CheckStretchPosition();
                }

                // This condition checks, whether the User has pressed the Left Mouse Button (Also holding it) and has not pressed the Left Control Button 
                if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift))
                {
                    // This condition checks, whether the user has pressed Left Shift (while pressing the LMB from the previous condition)
                    if (!Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        // This conditions checks, whether the user is NOT dragging, is in the Stretching Area and there is no group selection
                        if (!IsDragging && IsInStretchingArea && SelectedRoads == null)
                        {
                            // If that is the case, dragging the mouse will stretch the road
                            StretchRoad();
                        }
                        // Else, it will check whether a group is selected
                        else if (SelectedRoads != null)
                        {
                            // if that is the case, dragging the mouse will move a group of roads
                            DragAndDropRoads();
                        }
                        // Else
                        else
                        {
                            // Dragging the mouse will move the selected road. 
                            DragAndDropRoad();
                        }
                    }
                }
                if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift))
                {
                    // If that is the case, then the clicked road will be retrieved. 
                    RoadPiece clickedRoad = GetMouseObject()?.GetComponent<RoadPiece>();
                    // This condition checks, whether the user has actually clicked a RoadPiece 
                    if (clickedRoad != null)
                    {
                        // If that is the case, then all currently selected things are deselected, the selected road is the new SelectedRoad
                        // and the group of roads is selected
                        DeselectRoad();
                        DeselectGroup();
                        SelectRoad(clickedRoad);
                        SelectGroupOfRoads(clickedRoad);
                    }
                }


                // This condition checks, whether the user has clicked the Left Mouse Button (no holding) and has simultaneaously pressed the Left CTRL button
                if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
                {
                    // If that is the case, then the selected RoadPiece is deselected from the group
                    if (SelectedRoads != null)
                    {
                        ControlSelectRoadPiece();
                    }
                }

                // This condition checks, whether the user released the mouse click
                if (Input.GetMouseButtonUp(0))
                {
                    // If that is the case, the dragging stops, the stretching stops, the stretching distance is reset and the position of a roadPiec
                    // is validated
                    IsDragging = false;

                    NewRoadCreated = false;

                    //This conditions checks, whether the user has been stretching a road
                    if (IsStretching)
                    {
                        // if that is the case, then a new custom road is created
                        CreateCustomStraightRoad();
                        Snap();
                    }
                    IsStretching = false;
                    StretchingDistance = 3.78f * 5;
                }

                // These conditions checks, whether the user presses the "E" Key
                if (Input.GetKeyDown(KeyCode.E))
                {
                    RotateClockwise();
                }


                if (Input.GetKeyDown(KeyCode.Q))
                {
                    RotateCounterClockwise();
                }

                // This condition checks, whether the user wants to lock a road piece. This can only be applied, when a road is selected. 
                if (Input.GetKeyDown(KeyCode.L) && SelectedRoad != null)
                {
                    if (SelectedRoads == null)
                    {
                        LockRoad(SelectedRoad, !SelectedRoad.IsLocked);
                    }
                    else
                    {

                        bool locked = !SelectedRoad.IsLocked;
                        foreach (RoadPiece road in SelectedRoads)
                        {
                            LockRoad(road, locked);

                        }
                        ColorRoadPiece(SelectedRoad, SelectionColor.selected);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    if (SelectedRoads == null)
                    {
                        DeleteRoad(this.SelectedRoad);
                    }
                    else
                    {
                        foreach (RoadPiece road in SelectedRoads)
                        {
                            DeleteRoad(road);
                        }
                        SelectedRoads = null;
                    }
                }

                // This condition checks, whether the user wants to deselect the road he has clicked. 
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    DeselectRoad();
                    DeselectGroup();
                }
            }
        }

        /// <summary>
        /// This method will rotate the selected road or the selected group clockwise. Either by 15° or to the next available anchor point
        /// </summary>
        public void RotateClockwise()
        {
            // this condition checks, whether the selected road is snapped and no group has been selected
            if (IsSnapped && SelectedRoads == null)
            {
                // If that is the case, we first find the current index of our anchor in the list of anchor points of the selected road
                int currentIndex = SelectedRoad.AnchorPoints.FindIndex(anchor => anchor == SelectedRoad.LastSelectedSnappedAnchorPoint);
                // if our anchor Point is the first anchor point in the list, we set the currentIndex to the count of the list to 
                // retrieve the previous anchor point, which is the last
                if (currentIndex == SelectedRoad.AnchorPoints.Count - 1)
                {
                    currentIndex = -1;
                }
                VirtualAnchor nextAnchor = SelectedRoad.AnchorPoints[currentIndex + 1];

                // We then compare the orientation between the new anchor point and the anchor point we want to snap to and rotate the piece
                CompareAnchorPointOrientation(SelectedRoad.LastNeighborSnappedAnchorPoint, nextAnchor);
                foreach (VirtualAnchor va in SelectedRoad.AnchorPoints)
                {
                    // For each anchor point of our selcted road, we remove all connections they have to other anchor points
                    // (as through rotation, there are no connections anymore)
                    va.RemoveConntectedAnchorPoint();
                }
                // Our Last selected snapped anchor point is now our new anchor point, so we have a new index, if we rotate again
                SelectedRoad.LastSelectedSnappedAnchorPoint = nextAnchor;

                //At last we snap the piece to all roadpiece in the area to connect the Anchor Points back to other anchor points
                Snap();
            }
            // Else, it will check, if we have a group selected and the group is snapped to another road piece
            else if (SelectedRoads != null && IsSnappedGroup == true)
            {
                // if that is the case, we again retrieve the index of our currently connected anchorPoint. 
                int currentIndex = SelectedRoad.AnchorPoints.FindIndex(anchor => anchor == SelectedRoad.LastSelectedSnappedAnchorPoint);
                VirtualAnchor nextAnchor = null;

                //For every AnchorPoint we check, whether we have found a candidate to rotate to 
                for (int i = 1; i < SelectedRoad.AnchorPoints.Count && nextAnchor == null; i++)
                {
                    // As we rotate an entire group, we only want to rotate to anchor Points which are not connected to other roads of our group
                    VirtualAnchor candidate = SelectedRoad.AnchorPoints[(currentIndex + i) % SelectedRoad.AnchorPoints.Count];
                    // This condition will check, whether the connected Anchor Point of the candidate in in the group
                    // or if it has no connection
                    if (!SelectedRoads.Contains(candidate.ConnectedAnchorPoint?.RoadPiece) || candidate.ConnectedAnchorPoint == null)
                    {
                        // If this is the case, then the next anchor point is the next anchor we rotate to
                        nextAnchor = SelectedRoad.AnchorPoints[(currentIndex + i) % SelectedRoad.AnchorPoints.Count];
                    }
                }
                // This condition checks, whether there is a next anchor we can snap to when rotating (Not the case, when all anchor points are 
                // connected to roads of the group)
                if (nextAnchor != null)
                {
                    // If that is the case, then we compare the rotations again and save the rotation value
                    float rotation = CompareAnchorPointOrientation(SelectedRoad.LastNeighborSnappedAnchorPoint, nextAnchor);

                    foreach (VirtualAnchor va in SelectedRoad.AnchorPoints)
                    {
                        // For each Virtual Anchor in our Selected Road, we check if the connected virtual anchor is NOT in the group
                        if (!SelectedRoads.Contains(va.ConnectedAnchorPoint?.RoadPiece))
                        {
                            // if that is the case, we remove the connection to that anchor point
                            va.RemoveConntectedAnchorPoint();
                        }
                    }

                    // Our Last selected snapped anchor point is now our new anchor point, so we have a new index, if we rotate again
                    SelectedRoad.LastSelectedSnappedAnchorPoint = nextAnchor;

                    // we then align our group pieces to our selected road, so they rotate with the selected road. 
                    AlignGroupPiecesToEachOther(rotation);
                }
            }
            else
            {
                RotateRoadPiece(-ROTATING_ANGLE, true);
            }
        }

        /// <summary>
        ///  This method will rotate the selected road or the selected group counter-clockwise. Either by 15° or to the next available anchor point
        /// </summary>
        public void RotateCounterClockwise()
        {
            if (IsSnapped && SelectedRoads == null)
            {
                int currentIndex = SelectedRoad.AnchorPoints.FindIndex(anchor => anchor == SelectedRoad.LastSelectedSnappedAnchorPoint);
                if (currentIndex == 0)
                {
                    currentIndex = SelectedRoad.AnchorPoints.Count;
                }
                VirtualAnchor nextAnchor = SelectedRoad.AnchorPoints[(currentIndex - 1)];
                CompareAnchorPointOrientation(SelectedRoad.LastNeighborSnappedAnchorPoint, nextAnchor);
                foreach (VirtualAnchor va in SelectedRoad.AnchorPoints)
                {
                    va.RemoveConntectedAnchorPoint();
                }
                //SelectedRoad.LastSelectedSnappedAnchorPoint.RemoveConntectedAnchorPoint();
                SelectedRoad.LastSelectedSnappedAnchorPoint = nextAnchor;
                Snap();
            }
            else if (SelectedRoads != null && IsSnappedGroup == true)
            {
                int currentIndex = SelectedRoad.AnchorPoints.FindIndex(anchor => anchor == SelectedRoad.LastSelectedSnappedAnchorPoint);
                VirtualAnchor nextAnchor = null;
                for (int i = 1; i < SelectedRoad.AnchorPoints.Count && nextAnchor == null; i++)
                {
                    VirtualAnchor candidate = SelectedRoad.AnchorPoints[(currentIndex + i) % SelectedRoad.AnchorPoints.Count];
                    if (!SelectedRoads.Contains(candidate.ConnectedAnchorPoint?.RoadPiece) || candidate.ConnectedAnchorPoint == null)
                    {
                        nextAnchor = SelectedRoad.AnchorPoints[(currentIndex + i) % SelectedRoad.AnchorPoints.Count];
                    }
                }
                if (nextAnchor != null)
                {
                    float rotation = CompareAnchorPointOrientation(SelectedRoad.LastNeighborSnappedAnchorPoint, nextAnchor);
                    foreach (VirtualAnchor va in SelectedRoad.AnchorPoints)
                    {
                        if (!SelectedRoads.Contains(va.ConnectedAnchorPoint?.RoadPiece))
                        {
                            va.RemoveConntectedAnchorPoint();
                        }
                    }
                    SelectedRoad.LastSelectedSnappedAnchorPoint = nextAnchor;
                    AlignGroupPiecesToEachOther(rotation);
                }
            }
            else
            {
                RotateRoadPiece(ROTATING_ANGLE, true);
            }
        }


        /// <summary>
        /// This method will either add or remove a road piece from a group, when the user CTRL + clicked a road piece
        /// </summary>
        public void ControlSelectRoadPiece()
        {
            RoadPiece road = GetMouseObject()?.GetComponent<RoadPiece>();
            if (road != null && SelectedRoads.Contains(road))
            {
                CtrlDeselectedRoads.Add(road);
                DeselectGroup();
                ColorRoadPiece(SelectedRoad, SelectionColor.selected);
                SelectGroupOfRoads(SelectedRoad);
                //SelectedRoads.Remove(road);
                //ColorRoadPiece(road, SelectionColor.normal);
            }
            else if (road != null && !SelectedRoads.Contains(road))
            {
                bool roadIsConnectedToTheGroup = false;
                foreach (VirtualAnchor va in road.AnchorPoints)
                {
                    if (va.ConnectedAnchorPoint != null)
                    {
                        if (SelectedRoads.Contains(va.ConnectedAnchorPoint.RoadPiece))
                        {
                            roadIsConnectedToTheGroup = true;
                        }
                    }
                }
                if (roadIsConnectedToTheGroup)
                {
                    SelectedRoads.Add(road);
                    if (CtrlDeselectedRoads.Contains(road))
                    {
                        CtrlDeselectedRoads.Remove(road);
                    }
                    ColorRoadPiece(road, SelectionColor.groupSelected);
                }
            }
        }

        /// <summary>
        /// This method will deselect the selected group, if there has been a group selected
        /// </summary>
        public void DeselectGroup()
        {
            if (SelectedRoads != null)
            {
                foreach (RoadPiece road in SelectedRoads)
                {
                    ColorRoadPiece(road, SelectionColor.normal);
                }
            }
            SelectedRoads = null;
        }

        /// <summary>
        /// This method will select all roads with are connected to the selected group and group them
        /// </summary>
        /// <param name="clickedRoad"> clicked Road is the road that has been selcted for grouping from </param>
        /// <returns> </returns>
        public List<RoadPiece> SelectGroupOfRoads(RoadPiece clickedRoad)
        {
            List<RoadPiece> connectedRoadPieces = new List<RoadPiece>();
            connectedRoadPieces.Add(clickedRoad);
            foreach (VirtualAnchor va in clickedRoad.AnchorPoints)
            {
                if (va.ConnectedAnchorPoint != null)
                {
                    AddRoadToGroup(connectedRoadPieces, va.ConnectedAnchorPoint.RoadPiece);
                }
            }
            SelectedRoads = connectedRoadPieces;
            return null;
        }

        /// <summary>
        /// This method will add a road to the group if the road has not been added yet. Also, it will add all roads that are connected to it
        /// </summary>
        /// <param name="roads"> the list of roads in which the roads of the group are saved. </param>
        /// <param name="roadToCheck"> The road piece that is supposed to be added </param>
        /// <returns> </returns>
        public void AddRoadToGroup(List<RoadPiece> roads, RoadPiece roadToCheck)
        {
            if (!roads.Contains(roadToCheck) && !CtrlDeselectedRoads.Contains(roadToCheck))
            {
                ColorRoadPiece(roadToCheck, SelectionColor.groupSelected);
                roads.Add(roadToCheck);
                foreach (VirtualAnchor va in roadToCheck.AnchorPoints)
                {
                    if (va.ConnectedAnchorPoint != null)
                    {
                        AddRoadToGroup(roads, va.ConnectedAnchorPoint.RoadPiece);
                    }
                }
            }
        }

        /// <summary>
        /// This method checks, whether the mouse of the user is hovering over an anchor point of a piece which indicated, that the user wants to stretch the road piece
        /// </summary>
        public void CheckStretchPosition()
        {
            GameObject obj = GetMouseObject();
            RoadPiece road = obj?.GetComponent<RoadPiece>();
            if (SelectedRoads == null && SelectedRoad.IsLocked == false && SelectedRoad != null && !IsDragging && !NewRoadCreated)
            {

                if (obj != null && road != null && road == SelectedRoad)
                {
                    foreach (VirtualAnchor anchor in SelectedRoad.AnchorPoints)
                    {
                        if (anchor.ConnectedAnchorPoint == null)
                        {
                            if (!IsStretching)
                            {

                                if (Vector3.Distance(GetWorldPositionFromMouse(), SelectedRoad.transform.position + anchor.Offset) < 20)
                                {
                                    //UnityEngine.Cursor.SetCursor(cursorStretchTexture, Vector2.zero, CursorMode.Auto);
                                    ColorRoadPiece(SelectedRoad, SelectionColor.invalid);
                                    IsInStretchingArea = true;
                                    StretchingAnchor = anchor;
                                    return;
                                }
                                else
                                {
                                    ColorRoadPiece(SelectedRoad, SelectionColor.selected);
                                    IsInStretchingArea = false;
                                    StretchingAnchor = null;
                                    //UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                                }
                            }
                        }
                    }

                }
                else if (!IsStretching)
                {
                    ColorRoadPiece(SelectedRoad, SelectionColor.selected);
                    IsInStretchingArea = false;
                    StretchingAnchor = null;
                    //UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                }
            }
        }

        /// <summary>
        /// This method will create a straight road from the stretched roads that have been added to a road piece after stretching it. 
        /// </summary>
        public void CreateCustomStraightRoad()
        {
            if (StretchingAnchor.GetStretchingStraights().Count > 0)
            {
                float newXPosition = (StretchingAnchor.GetStretchingStraights()[0].transform.position.x + StretchingAnchor.GetStretchingStraights()[StretchingAnchor.GetStretchingStraights().Count - 1].transform.position.x) / 2;
                float newYPosition = (StretchingAnchor.GetStretchingStraights()[0].transform.position.y + StretchingAnchor.GetStretchingStraights()[StretchingAnchor.GetStretchingStraights().Count - 1].transform.position.y) / 2;
                RoadPiece parent = Instantiate(PrefabManager.Instance.GetPieceOfType(RoadType.None), new Vector3(newXPosition, newYPosition, 10), Quaternion.Euler(0f, 0f, StretchingAnchor.Orientation));

                BoxCollider2D bc2d = parent.gameObject.AddComponent<BoxCollider2D>();
                bc2d.size = new Vector2(StretchingAnchor.GetStretchingStraights().Count * 3.78f, 37.8f);

                parent.Width = bc2d.size.x;
                parent.Height = bc2d.size.y;
                foreach (GameObject straight in StretchingAnchor.GetStretchingStraights())
                {
                    straight.transform.SetParent(parent.gameObject.transform);
                }

                parent.PopulateVirtualAnchorPoints();
                parent.Rotation = StretchingAnchor.Orientation;


                DeselectRoad();
                SelectRoad(parent);

                foreach (VirtualAnchor va in parent.AnchorPoints)
                {
                    va.Update(StretchingAnchor.Orientation);
                }
                List<RoadPiece> roadsInArea = GetNearestNeighborsInArea(RoadList).referenceNeighbors;
                GetNeighborsReference(roadsInArea, SelectedRoad);

            }
            StretchingAnchor.ChildStraightPieces = new List<GameObject>();
            StretchingAnchor = null;
        }

        /// <summary>
        /// This method will visually add straight roads, as the user stretches a road piece
        /// </summary>
        public void StretchRoad()
        {
            IsStretching = true;
            Vector3 origin = SelectedRoad.transform.position + StretchingAnchor.Offset;

            //Normalize Offset Vector
            Vector3 offsetNormalized = StretchingAnchor.Offset.normalized;

            //Get Mouse Position
            Vector3 mousePosition = GetWorldPositionFromMouse();

            //Get the Vector from the mouse to the anchor point
            Vector3 vectorToMouse = mousePosition - origin;

            //calculate the dot Product to check, that the distance is positive in the direction of the vector. (So moving mouse in opposite direction will result in negative value)
            float dotProduct = Vector3.Dot(offsetNormalized, vectorToMouse);

            //Create Vector between Anchor Point and Mouse, but only the direction of the Offset
            Vector3 projectedVector = Vector3.Project(vectorToMouse, offsetNormalized);

            // Calculate the distance of that vector which only checks the distance in Offset direction. 
            float distance = projectedVector.magnitude;

            if (dotProduct >= 0 && distance >= StretchingDistance)
            {
                Vector3 newPosition = origin + (offsetNormalized * StretchingDistance) - (offsetNormalized * (3.78f * 5 / 2));
                newPosition = new Vector3(newPosition.x, newPosition.y, 9);

                GameObject go = new GameObject("Straight Stretched" + StretchingAnchor.GetStretchingStraights().Count);
                go.transform.position = newPosition;
                go.transform.localScale = new Vector3(5, 5, 1);
                go.transform.rotation = Quaternion.Euler(0f, 0f, StretchingAnchor.Orientation);
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("sprites/StraightShort");
                /*RoadPiece RoadPiece = PrefabManager.Instance.GetPieceOfType(RoadType.StraightShort);
                RoadPiece straight = Instantiate(RoadPiece, newPosition, Quaternion.Euler(0f, 0f, StretchingAnchor.Orientation));
                */
                StretchingAnchor.AddStretchingStraight(go);

                StretchingDistance += 3.78f * 5;
            }
            else if (dotProduct < StretchingDistance - 3.78f * 5)
            {
                if (StretchingAnchor.GetStretchingStraights().Count > 0)
                {
                    /*RoadPiece straight = StretchingAnchor.GetStretchingStraights()[StretchingAnchor.GetStretchingStraights().Count - 1];
                    DeleteRoad(straight);
                    */
                    Destroy(StretchingAnchor.GetStretchingStraights()[StretchingAnchor.GetStretchingStraights().Count - 1]);

                    StretchingAnchor.RemoveLastStretchingStraight();

                    StretchingDistance -= 3.78f * 5;
                }
            }

        }

        /// <summary>
        /// This method will add a new road to the list of roads. This list contains all roads existing. 
        /// </summary>
        /// <param name="road"> The road to be added to the list </param>
        public void AddRoadToList(RoadPiece road)
        {
            RoadList.Add(road);
        }

        /// <summary>
        /// Removes a road from the Roads List
        /// </summary>
        /// <param name="road"> The road to be removed from the list </param>
        public void RemoveRoadFromList(RoadPiece road)
        {
            RoadList.Remove(road);
        }


        /// <summary>
        /// This method will create a new road piece when the user selects a road piece from the piece menu. It will automatically assign it its road type.
        /// </summary>
        public void CreateRoad()
        {

            // Creates the new roadpiece
            var RoadPiece = PrefabManager.Instance.GetPieceOfType(ButtonManager.Instance.GetSelectedRoadType());
            var newRoadPiece = Instantiate(RoadPiece, GetWorldPositionFromMouse(), Quaternion.identity);
            newRoadPiece.transform.position = new Vector3(newRoadPiece.transform.position.x, Bottombar.GetComponent<BoxCollider2D>().bounds.center.y + Bottombar.GetComponent<BoxCollider2D>().bounds.extents.y + newRoadPiece.GetComponent<BoxCollider2D>().bounds.extents.y, newRoadPiece.transform.position.z);
            NewRoadCreated = true;

            SelectRoad(newRoadPiece);
        }

        /// <summary>
        /// This method will delete the road that has been given as a parameter
        /// </summary>
        /// <param name="road"> The road to be deleted</param>
        public void DeleteRoad(RoadPiece road)
        {

            if (road != null && !road.IsLocked)
            {
                // We have to destroy the SelectedObject, as the road is not a GameObject, but a RoadPiece. The effect will be the same, though. 

                foreach (VirtualAnchor va in road.AnchorPoints)
                {
                    va.RemoveConntectedAnchorPoint();
                }
                RemoveRoadFromList(road);
                if (road == SelectedRoad)
                {
                    DeselectRoad();
                }
                Destroy(road.gameObject);
            }
        }

        /*
         * This method drags a road across the screen. When the user drags a piece, it will follow the cursor of the mouse. This is only the case, if the road piece is not locked
         */
        /// <summary>
        /// This method will check, whether the user has selected a road piece and will move the road piece with the position of the mouse to imitate a drag functionality. 
        /// </summary>
        private void DragAndDropRoad()
        {
            if (!IsStretching)
            {
                // This will only check the object at the start of dragging and not check every frame. Prevents the user from going too fast and "Losing" the road. 
                if (IsDragging == false)
                {
                    SelectedObject = GetMouseObject();
                }
                // This will, if a road is selected, set dragging to true, deselect previous roads and select the new road. The road is then dragged with the mouse
                if (SelectedObject != null)
                {
                    if (SelectedObject.GetComponent<RoadPiece>() != null)
                    {
                        DeselectRoad();
                        SelectRoad(SelectedObject.GetComponent<RoadPiece>());

                        if (!CheckMouseCollisionWithPanels())
                        {
                            if (!SelectedRoad.IsLocked)
                            {
                                IsDragging = true;
                                IsSnapped = false;
                                SelectedRoad.transform.position = GetWorldPositionFromMouse();

                                foreach (VirtualAnchor va in SelectedRoad.AnchorPoints)
                                {
                                    va.RemoveConntectedAnchorPoint();
                                }

                                if (!Input.GetKey(KeyCode.LeftAlt))
                                {
                                    Snap();
                                }
                            }
                        }
                        else
                        {
                            IsDragging = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method will check whether the mouse position is over the side or bottom bar to restrict the movement of the roadpiece over the side or bottom bar
        /// </summary>
        public bool CheckMouseCollisionWithPanels()
        {
            BoxCollider2D road = SelectedRoad.gameObject.GetComponent<BoxCollider2D>();
            Vector3 mousePosition = GetWorldPositionFromMouse();
            BoxCollider2D sidebar = Sidebar.GetComponent<BoxCollider2D>();
            BoxCollider2D bottombar = Bottombar.GetComponent<BoxCollider2D>();

            if (mousePosition.x > sidebar.bounds.center.x - sidebar.bounds.extents.x - road.bounds.extents.x)
            {
                if (!(mousePosition.y < bottombar.bounds.center.y + bottombar.bounds.extents.y + road.bounds.extents.y))
                {
                    SelectedRoad.transform.position = new Vector3(SelectedRoad.transform.position.x, mousePosition.y, SelectedRoad.transform.position.z);
                }
                return true;
            }
            if (mousePosition.y < bottombar.bounds.center.y + bottombar.bounds.extents.y + road.bounds.extents.y)
            {
                if (!(mousePosition.x > sidebar.bounds.center.x - sidebar.bounds.extents.x - road.bounds.extents.x))
                {
                    SelectedRoad.transform.position = new Vector3(mousePosition.x, SelectedRoad.transform.position.y, SelectedRoad.transform.position.z);
                }
                return true;
            }

            return false;
        }


        /// <summary>
        /// This method will check, whether the user has selected a group and will move the entire group with the position of the mouse to imitate a drag functionality. 
        /// </summary>
        private void DragAndDropRoads()
        {
            if (IsDragging == false)
            {
                SelectedObject = GetMouseObject();
                InitialPositionOfGroup = SelectedRoad != null ? SelectedRoad.transform.position : InitialPositionOfGroup = new Vector3();
            }

            if (SelectedObject != null && SelectedObject.GetComponent<RoadPiece>() != null)
            {
                RoadPiece previouslySelectedRoad = null;
                if (SelectedRoad != SelectedObject.GetComponent<RoadPiece>())
                {
                    previouslySelectedRoad = SelectedRoad;

                }
                DeselectRoad();
                SelectRoad(SelectedObject.GetComponent<RoadPiece>());


                if (previouslySelectedRoad != null)
                {
                    if (SelectedRoads.Contains(SelectedRoad) && SelectedRoads.Contains(previouslySelectedRoad))
                    {
                        DeselectGroup();
                        SelectGroupOfRoads(SelectedRoad);
                    }
                    ColorRoadPiece(previouslySelectedRoad, SelectionColor.groupSelected);
                }

                if (SelectedRoads.Contains(SelectedRoad))
                {
                    if (CtrlDeselectedRoads.Count != 0)
                    {
                        foreach (RoadPiece road in CtrlDeselectedRoads)
                        {
                            foreach (VirtualAnchor va in road.AnchorPoints)
                            {
                                if (SelectedRoads.Contains(va.ConnectedAnchorPoint?.RoadPiece))
                                {
                                    va.RemoveConntectedAnchorPoint();
                                }
                            }
                        }
                        CtrlDeselectedRoads = new List<RoadPiece>();

                    }
                    IsDragging = true;
                    IsSnappedGroup = false;

                    foreach (RoadPiece road in SelectedRoads)
                    {
                        foreach (VirtualAnchor va in road.AnchorPoints)
                        {
                            if (!SelectedRoads.Contains(va.ConnectedAnchorPoint?.RoadPiece))
                            {
                                va.RemoveConntectedAnchorPoint();
                            }
                        }
                    }
                    Vector3 newPosition = GetWorldPositionFromMouse();

                    if (InitialPositionOfGroup != Vector3.zero && SelectedRoads.Find(road => road.IsLocked == true) == null)
                    {
                        Vector3 shift = newPosition - InitialPositionOfGroup;
                        shift.z = 0f;

                        foreach (RoadPiece road in SelectedRoads)
                        {
                            road.transform.position += shift;

                        }
                        InitialPositionOfGroup = SelectedRoad.transform.position;

                        foreach (VirtualAnchor va in SelectedRoad.AnchorPoints)
                        {
                            if (!SelectedRoads.Contains(va.ConnectedAnchorPoint?.RoadPiece))
                            {
                                va.RemoveConntectedAnchorPoint();
                            }
                        }
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            SnapGroup();
                        }
                    }
                }
                else
                {
                    DeselectRoad();
                    DeselectGroup();
                    SelectedRoads = null;
                }
            }
        }
        //Eva
        public (List<RoadPiece> snappingNeighbors, List<RoadPiece> referenceNeighbors) GetNearestNeighborsInArea(List<RoadPiece> roadList)
        {
            List<RoadPiece> snappingNeighbors = new List<RoadPiece>();
            List<RoadPiece> referenceNeighbors = new List<RoadPiece>();

            if (roadList.Count > 1)
            {
                float maxSnappingArea = SelectedRoad.Width * 5f;
                foreach (RoadPiece road in roadList)
                {
                    float sizePieceFound = road.Width * 5f;
                    if (Vector3.Distance(SelectedRoad.transform.position, road.transform.position) <= maxSnappingArea + SNAPPING_DISTANCE + sizePieceFound && SelectedRoad != road)
                    {
                        referenceNeighbors.Add(road);
                        if (Vector3.Distance(SelectedRoad.transform.position, road.transform.position) <= maxSnappingArea / 2 + SNAPPING_DISTANCE + sizePieceFound / 2 && SelectedRoad != road)
                        {
                            snappingNeighbors.Add(road);
                        }
                    }
                }
            }
            return (snappingNeighbors, referenceNeighbors);
        }
        // Eva
        public (VirtualAnchor selectedRoadVA, VirtualAnchor nearestNeighborVA) GetNearestAnchorPoints(List<RoadPiece> nearestNeighbors)
        {
            float nearestDistance = -1;
            VirtualAnchor selectedRoadVA = null;
            VirtualAnchor nearestNeighborVA = null;

            foreach (RoadPiece RoadPiece in nearestNeighbors)
            {
                foreach (VirtualAnchor neighborVA in RoadPiece.AnchorPoints)
                {
                    if (nearestDistance == -1 || Vector3.Distance(neighborVA.RoadPiece.transform.position + neighborVA.Offset, SelectedRoad.transform.position) < nearestDistance)
                    {
                        nearestDistance = Vector3.Distance(neighborVA.RoadPiece.transform.position + neighborVA.Offset, SelectedRoad.transform.position);
                        nearestNeighborVA = neighborVA;
                    }
                }
            }
            float nearestAnchorPoint = -1;
            if (nearestNeighborVA != null)
            {
                foreach (VirtualAnchor selectedVA in SelectedRoad.AnchorPoints)
                {
                    if (selectedVA.ConnectedAnchorPoint == null && nearestNeighborVA.ConnectedAnchorPoint == null && (nearestAnchorPoint == -1 || Vector3.Distance(nearestNeighborVA.RoadPiece.transform.position + nearestNeighborVA.Offset, selectedVA.RoadPiece.transform.position + selectedVA.Offset) < nearestAnchorPoint))
                    {
                        nearestAnchorPoint = Vector3.Distance(nearestNeighborVA.RoadPiece.transform.position + nearestNeighborVA.Offset, selectedVA.RoadPiece.transform.position + selectedVA.Offset);
                        selectedRoadVA = selectedVA;
                    }
                }
            }
            return (selectedRoadVA, nearestNeighborVA);
        }

        /// <summary>
        /// This mehtod compares the orientation of two road pieces to match the rotation when they are being snapped to each other
        /// </summary>
        /// <param name="neighbor"> The road that is being snapped to </param>
        /// <param name="selected"> The road that should be snapped </param>
        /// <returns> returns the difference in orientation (rotation) </returns>
        public float CompareAnchorPointOrientation(VirtualAnchor neighbor, VirtualAnchor selected)
        {
            if (Mathf.Abs(neighbor.Orientation - selected.Orientation) == 180)
            {
                return 0;
            }
            else
            {
                float orientationDifference = Mathf.Abs(neighbor.Orientation - selected.Orientation);
                float neededOrientation = orientationDifference - 180;

                if (neighbor.Orientation >= selected.Orientation)
                {

                    RotateRoadPiece(neededOrientation, false);
                    return neededOrientation;
                }
                else
                {
                    RotateRoadPiece(-neededOrientation, false);
                    return -neededOrientation;
                }
            }
        }

        /// <summary>
        /// This method will snap two road pieces that are close to each other together and will match their orientation
        /// </summary>
        public void Snap()
        {
            var nearestNeighbors = GetNearestNeighborsInArea(RoadList);
            var nearestAnchorPoints = GetNearestAnchorPoints(nearestNeighbors.snappingNeighbors);

            if (nearestAnchorPoints.nearestNeighborVA != null && nearestAnchorPoints.selectedRoadVA != null)
            {
                if (SelectedRoad.LastNeighborSnappedAnchorPoint != nearestAnchorPoints.nearestNeighborVA)
                {
                    CompareAnchorPointOrientation(nearestAnchorPoints.nearestNeighborVA, nearestAnchorPoints.selectedRoadVA);
                    SelectedRoad.transform.position = (nearestAnchorPoints.nearestNeighborVA.RoadPiece.transform.position + nearestAnchorPoints.nearestNeighborVA.Offset) - nearestAnchorPoints.selectedRoadVA.Offset;
                    nearestAnchorPoints.selectedRoadVA.ConnectAnchorPoint(nearestAnchorPoints.nearestNeighborVA);
                    SelectedRoad.LastNeighborSnappedAnchorPoint = nearestAnchorPoints.nearestNeighborVA;
                    SelectedRoad.LastSelectedSnappedAnchorPoint = nearestAnchorPoints.selectedRoadVA;
                }
                else
                {
                    CompareAnchorPointOrientation(nearestAnchorPoints.nearestNeighborVA, SelectedRoad.LastSelectedSnappedAnchorPoint);
                    SelectedRoad.transform.position = (nearestAnchorPoints.nearestNeighborVA.RoadPiece.transform.position + nearestAnchorPoints.nearestNeighborVA.Offset) - SelectedRoad.LastSelectedSnappedAnchorPoint.Offset;
                    SelectedRoad.LastSelectedSnappedAnchorPoint.ConnectAnchorPoint(nearestAnchorPoints.nearestNeighborVA);
                }
                this.IsSnapped = true;
                ColorRoadPiece(SelectedRoad, SelectionColor.snapped);
                GetNeighborsReference(nearestNeighbors.referenceNeighbors, SelectedRoad);
            }
        }

        /// <summary>
        /// This method will snap a group to another road piece. The snapping will originate from the selected road of the group
        /// </summary>
        public void SnapGroup()
        {
            List<RoadPiece> viableNeighbors = new List<RoadPiece>();
            viableNeighbors.Add(SelectedRoad);
            foreach (RoadPiece road in RoadList)
            {
                if (!SelectedRoads.Contains(road))
                {
                    viableNeighbors.Add(road);
                }
            }

            var nearestNeighbors = GetNearestNeighborsInArea(viableNeighbors);
            var nearestAnchorPoints = GetNearestAnchorPoints(nearestNeighbors.snappingNeighbors);

            float rotationForGroup = 0;
            if (nearestAnchorPoints.nearestNeighborVA != null && nearestAnchorPoints.selectedRoadVA != null)
            {
                if (SelectedRoad.LastNeighborSnappedAnchorPoint != nearestAnchorPoints.nearestNeighborVA)
                {
                    rotationForGroup = CompareAnchorPointOrientation(nearestAnchorPoints.nearestNeighborVA, nearestAnchorPoints.selectedRoadVA);
                    SelectedRoad.transform.position = (nearestAnchorPoints.nearestNeighborVA.RoadPiece.transform.position + nearestAnchorPoints.nearestNeighborVA.Offset) - nearestAnchorPoints.selectedRoadVA.Offset;
                    nearestAnchorPoints.selectedRoadVA.ConnectAnchorPoint(nearestAnchorPoints.nearestNeighborVA);
                    SelectedRoad.LastNeighborSnappedAnchorPoint = nearestAnchorPoints.nearestNeighborVA;
                    SelectedRoad.LastSelectedSnappedAnchorPoint = nearestAnchorPoints.selectedRoadVA;
                }
                else
                {
                    rotationForGroup = CompareAnchorPointOrientation(nearestAnchorPoints.nearestNeighborVA, SelectedRoad.LastSelectedSnappedAnchorPoint);

                    SelectedRoad.transform.position = (nearestAnchorPoints.nearestNeighborVA.RoadPiece.transform.position + nearestAnchorPoints.nearestNeighborVA.Offset) - SelectedRoad.LastSelectedSnappedAnchorPoint.Offset;
                    SelectedRoad.LastSelectedSnappedAnchorPoint.ConnectAnchorPoint(nearestAnchorPoints.nearestNeighborVA);
                }

                AlignGroupPiecesToEachOther(rotationForGroup);
                IsSnappedGroup = true;
                foreach (RoadPiece road in SelectedRoads)
                {
                    List<RoadPiece> neighborRoads = RoadList.FindAll(r => !SelectedRoads.Contains(r));
                    GetNeighborsReference(neighborRoads, road);
                }
                ColorRoadPiece(SelectedRoad, SelectionColor.snapped);
            }
        }

        /// <summary>
        /// This method will align the group pieces to each other, so that when the selected road of a group is snapped or rotated, the connected road pieces are aligned and repositioned. 
        /// </summary>
        /// <param name="rotationForGroup"> This is the rotation by which the rest of the group has to be rotated to match the selected roads rotation </param>
        public void AlignGroupPiecesToEachOther(float rotationForGroup)
        {
            List<RoadPiece> snappedRoads = new List<RoadPiece>();
            foreach (RoadPiece road in SelectedRoads)
            {
                foreach (VirtualAnchor va in road.AnchorPoints)
                {
                    if (va.ConnectedAnchorPoint != null && !snappedRoads.Contains(va.ConnectedAnchorPoint.RoadPiece) && SelectedRoads.Contains(va.ConnectedAnchorPoint.RoadPiece))
                    {
                        if (va.ConnectedAnchorPoint.RoadPiece != SelectedRoad)
                        {
                            RotateRoadPiece(va.ConnectedAnchorPoint.RoadPiece, rotationForGroup);
                            va.ConnectedAnchorPoint.RoadPiece.transform.position = va.RoadPiece.transform.position + va.Offset - va.ConnectedAnchorPoint.Offset;
                        }
                        snappedRoads.Add(va.ConnectedAnchorPoint.RoadPiece);
                    }
                }

            }
        }

        /// <summary>
        /// This sets the selected road and will change the color of the piece accordingly.
        /// </summary>
        /// <param name="road"> The road to be selected </param>
        public void SelectRoad(RoadPiece road)
        {
            if (SelectedRoad == null)
            {
                SelectedRoad = road;
                ColorRoadPiece(SelectedRoad, SelectionColor.selected);
            }
        }

        /// <summary>
        ///  This deselects the currently selected road and changes the color accordingly. 
        /// </summary>
        public void DeselectRoad()
        {
            if (SelectedRoad != null)
            {
                ColorRoadPiece(SelectedRoad, SelectionColor.normal);
                SelectedRoad.transform.position = new Vector3(SelectedRoad.transform.position.x, SelectedRoad.transform.position.y, 9);
                SelectedRoad = null;
                IsDragging = false;
            }
        }

        /*
         * This rotates the selected roadpiece by a given rotation. Only if the piece is not locked
         */
        /// <summary>
        /// This will rotate the selected roadpiece or group by the specified rotation
        /// </summary>
        /// <param name="rotation"> Rotation by which the selected road piece or group should be rotated </param>
        /// <param name="manualRotation"> This boolean defines, if the rotation is prompted by the user or not (e.g., by snapping) </param>
        public void RotateRoadPiece(float rotation, bool manualRotation)
        {
            if (SelectedRoad != null && !SelectedRoad.IsLocked)
            {
                SelectedRoad.transform.Rotate(new Vector3(0, 0, rotation));
                SelectedRoad.Rotation = SelectedRoad.Rotation + rotation;

                foreach (VirtualAnchor va in SelectedRoad.AnchorPoints)
                {
                    va.Update(rotation);
                }
                if (SelectedRoads != null && manualRotation)
                {
                    AlignGroupPiecesToEachOther(rotation);
                }
            }
        }

        /// <summary>
        /// This method will not rotate the selected road, but a specified road by a specified rotation
        /// </summary>
        /// <param name="road"> The road to be rotated </param>
        /// <param name="rotation"> The degree by which the road should be rotated </param>
        public void RotateRoadPiece(RoadPiece road, float rotation)
        {
            if (road != null)
            {
                road.transform.Rotate(new Vector3(0, 0, rotation));
                road.Rotation = road.Rotation + rotation;

                foreach (VirtualAnchor va in road.AnchorPoints)
                {
                    va.Update(rotation);
                }
            }
        }

        /*
         * Connects by Reference all AnchorPoints of a Piece placed between multiple Pieces
         */
        /// <summary>
        /// This method will connect a piece to all piece it is connected to, even if it only snapped to one. 
        /// </summary>
        /// <param name="neighborRoads"> The roads that are in a specific area of the snapped road </param>
        /// <param name="road"> the road, that has been snapped </param>
        public void GetNeighborsReference(List<RoadPiece> neighborRoads, RoadPiece road)
        {
            bool stop = false;
            foreach (VirtualAnchor vaS in road.AnchorPoints)
            {
                if (vaS.ConnectedAnchorPoint == null)
                {
                    foreach (RoadPiece neighbor in neighborRoads)
                    {
                        foreach (VirtualAnchor vaN in neighbor.AnchorPoints)
                        {
                            if (vaN.ConnectedAnchorPoint == null && Vector3.Distance(vaS.RoadPiece.transform.position + vaS.Offset, vaN.RoadPiece.transform.position + vaN.Offset) <= 1)
                            {
                                vaS.ConnectAnchorPoint(vaN);
                                stop = true;
                                break;
                            }
                        }
                        if (stop == true)
                            break;
                    }
                }
                stop = false;
            }
        }

        /// <summary>
        /// This method will return the clicked object when the user presses the mouse button on the screen.
        /// </summary>
        /// <returns> Return the GameObject, if the user has clicked one (rotation) </returns>

        private GameObject GetMouseObject()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }

            return null;
        }

        /// <summary>
        /// This method gets the position of the mouse relative to the world, not to the screen
        /// </summary>
        /// <returns> returns a Vector3 that represents a position  </returns>        
        private Vector3 GetWorldPositionFromMouse()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.nearClipPlane;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            worldPosition.z = 1;
            return worldPosition;
        }

        /// <summary>
        /// This method colors road pieces based on the state of this piece (clicked, snapped, grouped, etc.) and the locked attribute of the road piece
        /// </summary>

        public void ColorRoadPiece(RoadPiece road, SelectionColor sColor)
        {
            Color color = new Color();
            if (!road.IsLocked)
            {
                switch (sColor)
                {
                    case SelectionColor.normal:
                        color = Color.white;
                        break;
                    case SelectionColor.selected:
                        color = new Color(0.475f, 0.714f, 0.851f, 1f);
                        break;
                    case SelectionColor.groupSelected:
                        color = new Color(0f, 1f, 0.75f, 1f);
                        break;
                    case SelectionColor.snapped:
                        color = new Color(0.72f, 1f, 0.65f, 1f);
                        break;
                    case SelectionColor.groupSnapped:
                        color = new Color(0.22f, 0.91f, 0.247f, 1f);
                        break;
                    case SelectionColor.stretching:
                        color = new Color(0.823f, 0.792f, 0.337f, 1f);
                        break;
                    case SelectionColor.invalid:
                        color = new Color(0.84f, 0.145f, 0.145f, 1f);
                        break;
                    default:
                        break;
                }
            }
            else
            {

                switch (sColor)
                {
                    case SelectionColor.normal:
                        color = new Color(1f, 1f, 1f, 0.5f);
                        break;
                    case SelectionColor.selected:
                        color = new Color(0.475f, 0.714f, 0.851f, 0.5f);
                        break;
                    case SelectionColor.groupSelected:
                        color = new Color(0f, 1f, 0.75f, 0.5f);
                        break;
                    case SelectionColor.groupSnapped:
                        color = new Color(0.22f, 0.91f, 0.247f, 1f);
                        break;
                    case SelectionColor.snapped:
                        color = new Color(0.72f, 1f, 0.65f, 0.5f);
                        break;
                    case SelectionColor.stretching:
                        color = new Color(0.823f, 0.792f, 0.337f, 0.5f);
                        break;
                    case SelectionColor.invalid:
                        color = new Color(0.84f, 0.145f, 0.145f, 0.5f);
                        break;
                    default:
                        break;
                }
            }
            if (road.gameObject.transform.childCount == 0 || road.gameObject.GetComponent<SpriteRenderer>().sprite != null)
            {
                road.GetComponent<SpriteRenderer>().color = color;
            }
            else if (road.gameObject.transform.childCount > 0)
            {
                for (int i = 0; i < road.gameObject.transform.childCount; i++)
                {
                    road.gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = color;
                }
            }
        }

        /// <summary>
        /// This method will lock or unlock a road and color the road piece based on the locked attribute
        /// </summary>
        /// <param name="road"> The road to be locked and colored </param>
        /// <param name="locked"> boolean, that states the new locked state of the road</param>
        public void LockRoad(RoadPiece road, bool locked)
        {
            road.IsLocked = locked;
            if (SelectedRoads != null && SelectedRoads.Contains(road))
            {
                ColorRoadPiece(road, SelectionColor.groupSelected);
            }
            else if (SelectedRoads == null)
            {
                ColorRoadPiece(road, SelectionColor.selected);
            }
        }
    }
}
