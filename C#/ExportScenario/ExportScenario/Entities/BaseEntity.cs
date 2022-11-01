using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class BaseEntity
    {
        private static int autoIncrementId = 0;
        public BaseEntity()
        {
            Id = autoIncrementId++;
        }

        public BaseEntity(Coord3D spawnPoint)
        {
            Id = autoIncrementId++;
            SpawnPoint = spawnPoint;
        }

        public BaseEntity(int id, Coord3D spawnPoint)
        {
            Id = id;
            SpawnPoint = spawnPoint;
        }

        public int Id { get; set; }
        public Coord3D SpawnPoint { get; set; }
    }
}
