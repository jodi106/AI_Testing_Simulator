using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Helpers
{

    /// <summary>
    /// Vector3Ser Class partly compatible with UnityEngine.Vector3Ser, that is Serializable.
    /// </summary>
    [Serializable]
    public class Vector3Ser
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        

        /// <summary>
        /// Initializes a new instance of the Vector3Ser class with the specified x, y, and z values.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="z">The z value.</param>
        public Vector3Ser(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the Vector3Ser class with the specified Vector3.
        /// </summary>
        /// <param name="vector3">The Vector3 to initialize from.</param>
        public Vector3Ser(Vector3 vector3)
        {
            this.X = vector3.x;
            this.Y = vector3.y;
            this.Z = vector3.z;
        }

        /// <summary>
        /// Converts this Vector3Ser to a Vector3.
        /// </summary>
        /// <returns>The Vector3 representation of this Vector3Ser.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Sets the x, y, and z values of this Vector3Ser from the specified Vector3.
        /// </summary>
        /// <param name="vector3">The Vector3 to set the values from.</param>
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
