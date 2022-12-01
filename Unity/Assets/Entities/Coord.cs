using UnityEngine;

namespace Entity
{
    public class Location
    {
        public Location(Vector3 vector3, double rot)
        {
            Vector3 = vector3;
            Rot = rot;
        }

        public Location(float x, float y, float z, double rot)
        {
            Vector3 = new Vector3(x, y, z);
            Rot = rot;
        }

        public Vector3 Vector3 { get; set; }
        public double Rot { get; set; }

        
    }

}