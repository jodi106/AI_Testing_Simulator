using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Pedestrian : BaseEntity
    /// <summary>Creates Pedestrian Object. Contains all Pedestiran-Entity specific info created by Gui-User.</summary>
    {
        private static int autoIncrementId = 1;
        public Pedestrian() : base() { }
        public Pedestrian(Coord3D spawnPoint, Path path) : base(spawnPoint)
        {
            Id = autoIncrementId++;
            Path = path;
        }

        public Pedestrian(Coord3D spawnPoint, EntityModel model, Path path) : base(spawnPoint)
        {
            Id = autoIncrementId++;
            Model = model;
            Path = path;
        }

        public Pedestrian(Coord3D spawnPoint, EntityModel model, Path path, double initialSpeed) : base(spawnPoint, initialSpeed)
        {
            Id = autoIncrementId++;
            Model = model;
            Path = path;
        }

        public int Id { get; set; }
        public EntityModel Model { get; set; }
        public Path Path { get; set; }
    }
}
