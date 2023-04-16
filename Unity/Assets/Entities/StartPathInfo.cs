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
    public class StartPathInfo
    {

        public Adversary Vehicle { get; set; } // this vehicle is Trigger to start my route
        public Location LocationCarla { get; private set; }
        public int Time { get; set; }
        public int Distance { get; set; }
        public Ego EgoVehicle { get; private set; }

        public string Type { get; private set; } // Waypoint, Time, Ego

        /// <summary>
        /// Initializes a new instance of the <see cref="StartPathInfo"/> class with a waypoint type.
        /// </summary>
        /// <param name="vehicle">The adversary vehicle that triggers the route start. (Distance based Start).</param>
        /// <param name="waypoint">The waypoint location (of adversary vehicle) that triggers the route start.</param>
        /// <param name="distance">The distance in meters to the vehicle must be lower than this value to trigger the route start. (default is 5).</param>
        /// <param name="type">The type of start path information (Default is "Waypoint" and shouldn't be changed).</param>
        public StartPathInfo(Adversary vehicle, Waypoint waypoint, int distance = 5, string type = "Waypoint")
        {
            this.Vehicle = vehicle;
            this.LocationCarla = waypoint.Location;
            this.Distance = distance;
            this.Type = "Waypoint";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartPathInfo"/> class with a time type (Time based Start).
        /// </summary>
        /// <param name="vehicle">This value is not used for the type "Time". It can be a dummy vehicle in this case.</param>
        /// <param name="time">The time in seconds when the route should start.</param>
        /// <param name="type">The type of start path information. (Default is "Time and shouldn't be changed").</param>
        public StartPathInfo(Adversary vehicle, int time, string type = "Time") // TODO remove vehicle in constructor and fix NullPointerExceptions, TODO make type not changable (remove as a parameter)
        {
            this.Vehicle = vehicle;
            this.Time = time;
            this.Type = "Time";
        }

        /// <summary>
        /// Initializes a new instance of the StartPathInfo class with the specified parameters and the "Ego" Type (For Ego Vehicle).
        /// </summary>
        /// <param name="vehicle">This value is not used for the type "Time". It can be a dummy vehicle in this case.</param>
        /// <param name="location">The location the ego vehicle must reach to trigger the route start.</param>
        /// <param name="distance">The distance in meters to the ego vehicle must be lower than this value to trigger the route start.</param>
        /// <param name="egoVehicle">The Ego vehicle that triggers the route start (Distance based).</param>
        /// <param name="type">The type of start path information. (Default is "Ego" and shouldn't be changed).</param>
        /// <returns>A new instance of the StartPathInfo class.</returns>
        public StartPathInfo(Adversary vehicle, Location location, int distance, Ego egoVehicle, string type = "Ego") // TODO remove vehicle in constructor and fix NullPointerExceptions
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
