using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ExportScenario.Entities
{
    public class Ego : BaseEntity
    /// <summary>Create Ego vehicle. Object has no actions, only start point and destination</summary>
    /// ToDo: Implement AI driving
    {
        public Ego() : base() { }
        public Ego(Coord3D spawnPoint) : base(spawnPoint)
        {
        }
        public Ego(Coord3D spawnPoint, Coord3D destination) : base(spawnPoint)
        {
            Destination = destination;
        }

        public Ego(Coord3D spawnPoint, EntityModel model) : base(spawnPoint)
        {
            Model = model;            
        }

        public Ego(Coord3D spawnPoint, EntityModel model, double initialSpeed) : base(spawnPoint, initialSpeed)
        {
            Model = model;
        }

        public Ego(Coord3D spawnPoint, EntityModel model, double initialSpeed, Coord3D destination) : base(spawnPoint, initialSpeed)
        {
            Model = model;
            Destination = destination;
        }
        public EntityModel Model { get; set; }
        public Coord3D Destination { get; set; }
    }
}
