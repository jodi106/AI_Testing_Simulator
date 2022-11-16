using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class Vehicle : BaseEntity
    /// <summary>Create Vehicle Object. Contains all Vehicle-Entity specific info created by Gui-User.</summary>
    {
        private static int autoIncrementId = 1;
        public Vehicle() : base() { }

        // TODO auto increment id
        public Vehicle(Coord3D spawnPoint, EntityModel model, Path path, double initialSpeed) : base(spawnPoint, initialSpeed)
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
