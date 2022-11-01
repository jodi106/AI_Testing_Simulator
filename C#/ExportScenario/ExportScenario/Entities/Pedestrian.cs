using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Pedestrian : BaseEntity
    {
        public Pedestrian() : base() { }
        public Pedestrian(Coord3D spawnPoint, Path path) : base(spawnPoint)
        {
            Path = path;
        }

        public Pedestrian(int id, Coord3D spawnPoint, Path path) : base(id, spawnPoint)
        {
            Path = path;
        }

        public Pedestrian(Coord3D spawnPoint, EntityModel model, Path path) : base(spawnPoint)
        {
            Model = model;
            Path = path;
        }

        public Pedestrian(int id, Coord3D spawnPoint, EntityModel model, Path path) : base(id, spawnPoint)
        {
            Model = model;
            Path = path;
        }

        public EntityModel Model { get; set; }
        public Path Path { get; set; }
    }
}
