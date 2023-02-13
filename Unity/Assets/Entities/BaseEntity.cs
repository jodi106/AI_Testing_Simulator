using System;
using UnityEngine;

namespace Entity
{
    public class BaseEntity
    /// <summary>Creates BaseEntity Object which contains Coord3D SpawnPoint for entities (Veh, Ped)</summary>
    {
        public BaseEntity(string id, Location spawnPoint, double initialSpeed)
        {
            Id = id;
            SpawnPoint = spawnPoint;
            InitialSpeed = initialSpeed;
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

        public double InitialSpeed { get; set; }

        public Color Color { get; protected set; }

        public IBaseEntityView View { get; set; }

        public void setSpawnPoint(Location pos)
        {
            SpawnPoint = pos;
            View?.onChangePosition(SpawnPoint);
        }
        public void setView(IBaseEntityView view)
        {
            this.View = view;
        }

        public void setColor(Color c)
        {
            this.Color = c;
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