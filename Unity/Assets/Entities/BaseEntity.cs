using System;
using UnityEngine;

namespace Entity
{
    public class BaseEntity
    /// <summary>Creates BaseEntity Object which contains Coord3D SpawnPoint for entities (Veh, Ped)</summary>
    {
        public BaseEntity(int id, Location spawnPoint, double initialSpeed)
        {
            Id = id.ToString();
            SpawnPoint = spawnPoint;
            InitialSpeed = initialSpeed;
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
        public Location SpawnPointCarla { get; set; }

        public double InitialSpeed { get; set; }

        public Color color { get; protected set; }

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
            this.color = c;
            this.View?.onChangeColor(c);
        }

        public void CalculateLocationCarla()
        {
            (float xCarla, float yCarla) = SnapController.UnityToCarla(SpawnPoint.X, SpawnPoint.Y);
            float rotCarla = SnapController.UnityRotToRadians(SpawnPoint.Rot);
            rotCarla = (float)Math.Round(rotCarla * 100f) / 100f; // otherwise we'll have a number like this 3.339028E-05
            this.SpawnPointCarla = new Location(xCarla, yCarla, 0.3f, rotCarla);
        }
    }
}