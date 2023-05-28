using System.Collections.Generic;
using UnityEngine;
using Entity;
using System.ComponentModel;
using System.Linq;
using UnityEngine.UIElements;
using System;

namespace scripts
{
    public enum RoadType
    {
        [Description("None")]
        None, 
        [Description("StraightRoad")]
        StraightRoad,
        [Description("Turn")]
        Turn,
        [Description("ThreeWayIntersection")]
        ThreeWayIntersection,
        [Description("FourWayIntersection")]
        FourWayIntersection,
        [Description("ParkingBottom")]
        ParkingBottom,
        [Description("ParkingTop")]
        ParkingTop,
        [Description("ParkingTopAndBottom")]
        ParkingTopAndBottom,
        [Description("Crosswalk")]
        Crosswalk,
        [Description("FourWayRoundAbout")]
        FourWayRoundAbout,
        [Description("ThreeWayRoundAbout")]
        ThreeWayRoundAbout,
    }
    public enum SelectionColor 
    {
        normal,
        selected,
        invalid,
        snapped,
        locked,
        lockedSelected,
        lockedSnapped, 

    }

    public enum RoadSides
    {
        bottom, 
        top, 
        left, 
        right,
    }
    public class RoadPieceController : MonoBehaviour
    {
        private const float innerCircleRadius = 66.5f;

        private const float outerCircleRadius = 103.5f;

        private const float centerOffset = 18.5f;

        public Dictionary<Vector2, RoadPiece> pieces;

        // Start is called before the first frame update
        void Start()
        {
            pieces = new Dictionary<Vector2, RoadPiece>();
        }

        // Update is called once per frame
        public void Update()
        {

        }


        public void Add(RoadPiece piece)
        {
            pieces.Add(piece.transform.position, piece);
        }

        public void Align(GameObject current)
        {
            if (pieces.Count > 0)
            {
                var closestNeighborDistance = GetClosestPiece(current);

                Vector2 newPos = CalcPositionRelativeToNeighbor(closestNeighborDistance);

                var neighbor = pieces[closestNeighborDistance];

                PositionPiece(current, newPos, neighbor.transform.rotation);
            }
        }

        /**
         * changes the position of the piece current to the given position
         */
        public void PositionPiece(GameObject current, Vector2 position, Quaternion rotation)
        {
            current.transform.position = Vector2.MoveTowards(current.transform.position, position, 100000000);
        }

        /**
         * calculates the position of the piece, which should be draged to the map
         * based on position of the next closest road piece
         */
        public Vector2 CalcPositionRelativeToNeighbor(Vector3 neighborPos)
        {
            Vector2 position;
            position.x = neighborPos.x;
            position.y = neighborPos.y - 100;

            //position.x = Mathf.Cos(rotation) * radius;
            //position.y = Mathf.Sin(rotation) * radius;

            return position;
        }

        /**
         * searches the closest road piece to the moving piece by comparing the euclidian distance
         */
        public Vector3 GetClosestPiece(GameObject current)
        {
            RoadPiece closest = null;
            var minDistance = float.PositiveInfinity;

            foreach (var piece in pieces.Values)
            {
                var euklDist = Mathf.Sqrt((float)FastEuclideanDistance(piece.transform.position,
                                                                            current.transform.position));
                if (euklDist < minDistance)
                {
                    minDistance = euklDist;
                    closest = piece;
                }
            }

            if (minDistance > 10)
            {
                return current.transform.position;
            }

            return closest.transform.position;
        }

        public Road GetCoordsOfRoadPiece(Road previousRoad, int roadId, double roadLength, RoadType roadtype, float distanceBetweenPoints = 30)
        {
            var leftLaneWaypoints = new List<Waypoint>();
            var rightLaneWaypoints = new List<Waypoint>();

            leftLaneWaypoints.Add(new Waypoint(-centerOffset, 0));
            rightLaneWaypoints.Add(new Waypoint(centerOffset, 0));

            var leftLaneAngularOffset = Mathf.PI / 2;
            var rightLaneAngularOffset = Mathf.PI / 2;

            switch (roadtype)
            {
                case RoadType.StraightRoad:
                    var pointCount = roadLength / distanceBetweenPoints;

                    for (var index = 1; index < pointCount + 1; index++)
                    {
                        leftLaneWaypoints.Add(new Waypoint(-centerOffset, distanceBetweenPoints * index));
                        rightLaneWaypoints.Add(new Waypoint(centerOffset, distanceBetweenPoints * index));
                    }
                    break;
                case RoadType.Turn:
                    leftLaneAngularOffset = previousRoad.Lanes[-1].AngularOffset;
                    rightLaneAngularOffset = previousRoad.Lanes[1].AngularOffset;

                    var angleBetweenPointsOnOuterCircleInRadians = distanceBetweenPoints * outerCircleRadius;
                    var angleBetweenPointsOnInnerCircleInRadians = distanceBetweenPoints * innerCircleRadius;

                    leftLaneWaypoints.Concat(GetPointsOnCurvedLane(-1, roadId, leftLaneAngularOffset,
                        outerCircleRadius, -centerOffset, angleBetweenPointsOnOuterCircleInRadians)).ToList();

                    rightLaneWaypoints.Concat(GetPointsOnCurvedLane(1, roadId, rightLaneAngularOffset,
                        innerCircleRadius, centerOffset, angleBetweenPointsOnInnerCircleInRadians)).ToList();
                    break;
                default:
                    Debug.Log("wHAT za hek");
                    break;
            }
            var lanes = new SortedList<int, Lane>
        {
            { -1, new Lane(-1, roadId, Mathf.PI / 2 - leftLaneAngularOffset) },

            { 1, new Lane(1, roadId, Mathf.PI / 2 - rightLaneAngularOffset) },
        };

            return new Road(roadId, lanes);
        }


        private static List<Waypoint> GetPointsOnCurvedLane(int laneId, int roadId, float overflow,
            float circleRadius, float offset, float angleBetweenPoints)
        {
            var waypoints = new List<Waypoint>();

            while (overflow <= Mathf.PI / 2)
            {
                var x = (Mathf.Cos(Mathf.PI - overflow) + 1) * circleRadius + offset;

                var y = Mathf.Sin(Mathf.PI - overflow) * circleRadius;

                overflow += angleBetweenPoints;

                waypoints.Add(new Waypoint(x, y));
            }

            return waypoints;
        }

        private static Road TranslateRoadToGlobalCoords(Road road, Vector2 globalPosition)
        {
            road.Lanes.Values.ToList().ForEach((lane) =>
            {
                lane.Waypoints.ForEach((waypoint) => waypoint = new AStarWaypoint(waypoint.Location.X + globalPosition.x,
                                                                            waypoint.Location.Y + globalPosition.y));
            });

            return road;
        }

        public static (float x, float y) CarlaToUnity(float x, float y)
        {

            //Convert to Mouse Coordinates
            x = (x - -114.59522247314453f) / 4;
            y = (y - -68.72904205322266f) / 4 * (-1);


            //Handle Offset, so that 0,0 is in the middle
            x = x + -28.077075f;
            y = y + 26.24f;

            return (x, y);
        }


        //Only for Town06 later do as extension method for Vector3Ser or Location
        public static (float x, float y) UnityToCarla(float x, float y)
        {

            x = x + 28.077075f;
            y = y + -26.24f;

            x = x * 4;
            y = y * 4 * (-1);


            x = (x + -114.59522247314453f);
            y = (y + -68.72904205322266f);

            return (x, y);
        }

        public static float UnityRotToRadians(float rotation)
        {
            return (float)(Math.PI / 180 * -rotation);
        }

        public static double FastEuclideanDistance(Vector2 a, Vector2 b)
        {
            return Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2);
        }



    }

}