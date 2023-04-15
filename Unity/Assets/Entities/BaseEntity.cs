using Assets.Helpers;
using System;
using System.Xml.Serialization;
using UnityEngine;

namespace Entity
{
    [Serializable]
    public class BaseEntity
    /// <summary>Creates BaseEntity Object which contains Coord3D SpawnPoint for entities (Veh, Ped)</summary>
    {
        public BaseEntity(string id, Location spawnPoint, double initialSpeedKMH)
        {
            Id = id;
            SpawnPoint = spawnPoint;
            InitialSpeedKMH = initialSpeedKMH;
        }

        public BaseEntity()
        {

        }

        private string id;
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

        public void setPosition(float x, float y)
        {
            if (SpawnPoint.X != x || SpawnPoint.Y != y)
            {
                SpawnPoint = new Location(x, y, SpawnPoint.Z, SpawnPoint.Rot);
                View?.onChangePosition(x, y);
            }
        }

        public void setRotation(float angle)
        {
            if (SpawnPoint.Rot != angle)
            {
                SpawnPoint = new Location(SpawnPoint.X, SpawnPoint.Y, SpawnPoint.Z, angle);
                View?.onChangeRotation(angle);
            }
        }

        public void setView(IBaseEntityView view)
        {
            this.View = view;
        }

        public void setColor(Color c)
        {
            this.Color = new ColorSer(c);
            
            this.View?.onChangeColor(c);
        }

        public Location getCarlaLocation()
        {
            (float xCarla, float yCarla) = SnapController.UnityToCarla(SpawnPoint.X, SpawnPoint.Y);
            float rotCarla = SnapController.UnityRotToRadians(SpawnPoint.Rot);
            rotCarla = (float)Math.Round(rotCarla * 100f) / 100f; // otherwise we'll have a number like this 3.339028E-05
            return new Location(xCarla, yCarla, 0.3f, rotCarla);
        }
    }
}