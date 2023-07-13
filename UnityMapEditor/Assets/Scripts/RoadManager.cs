
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


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
        public List<RoadPiece> RoadList = new List<RoadPiece>();

        // selected Road
        public RoadPiece SelectedRoad { get; set; } = null;
        public List<RoadPiece> SelectedRoads { get; set; } = null;

        // selected Object, not necessarily a road
        public GameObject SelectedObject { get; set; }

        // boolean for road validation. If false, certain functions are disabled
        public bool InValidPosition { get; set; } = true;

        // used to check whether the user is currently dragging. So no other object can be selected during a drag. 
        private bool IsDragging { get; set; } = false;

        private bool IsInStretchingArea { get; set; } = false;
        private bool IsStretching { get; set; } = false;
        private bool IsSnapped { get; set; } = false;

        // Rotating Angle of the Pieces
        private const float SNAPPING_DISTANCE = 200;
        private const float ROTATING_ANGLE = 15f;

        private VirtualAnchor StretchingAnchor { get; set; }

        private float StretchingDistance { get; set; } = 3.78f * 5;

        public Vector3 InitialPositionOfGroup { get; set; } = new Vector3();
        public bool IsSnappedGroup { get; set; } = false;

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
            if (SelectedRoad != null)
            {
                CheckStretchPosition();
            }
            // This condition checks, whether a tile is being dragged. 
            if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (!Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (!IsDragging && IsInStretchingArea && SelectedRoads == null)
                    {
                        StretchRoad();
                    }
                    else if (SelectedRoads != null)
                    {
                        DragAndDropRoads();
                    }
                    else
                    {
                        DragAndDropRoad();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    RoadPiece clickedRoad = GetMouseObject()?.GetComponent<RoadPiece>();
                    if (clickedRoad != null)
                    {
                        DeselectRoad();
                        DeselectGroup();
                        SelectRoad(clickedRoad);
                        SelectGroupOfRoads(clickedRoad);
                    }
                }
            }

            // This condition checks, when the user releases the drag and checks the road position
            if (Input.GetMouseButtonUp(0))
            {
                IsDragging = false;
                if (IsStretching)
                {
                    CreateCustomStraightRoad();
                }
                IsStretching = false;
                StretchingDistance = 3.78f * 5;
                ValidateRoadPosition();
            }

            // These conditions checks, whether the user wants to rotate the piece clockwise or counter-clockwise
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (IsSnapped && SelectedRoads == null)
                {

                    int currentIndex = SelectedRoad.AnchorPoints.FindIndex(anchor => anchor == SelectedRoad.LastSelectedSnappedAnchorPoint);
                    VirtualAnchor nextAnchor = SelectedRoad.AnchorPoints[(currentIndex + 1) % SelectedRoad.AnchorPoints.Count];
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
                        float test = CompareAnchorPointOrientation(SelectedRoad.LastNeighborSnappedAnchorPoint, nextAnchor);
                        foreach (VirtualAnchor va in SelectedRoad.AnchorPoints)
                        {
                            if (!SelectedRoads.Contains(va.ConnectedAnchorPoint?.RoadPiece))
                            {
                                va.RemoveConntectedAnchorPoint();
                            }
                        }
                        SelectedRoad.LastSelectedSnappedAnchorPoint = nextAnchor;
                        AlignGroupPiecesToEachOther(test);
                    }
                }
                else if (SelectedRoads != null)
                {
                    RotateRoadPiece(-ROTATING_ANGLE, true);
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
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
                        float test = CompareAnchorPointOrientation(SelectedRoad.LastNeighborSnappedAnchorPoint, nextAnchor);
                        foreach (VirtualAnchor va in SelectedRoad.AnchorPoints)
                        {
                            if (!SelectedRoads.Contains(va.ConnectedAnchorPoint?.RoadPiece))
                            {
                                va.RemoveConntectedAnchorPoint();
                            }
                        }
                        SelectedRoad.LastSelectedSnappedAnchorPoint = nextAnchor;
                        AlignGroupPiecesToEachOther(test);
                    }
                }
                else
                {
                    RotateRoadPiece(ROTATING_ANGLE, true);
                }
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
                    foreach (RoadPiece road in SelectedRoads)
                    {
                        LockRoad(road, !road.IsLocked);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
            {
                ControlSelectRoadPiece();
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


        public void ControlSelectRoadPiece()
        {
            RoadPiece road = GetMouseObject()?.GetComponent<RoadPiece>();
            if (road != null && SelectedRoads.Contains(road))
            {

                foreach (VirtualAnchor va in road.AnchorPoints)
                {
                    if (SelectedRoads.Contains(va.ConnectedAnchorPoint?.RoadPiece))
                    {
                        va.RemoveConntectedAnchorPoint();
                    }
                }

                SelectedRoads.Remove(road);
                ColorRoadPiece(road, SelectionColor.normal);
            }
            else if (road != null && !SelectedRoads.Contains(road))
            {
                SelectedRoads.Add(road);
                ColorRoadPiece(road, SelectionColor.groupSelected);
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
            if (!roads.Contains(roadToCheck))
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
                                ColorRoadPiece(SelectionColor.invalid);
                                IsInStretchingArea = true;
                                StretchingAnchor = anchor;
                                return;
                            }
                            else
                            {
                                ColorRoadPiece(SelectionColor.selected);
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
                ColorRoadPiece(SelectionColor.selected);
                IsInStretchingArea = false;
                StretchingAnchor = null;
                //UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
                SelectedRoad = SelectedObject.GetComponent<RoadPiece>();
                foreach (RoadPiece r in SelectedRoads)
                {
                    Debug.Log(r.IsLocked);
                }
                if (SelectedRoads.Contains(SelectedRoad))
                {
                    IsDragging = true;
                    IsSnappedGroup = false;
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
                    Debug.Log(neighborRoads.Count);
                    GetNeighborsReference(neighborRoads, road);
                }
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
                ValidateRoadPosition();
                ColorRoadPiece(SelectedRoad.IsLocked ? SelectionColor.lockedSelected : SelectionColor.selected);
            }
        }

        /*
         * This deselect any road currently selected and changes the color accordingly. 
         */
        public void DeselectRoad()
        {
            if (SelectedRoad != null)
            {
                ColorRoadPiece(SelectedRoad.IsLocked ? SelectionColor.locked : SelectionColor.normal);
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

        /*
         * This will color the road piece dependend on the status the road currently has. 
         */
        public void ColorRoadPiece(SelectionColor sColor)
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
            if (SelectedRoad.gameObject.transform.childCount == 0 || SelectedRoad.gameObject.GetComponent<SpriteRenderer>().sprite != null)
            {
                SelectedRoad.GetComponent<SpriteRenderer>().color = color;
            }
            else if (SelectedRoad.gameObject.transform.childCount > 0)
            {
                for (int i = 0; i < SelectedRoad.gameObject.transform.childCount; i++)
                {
                    SelectedRoad.gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = color;
                }
            }
        }
        public void ColorRoadPiece(RoadPiece road, SelectionColor sColor)
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
                case SelectionColor.groupSelected:

                    color = new Color(0.48f, 0.12f, 0.64f, 1f);
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
        }

    }
}
