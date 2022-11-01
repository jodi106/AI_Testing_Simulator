using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Ego : BaseEntity
    {
        public Ego() : base() { }
        public Ego(Coord3D spawnPoint) : base(spawnPoint)
        {
        }

        public Ego(int id, Coord3D spawnPoint) : base(id, spawnPoint)
        {
        }

        public Ego(Coord3D spawnPoint, EntityModel model) : base(spawnPoint)
        {
            Model = model;
        }

        public Ego(int id, Coord3D spawnPoint, EntityModel model) : base(id, spawnPoint)
        {
            Model = model;
        }

        public EntityModel Model { get; set; }

    }
}
