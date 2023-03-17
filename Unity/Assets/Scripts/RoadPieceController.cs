using System.Collections.Generic;
using UnityEngine;
using Entity;
using System.ComponentModel;
using System.Linq;

public enum RoadType
{
    [Description("StraightRoad")]
    StraightRoad,
    [Description("Turn")]
    Turn,
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
    void Update()
    {
        
    }

    void Align(RoadPiece current, Dictionary<RoadPiece, Vector2> pieces)
    {
        RoadPiece neighbor = null;
        if (pieces.Count > 0)
        {
            neighbor = getClosestPiece(pieces, current);
            print(neighbor.position.x);
            if (neighbor != null)
            {
                Vector2 newPos = CalcPositionRelativeToNeighbor(neighbor);
                PositionPiece(current, newPos, neighbor.rotation);
            }
        }
    }

    /**
     * changes the position of the piece current to the given position
     */
    void PositionPiece(RoadPiece current, Vector2 position, double rotation)
    {
        transform.position = Vector2.MoveTowards(current.transform.position, position, 100000000);
    }

    /**
     * calculates the position of the piece, which should be draged to the map
     * based on position of the next closest road piece
     */
    Vector2 CalcPositionRelativeToNeighbor(RoadPiece neighbor)
    {
        Vector2 position;
        position.x = neighbor.transform.position.x;
        position.y = neighbor.transform.position.y - 1;

        //position.x = Mathf.Cos(rotation) * radius;
        //position.y = Mathf.Sin(rotation) * radius;



        return position;
    }

    /**
     * searches the closest road piece to the moving piece by comparing the euclidian distance
     */
    RoadPiece getClosestPiece(Dictionary<RoadPiece, Vector2> pieces, RoadPiece current)
    {
        RoadPiece closest = null;
        Dictionary<RoadPiece, float> distances = new Dictionary<RoadPiece, float>();
        Vector2 position;
        float positionX;
        float positionY;
        float eukl_dist;
        foreach (KeyValuePair<RoadPiece, Vector2> piece in pieces)
        {
            position = piece.Value;
            positionX = position.x;
            positionY = position.y;
            eukl_dist = Mathf.Sqrt((float)SnapController.FastEuclideanDistance(position, current.position));
            distances.Add(piece.Key, eukl_dist);
        };
        float min_dist = int.MaxValue;
        foreach (KeyValuePair<RoadPiece, float> distance in distances)
        {
            if (min_dist < distance.Value)
            {
                min_dist = distance.Value;
                closest = distance.Key;
            }
        };
        if (min_dist > 10)
        {
            closest = null;
        }

        return closest;
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
            var x = (Mathf.Cos(Mathf.PI - overflow)+1) * circleRadius + offset;

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



}
