using UnityEngine;
using System;

namespace Entity
{
    public class Location : ICloneable
    {
        public Location(Vector3 vector3)
        {
            Vector3 = vector3;
            Rot = 0;
        }

        public Location(Vector3 vector3, float rot)
        {
            Vector3 = vector3;
            Rot = rot;
        }

        public Location(float x, float y, float z, float rot)
        {
            Vector3 = new Vector3(x, y, z);
            Rot = rot;
        }

        public Location()
        {

        }
        public Vector3 Vector3 { get; set; }
        public float Rot { get; set; }

        public float X { get => Vector3.x; }
        public float Y { get => Vector3.y; }
        public float Z { get => Vector3.z; }

        public object Clone()
        {
            return new Location(new Vector3(X, Y, Z), this.Rot);
        }
    }
}