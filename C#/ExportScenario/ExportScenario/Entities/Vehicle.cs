using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Vehicle : BaseEntity
    {
        public Vehicle() : base() { }

        // TODO auto increment id
        public Vehicle(int id, Coord3D spawnPoint, EntityModel model, Path path, double initialSpeed) : base(id, spawnPoint)
        {
            Model = model;
            Path = path;
            InitialSpeed = initialSpeed;
        }

        public EntityModel Model { get; set; }
        public Path Path { get; set; }
        public double InitialSpeed { get; set; }    
    }
}
