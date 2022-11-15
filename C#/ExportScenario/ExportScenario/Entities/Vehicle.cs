using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Vehicle : BaseEntity
    {
        private static int autoIncrementId = 1;
        public Vehicle() : base() { }

        // TODO auto increment id
        public Vehicle(Coord3D spawnPoint, EntityModel model, Path path, double initialSpeed) : base(spawnPoint)
        {
            Id = autoIncrementId++;
            Model = model;
            Path = path;
            InitialSpeed = initialSpeed;
        }

        public int Id { get; set; }
        public EntityModel Model { get; set; }
        public Path Path { get; set; }
        public double InitialSpeed { get; set; }    
    }
}
