using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class StartRouteInfo
    {

        public Vehicle vehicle { get; set; }
        public Location locationCarla { get; private set; }

        public StartRouteInfo(Vehicle vehicle, Waypoint waypoint)
        {
            this.vehicle = vehicle;
            this.locationCarla = waypoint.Location;
            //this.locationCarla = CalculateLocationCarla(waypoint.Location);
        }

        public StartRouteInfo(Vehicle vehicle, Location location)
        {
            this.vehicle = vehicle;
            this.locationCarla = location;
            //this.locationCarla = CalculateLocationCarla(location);
        }

        private Location CalculateLocationCarla(Location pos)
        {
            (float xCarla, float yCarla) = SnapController.UnityToCarla(pos.X, pos.Y);
            float rotCarla = SnapController.UnityRotToRadians(pos.Rot);
            rotCarla = (float)Math.Round(rotCarla * 100f) / 100f; // otherwise we'll have a number like this 3.339028E-05
            return new Location(xCarla, yCarla, 0.3f, rotCarla);
        }
    }
}
