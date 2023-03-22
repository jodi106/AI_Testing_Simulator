using UnityEngine;
using System;
using Assets.Helpers;

namespace Entity
{
    [Serializable]
    /// <summary>
    /// A class representing a location in 3D space and rotation.
    /// </summary>
    /// <remarks>
    /// This class implements the <see cref="ICloneable"/> interface to provide a way to create a deep copy of an instance of this class.
    /// </remarks>
    public class Location : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class with a specified position and a rotation of 0.
        /// </summary>
        /// <param name="vector3">The position in 3D space represented as a <see cref="Vector3"/>.</param>
        public Location(Vector3 vector3)
        {
            Vector3Ser = new Vector3Ser(vector3);
            Rot = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class with a specified position and rotation.
        /// </summary>
        /// <param name="vector3">The position in 3D space represented as a <see cref="Vector3"/>.</param>
        /// <param name="rot">The rotation around the y-axis.</param>
        public Location(Vector3 vector3, float rot)
        {
            Vector3Ser = new Vector3Ser(vector3);
            Rot = rot;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class with a specified position and rotation.
        /// </summary>
        /// <param name="x">The x-coordinate of the position in 3D space.</param>
        /// <param name="y">The y-coordinate of the position in 3D space.</param>
        /// <param name="z">The z-coordinate of the position in 3D space.</param>
        /// <param name="rot">The rotation around the y-axis.</param>
        public Location(float x, float y, float z, float rot)
        {
            Vector3Ser = new Vector3Ser(x, y, z);
            Rot = rot;
        }

        /// <summary>
        /// Initializes a new empty instance of the <see cref="Location"/> class.
        /// </summary>
        public Location()
        {

        }

        public Vector3Ser Vector3Ser { get; set; }
        public float Rot { get; set; }

        public float X { get => Vector3Ser.X; }
        public float Y { get => Vector3Ser.Y; }
        public float Z { get => Vector3Ser.Z; }


        /// <summary>
        /// Creates a deep copy of this instance of the <see cref="Location"/> class.
        /// </summary>
        /// <returns>A new instance of the <see cref="Location"/> class that is a deepcopy of this instance.</returns>
        public object Clone()
        {
            return new Location(this.X, this.Y, this.Z, this.Rot);
        }
    }
}