using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using System.ComponentModel;
using System.Linq.Expressions;
using System;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            lane.Waypoints.ForEach((waypoint) => waypoint = new Waypoint(waypoint.Location.X + globalPosition.x,
                                                                        waypoint.Location.Y + globalPosition.y));
        });

        return road;
    }
}
