using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
        }

        public BaseEntity(Coord3D spawnPoint)
        {
            SpawnPoint = spawnPoint;
        }

        public BaseEntity(int id, Coord3D spawnPoint)
        {
            SpawnPoint = spawnPoint;
        }

        public Coord3D SpawnPoint { get; set; }
    }
}
