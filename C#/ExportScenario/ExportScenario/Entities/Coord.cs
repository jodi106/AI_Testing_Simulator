using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Coord
    {
        public Coord(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Coord3D : Coord
    /// <summary>Create Coord3D object corresponsing to global Carla coordinates and rotation</summary>
    {
        public Coord3D(double x, double y, double z, double rot) : base(x, y)
        {
            Z = z;
            Rot = rot;
        }

        public double Z { get; set; }
        public double Rot { get; set; }
    }

}
