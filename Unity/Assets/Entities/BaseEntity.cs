using Assets.Helpers;
using System;
using System.Xml.Serialization;
using UnityEngine;

namespace Entity
{
    [Serializable]
    /// <summary>Creates BaseEntity Object which contains Coord3D SpawnPoint for entities (Veh, Ped)</summary>

    public class BaseEntity
    {
        

        /// <summary>
        /// Creates a new BaseEntity object with an Id, SpawnPoint, and initial speed.
        /// </summary>
        /// <param name="id">The unique identifier of the BaseEntity object.</param>
        /// <param name="spawnPoint">The Coord3D spawn point of the BaseEntity object.</param>
        /// <param name="initialSpeedKMH">The initial speed of the BaseEntity object in kilometers per hour.</param>
        public BaseEntity(string id, Location spawnPoint, double initialSpeedKMH)
        {
            Id = id;
            SpawnPoint = spawnPoint;
            InitialSpeedKMH = initialSpeedKMH;
        }


        /// <summary>
        /// Creates a new BaseEntity object with empty values.
        /// </summary>
        public BaseEntity()
        {

        }

        private string id;


        /// <summary>
        /// The unique identifier of the BaseEntity object.
        /// </summary>
        public string Id
        {
            get => id; set
            {
                id = value;
                View?.onChangeID(value);
            }
        }
        public Location SpawnPoint { get; set; }
        public double InitialSpeedKMH { get; set; }

        public ColorSer Color;

        [field: NonSerialized]
        public IBaseEntityView View { get; set; }



        /// <summary>
        /// Sets the position of the BaseEntity object.
        /// </summary>
        /// <param name="x">The x coordinate of the new position.</param>
        /// <param name="y">The y coordinate of the new position.</param>
        public void setPosition(float x, float y)
        {
            if (SpawnPoint.X != x || SpawnPoint.Y != y)
            {
                SpawnPoint = new Location(x, y, SpawnPoint.Z, SpawnPoint.Rot);
                View?.onChangePosition(x, y);
            }
        }


        /// <summary>
        /// Sets the rotation angle of the BaseEntity object.
        /// </summary>
        /// <param name="angle">The new rotation angle in degrees.</param>    
        public void setRotation(float angle)
        {
            if (SpawnPoint.Rot != angle)
            {
                SpawnPoint = new Location(SpawnPoint.X, SpawnPoint.Y, SpawnPoint.Z, angle);
                View?.onChangeRotation(angle);
            }
        }

        /// <summary>
        /// Sets the view of the BaseEntity object.
        /// </summary>
        /// <param name="view">The new view of the BaseEntity object.</param>
        public void setView(IBaseEntityView view)
        {
            this.View = view;
        }


        /// <summary>
        /// Sets the color of the BaseEntity object.
        /// </summary>
        /// <param name="c">The new color of the BaseEntity object.</param>
        public void setColor(Color c)
        {
            this.Color = new ColorSer(c);
            
            this.View?.onChangeColor(c);
        }



        /// <summary>
        /// Returns the location of the BaseEntity object in CARLA coordinates.
        /// </summary>
        /// <returns>The location of the BaseEntity object in CARLA coordinates.</returns>
        public Location getCarlaLocation()
        {
            (float xCarla, float yCarla) = SnapController.UnityToCarla(SpawnPoint.X, SpawnPoint.Y);
            float rotCarla = SnapController.UnityRotToRadians(SpawnPoint.Rot);
            rotCarla = (float)Math.Round(rotCarla * 100f) / 100f; // otherwise we'll have a number like this 3.339028E-05
            // Round coordinates important to avoid strange behavior in Carla
            xCarla = (float)Math.Round(xCarla, 2);
            yCarla = (float)Math.Round(yCarla, 2);
            return new Location(xCarla, yCarla, 0.3f, rotCarla);
        }
    }
}