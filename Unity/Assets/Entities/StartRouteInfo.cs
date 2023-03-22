using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]

    /// <summary>
    /// Class representing information needed to start a route.
    /// </summary>
    public class StartRouteInfo
    {

        public Adversary Vehicle { get; set; } // this vehicle is Trigger to start my route
        public Location LocationCarla { get; private set; }
        public int Time { get; set; }
        public int Distance { get; set; }
        public Ego EgoVehicle { get; private set; }

        public string Type { get; private set; } // Waypoint, Time, Ego

        /// <summary>
        /// Initializes a new instance of the <see cref="StartRouteInfo"/> class with a waypoint type.
        /// </summary>
        /// <param name="vehicle">The adversary vehicle that triggers the route (Distance based Start).</param>
        /// <param name="waypoint">The waypoint location where the route should start.</param>
        /// <param name="distance">The distance in meters between the vehicle and the waypoint (default is 5).</param>
        /// <param name="type">The type of start route information (default is "Waypoint").</param>
        public StartRouteInfo(Adversary vehicle, Waypoint waypoint, int distance = 5, string type = "Waypoint")
        {
            this.Vehicle = vehicle;
            this.LocationCarla = waypoint.Location;
            this.Distance = distance;
            this.Type = "Waypoint";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartRouteInfo"/> class with a time type (Time based Start).
        /// </summary>
        /// <param name="vehicle">The adversary vehicle that triggers the route.</param>
        /// <param name="time">The time in seconds when the route should start.</param>
        /// <param name="type">The type of start route information (default is "Time").</param>
        public StartRouteInfo(Adversary vehicle, int time, string type = "Time")
        {
            this.Vehicle = vehicle;
            this.Time = time;
            this.Type = "Time";
        }



        /// <summary>
        /// Initializes a new instance of the StartRouteInfo class with the specified parameters and the "Ego" Type (For Ego Vehicle).
        /// </summary>
        /// <param name="vehicle">The Adversary vehicle that will be part of the route.</param>
        /// <param name="location">The starting location for the route.</param>
        /// <param name="distance">The distance of the route in meters.</param>
        /// <param name="egoVehicle">The Ego vehicle that will be part of the route.</param>
        /// <param name="type">The type of the route. Default is "Ego".</param>
        /// <returns>A new instance of the StartRouteInfo class.</returns>
        public StartRouteInfo(Adversary vehicle, Location location, int distance, Ego egoVehicle, string type = "Ego")
        {
            this.Vehicle = vehicle;
            this.LocationCarla = location;
            this.Distance = distance;
            this.EgoVehicle = egoVehicle;
            this.Type = "Ego";
        }


        /// <summary>
        /// returns true if the type of the start route information is "Waypoint".
        /// </summary>
        /// <returns>True if the type of the start route information is "Waypoint", false otherwise.</returns>
        public bool isTypeWaypoint()
        {
            return this != null && this.Type == "Waypoint";
        }

        /// <summary>
        /// returns true if the type of the start route information is "Time" or "Ego" (or not Waypoint)
        /// </summary>
        /// <returns>True if the type of the start route information is NOT "Waypoint", false otherwise.</returns>
        public bool isTypeTimeOrEgo()
        {
            return this != null && this.Type != "Waypoint";
        }
    }
}
