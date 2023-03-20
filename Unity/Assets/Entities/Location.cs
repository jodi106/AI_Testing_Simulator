using UnityEngine;
using System;
using Assets.Helpers;

namespace Entity
{
    [Serializable]
    public class Location : ICloneable
    {
        public Location()
        {

        }

        public Location(Vector3 vector3)
        {
            Vector3Ser = new Vector3Ser(vector3);
            Rot = 0;
        }

        public Location(float x, float y)
        {
            Vector3Ser = new Vector3Ser(x, y);
        }

        public Location(Vector3 vector3, float rot)
        {
            Vector3Ser = new Vector3Ser(vector3);
            Rot = rot;
        }

        public Location(float x, float y, float z, float rot)
        {
            Vector3Ser = new Vector3Ser(x, y, z);
            Rot = rot;
        }

        public Vector3Ser Vector3Ser { get; set; }

        public float Rot { get; set; }

        public float X { get => Vector3Ser.X; }
        public float Y { get => Vector3Ser.Y; }
        public float Z { get => Vector3Ser.Z; }

        public object Clone()
        {
            return new Location(this.X, this.Y, this.Z, this.Rot);
        }
    }
}