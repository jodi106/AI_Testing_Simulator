using Assets.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entity
{
    [Serializable]
    /// <summary>
    /// Represents a Path object that contains actions-info for a specific Entity created by Gui-User.
    /// </summary>
    public class Path : ICloneable// Story in .xosc
    {

        /// <summary>
        /// Initializes a new instance of the Path class with an empty WaypointList.
        /// </summary>
        public Path()
        {
            WaypointList = new List<Waypoint>();
        }

        /// <summary>
        /// Initializes a new instance of the Path class with the specified WaypointList, OverallStartTrigger, and OverallStopTrigger.
        /// </summary>
        /// <param name="waypointList">The list of waypoints in the path.</param>
        /// <param name="overallStartTrigger">A Waypoint object containing information regarding the overall start trigger of an act.</param>
        /// <param name="overallStopTrigger">A Waypoint object containing information regarding the overall stop trigger of an act.</param>
        public Path(List<Waypoint> waypointList, Waypoint overallStartTrigger = null, Waypoint overallStopTrigger = null)
        {
            OverallStartTrigger = overallStartTrigger;
            OverallStopTrigger = overallStopTrigger;
            WaypointList = waypointList;

            AssignRouteWaypoint = new Waypoint(
                    WaypointList[0].Location,
                    new ActionType("AssignRouteAction", GetRouteLocations()),
                    new List<TriggerInfo>() { new TriggerInfo("SimulationTimeCondition", 0, "greaterThan")});
            WaypointList.Insert(0,AssignRouteWaypoint);
        }

        public Waypoint OverallStartTrigger { get; set; } // A Waypoint Object containing Info regarding the overall start trigger of an act
        public Waypoint OverallStopTrigger { get; set; }
        public List<Waypoint> WaypointList { get; set; }

        public Waypoint AssignRouteWaypoint { get; set; } // is first waypoint of WaypointList to create OpenScenario Event AssignRouteAction and corresponding Trigger


        /// <summary>
        /// Creates a new Path object that is a deepcopy of the current instance.
        /// </summary>
        /// <returns>A new Path object that is a deepcopy of the current instance.</returns>
        public object Clone()
        {
            Path clonePath = new Path();

            clonePath.OverallStartTrigger = new Waypoint();
            if (this.OverallStartTrigger!= null) 
                clonePath.OverallStartTrigger = (Waypoint)this.OverallStartTrigger.Clone();
            
            clonePath.OverallStopTrigger = new Waypoint();
            if (this.OverallStopTrigger != null)
                clonePath.OverallStopTrigger = (Waypoint)this.OverallStopTrigger.Clone();

            clonePath.WaypointList = this.WaypointList.Select(x => (Waypoint)x.Clone()).ToList();

            return clonePath; 
        }


        /// <summary>
        /// Creates a list of all waypoint locations in the path to define the entity's route via an AssignRouteAction.
        /// </summary>
        /// <returns>A list of all waypoint locations in the path.</returns>
        public List<Location> GetRouteLocations()
        /// Creates a List of all Waypoint.Locations in the Path to define the Entities Route via a AssignRouteAction
        {
            return WaypointList.Where(w => w.ActionTypeInfo is not null).Select(w => w.Location).ToList();
        }

        public List<Location> GetAllLocations()
        {
            return WaypointList.Select(w => w.Location).ToList();
        }


        /// <summary>
        /// Determines if the WaypointList is empty or not.
        /// </summary>
        /// <returns>True if the WaypointList is empty, false otherwise.</returns>
        public bool IsEmpty()
        {
            return this.WaypointList.Count == 0;
        }

        /// <summary>
        /// Initializes the AssignRouteWaypoint at the end of the WaypointList with a Waypoint containing the ActionType "AssignRouteAction" 
        /// and all waypoint locations. Needs to be invoked at the end after WayPointList is finished (so when ExportButton is pressed).
        /// </summary>
        /// <param name="spawnpoint">The initial spawnpoint for the Entity.</param>
        public void InitAssignRouteWaypoint(Location spawnpoint, WaypointStrategy strategy)
        {
            // Calculate dummy waypoint coordinates 4m after spawn coordinate
            var first = WaypointList[0];
            Quaternion rotation = Quaternion.Euler(0f, 0f, spawnpoint.Rot);
            var originalStartLocation = (Location) first.Location.Clone();
            first.Location.Vector3Ser.SetFromVector3(first.Location.Vector3Ser.ToVector3() + rotation * Vector3.right);

            var locations = GetRouteLocations();
            locations.Insert(0, spawnpoint); // Insert spawn coodinate as waypoint to avoid having a AssignRouteAction with just 2 waypoints

            Waypoint assignRouteWaypoint = new Waypoint(
                    originalStartLocation,
                    new ActionType("AssignRouteAction", locations),
                    new List<TriggerInfo>() { new TriggerInfo("SimulationTimeCondition", 0, "greaterThan")}, strategy);

            AssignRouteWaypoint = assignRouteWaypoint;
            WaypointList.Insert(0, AssignRouteWaypoint); // Insert dummy waypoint after spawn waypoint
        }
    }
}
