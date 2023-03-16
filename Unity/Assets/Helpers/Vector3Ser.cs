using System;
using UnityEngine;

namespace Assets.Helpers
{

    //Vector3Ser Class partly compatible with UnityEngine.Vector3Ser, that is Serializable
    [Serializable]
    public class Vector3Ser
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3Ser(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }


        public Vector3Ser(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3Ser(Vector3 vector3)
        {
            this.X = vector3.x;
            this.Y = vector3.y;
            this.Z = vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(this.X, this.Y, this.Z);
        }

        public void SetFromVector3(Vector3 vector3)
        {
            {
                this.X = vector3.x;
                this.Y = vector3.y;
                this.Z = vector3.z;
            }
        }
    }
    
}
