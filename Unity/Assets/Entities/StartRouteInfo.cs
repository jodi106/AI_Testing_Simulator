using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class StartRouteInfo
    {

        public SimulationEntity Vehicle { get; set; }
        public Location LocationCarla { get; private set; }
        public int Time { get; set; }
        public int Distance { get; set; }
        public Ego EgoVehicle { get; private set; }

        public string Type { get; private set; } // Waypoint, Time, Ego

        public StartRouteInfo(SimulationEntity vehicle, Waypoint waypoint, int distance = 5, string type = "Waypoint")
        {
            this.Vehicle = vehicle;
            this.LocationCarla = waypoint.Location;
            this.Distance = distance;
            this.Type = "Waypoint";
        }

        public StartRouteInfo(SimulationEntity vehicle, int time, string type = "Time")
        {
            this.Vehicle = vehicle;
            this.Time = time;
            this.Type = "Time";
        }

        public StartRouteInfo(Vehicle vehicle, Location location, int distance, Ego egoVehicle, string type = "Ego")
        {
            this.Vehicle = vehicle;
            this.LocationCarla = location;
            this.Distance = distance;
            this.EgoVehicle = egoVehicle;
            this.Type = "Ego";
        }

        public bool isTypeWaypoint()
        {
            return this != null && this.Type == "Waypoint";
        }

        public bool isTypeTimeOrEgo()
        {
            return this != null && this.Type != "Waypoint";
        }
    }
}
