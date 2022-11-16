using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class BaseEntity
    /// <summary>Creates BaseEntity Object which contains Coord3D SpawnPoint for entities (Veh, Ped)</summary>
    {
        public BaseEntity()
        {
        }

        public BaseEntity(Coord3D spawnPoint)
        {
            SpawnPoint = spawnPoint;
        }

        public BaseEntity(Coord3D spawnPoint, double initialSpeed)
        {
            SpawnPoint = spawnPoint;
            InitialSpeed = initialSpeed;
        }

        public Coord3D SpawnPoint { get; set; }
        public double InitialSpeed { get; set; }
    }
}
