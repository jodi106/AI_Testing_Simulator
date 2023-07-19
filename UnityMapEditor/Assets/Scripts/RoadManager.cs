
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

        // TO BE USED SOMEWHERE - This is a boolean for road validation. If false, certain functions are disabled
        public bool InValidPosition { get; set; } = true;

        // This boolean is used to check whether the user is currently dragging. So no other object can be selected during a drag. 
        private bool IsDragging { get; set; } = false;

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
        private void Awake()
        {
            instance = this;
        }

        /*
        * The Update method is a "Monobehavior" method from Unity, which is automaticlly called every frame. 
        * This method is used to check for user input from mouse or keyboard. 
        */
        void Update()
        {
            // This condition checks, whether the user has selected a road to then check the stretching position
            if (SelectedRoad != null)
            {
                CheckStretchPosition();
            }

            // This condition checks, whether the User has pressed the Left Mouse Button (Also holding it) and has not pressed the Left Control Button 
            if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl))
            {
                // This condition checks, whether the user has pressed Left Shift (while pressing the LMB from the previous condition)
                if (!Input.GetKeyDown(KeyCode.LeftShift))
                {
                    // This conditions checks, whether the user is NOT dragging, is in the Stretching Area and there is no group selection
                    if (!IsDragging && IsInStretchingArea && SelectedRoads == null)
                    {
                        // If that is the case, dragging the mouse will stretch the road
                        StretchRoad();
                       // Debug.Log("l");
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
                // Else, it will check whether the user has pressed Left Shift (while pressing the LMB from the previous condition)
                else if (Input.GetKeyDown(KeyCode.LeftShift))
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

                //This conditions checks, whether the user has been stretching a road
                if (IsStretching)
                {
                    // if that is the case, then a new custom road is created
                    CreateCustomStraightRoad();
                    Snap();
                }
                IsStretching = false;
                StretchingDistance = 3.78f * 5;
                ValidateRoadPosition();
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

        public void RotateClockwise(){
                 // this condition checks, whether the selected road is snapped and no group has been selected
                if (IsSnapped && SelectedRoads == null)
                {
                    // If that is the case, we first find the current index of our anchor in the list of anchor points of the selected road
                    int currentIndex = SelectedRoad.AnchorPoints.FindIndex(anchor => anchor == SelectedRoad.LastSelectedSnappedAnchorPoint);
                    // if our anchor Point is the first anchor point in the list, we set the currentIndex to the count of the list to 
                    // retrieve the previous anchor point, which is the last
                    if (currentIndex == 0)
                    {
                        currentIndex = SelectedRoad.AnchorPoints.Count;
                    }
                    VirtualAnchor nextAnchor = SelectedRoad.AnchorPoints[(currentIndex - 1)];

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
public void RotateCounterClockwise(){ 
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

        public void CheckStretchPosition()
        {
            GameObject obj = GetMouseObject();
            if (SelectedRoads == null && SelectedRoad.IsLocked == false)
            {

                if (obj != null && obj.GetComponent<RoadPiece>() != null)
                {
                    RoadPiece road = obj.GetComponent<RoadPiece>();

                    foreach (VirtualAnchor anchor in SelectedRoad.AnchorPoints)
                    {
                        if (anchor.ConnectedAnchorPoint == null)
                        {
                            if (!IsStretching)
                            {

                                if (Vector3.Distance(GetWorldPositionFromMouse(), road.transform.position + anchor.Offset) < 50)
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
                GetNeighborsReference(new List<RoadPiece> { StretchingAnchor.RoadPiece }, SelectedRoad);
               // Snap();
                Debug.Log(1);
               // GetNeighborsReference(new List<RoadPiece> { StretchingAnchor.RoadPiece }, SelectedRoad);






                //TODO Implement Snapping here. SO when we create the piece we also check, whether the piece is connected.  

            }
            StretchingAnchor.ChildStraightPieces = new List<GameObject>();
            StretchingAnchor = null;
        }

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

        /*
         * This method will add every created road piece to the list of roads. This way, snapping can be implemented because the positions of roads can be determined. 
         */
        public void AddRoadToList(RoadPiece road)
        {
            RoadList.Add(road);
        }

        /*
         * Removes a road from the Roads List
         */
        public void RemoveRoadFromList(RoadPiece road)
        {
            RoadList.Remove(road);
        }
        /* 
         * This method creates a new road piece. It gets the selected road type from the sidebar and creates the corresponding prefab on the screen. 
         */
        public void CreateRoad()
        {

            // Creates the new roadpiece
            var RoadPiece = PrefabManager.Instance.GetPieceOfType(ButtonManager.Instance.GetSelectedRoadType());
            var newRoadPiece = Instantiate(RoadPiece, GetWorldPositionFromMouse(), Quaternion.identity);

            // Sets the valid position to false (As it will always spawn on the sidebar), adds the road to the list of roads and selects the road automatically upon creation, allowing the user to instantly drag it)
            InValidPosition = false;
            SelectRoad(newRoadPiece);
        }

        /*
         * This method deletes the selected road
         */
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
                            ValidateRoadPosition();
                        }

                    }
                }
            }
        }

        private void DragAndDropRoads()
        {
            if (IsDragging == false)
            {
                SelectedObject = GetMouseObject();
                InitialPositionOfGroup = SelectedRoad != null ? SelectedRoad.transform.position : InitialPositionOfGroup = new Vector3();
            }

            if (SelectedObject != null && SelectedObject.GetComponent<RoadPiece>() != null)
            {
                if (SelectedRoad != SelectedObject.GetComponent<RoadPiece>())
                {
                    if (SelectedRoad.IsLocked)
                    {
                        ColorRoadPiece(SelectedRoad, SelectionColor.groupSelected);
                    }
                    else
                    {
                        ColorRoadPiece(SelectedRoad, SelectionColor.groupSelected);
                    }
                }
                DeselectRoad();
                SelectRoad(SelectedObject.GetComponent<RoadPiece>());

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

        /*
         * This sets the selected road and will change the color of the piece accordingly. Also, it checks the current Road Position. 
         */
        public void SelectRoad(RoadPiece road)
        {
            if (SelectedRoad == null)
            {
                SelectedRoad = road;
                //SelectedRoads = new List<RoadPiece>();
                //SelectedRoads.Add(SelectedRoad);
                ValidateRoadPosition();
                ColorRoadPiece(SelectedRoad, SelectionColor.selected);
            }
        }

        /*
         * This deselect any road currently selected and changes the color accordingly. 
         */
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


        /*
         * This will get the clicked object on the screen. Currently only works for GameObjects. 
         */
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

        /*
         * This translates px into world position, so the user can only click inside the camera frame
         */
        private Vector3 GetWorldPositionFromMouse()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.nearClipPlane;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            worldPosition.z = 1;
            return worldPosition;
        }

        /*
         * This method validates the road position. Currently always true, as not implemented yet. 
         */
        private void ValidateRoadPosition()
        {
            InValidPosition = true;
        }

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
