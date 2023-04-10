using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using Assets.Enums;

namespace Entity
{
    [Serializable]
    /// <summary>
    /// Create Waypoint Object. Contains User defined Input for a specific Event on a Entity Path Represents an Event in the .xosc File
    /// </summary>
    public class Waypoint : ICloneable // Event in .xosc
    {

        public Location Location { get; private set; } // coords of unity editor GUI
        public Location LocationCarla { get; set; } // coords of Carla. Used in Export xosc.
        public ActionType ActionTypeInfo { get; set; }
        public string Priority { get; set; } // has enum: PriorityType
        public List<TriggerInfo> TriggerList { get; set; }
        // One Waypoint can have mutliple triggers for an event

        [field: NonSerialized]
        public IBaseView View { get; set; }
        public List<ActionType> Actions { get; set; } // Actions without Triggers
        public Adversary StartRouteOfOtherVehicle { get; set; }
        public WaypointStrategy Strategy { get; set; }


        /// <summary>
        /// Initializes a new empty instance of the Waypoint class
        /// </summary>
        public Waypoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Waypoint class with a given location, action type info, trigger list, and priority
        /// </summary>
        /// <param name="location">The location of the Waypoint</param>
        /// <param name="actionTypeInfo">The ActionType info of the Waypoint</param>
        /// <param name="triggerList">The list of triggers of the Waypoint</param>
        /// <param name="priority">The priority of the Waypoint</param>
        public Waypoint(Location location, ActionType actionTypeInfo, List<TriggerInfo> triggerList, WaypointStrategy strategy = WaypointStrategy.FASTEST)
        {
            Location = location;
            ActionTypeInfo = actionTypeInfo;
            TriggerList = triggerList;
            CalculateLocationCarla();
            Actions = new List<ActionType>();
            Strategy = strategy;
        }

        /// <summary>
        /// Sets the position of the Waypoint to the given coordinates
        /// </summary>
        /// <param name="x">The x-coordinate of the Waypoint</param>
        /// <param name="y">The y-coordinate of the Waypoint</param>
        public void setPosition(float x, float y)
        {
            Location = new Location(x, y, Location.Z, Location.Rot);
            this.View?.onChangePosition(x, y);
            CalculateLocationCarla();
        }
        /// <summary>
        /// Calculates the location of the Waypoint in Carla given the corresponding Unity Coordinates and fills the LocationCarla property.
        /// </summary>
        public void CalculateLocationCarla()
        {
            (float xCarla, float yCarla) = SnapController.UnityToCarla(Location.X, Location.Y);
            float rotCarla = SnapController.UnityRotToRadians(Location.Rot);
            rotCarla = (float)Math.Round(rotCarla * 100f) / 100f; // otherwise we'll have a number like this 3.339028E-05
            this.LocationCarla = new Location(xCarla, yCarla, 0.3f, rotCarla);
        }

        /// <summary>
        /// Clones the Waypoint object
        /// </summary>
        /// <returns>A cloned Waypoint object</returns>
        public object Clone()
        {
            var cloneWaypoint = new Waypoint();
            if (this.Location != null) cloneWaypoint.Location = (Location)this.Location.Clone();
            if (this.LocationCarla != null) cloneWaypoint.LocationCarla = (Location)this.LocationCarla.Clone();
            if (this.ActionTypeInfo != null) cloneWaypoint.ActionTypeInfo = (ActionType)this.ActionTypeInfo.Clone();
            if (this.TriggerList != null) cloneWaypoint.TriggerList = this.TriggerList.Select(x => (TriggerInfo)x.Clone()).ToList();

            try
            {
                if (this.StartRouteOfOtherVehicle != null) cloneWaypoint.StartRouteOfOtherVehicle = (Adversary)this.StartRouteOfOtherVehicle.Clone();
            }
            catch (StackOverflowException e)
            {
                WarningPopupController warningPopupController = GameObject.Find("PopUps").transform.Find("WarningPopUp").gameObject.GetComponent<WarningPopupController>();
                warningPopupController.gameObject.SetActive(true);
                warningPopupController.open("Illogical behavior", "Your adversary vehicles probably won't start.\nThere's a contradiction in their startPath trigger (Circle).");
            }

            cloneWaypoint.Actions = new();
            if (this.Actions != null)
                cloneWaypoint.Actions = this.Actions.Select(x => (ActionType)x.Clone()).ToList();

            cloneWaypoint.Strategy = Strategy;

            return cloneWaypoint;
        }
    }
}