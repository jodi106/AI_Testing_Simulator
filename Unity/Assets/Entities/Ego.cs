using Assets.Enums;

namespace Entity
{
    public class Ego : BaseEntity
    /// <summary>Create Ego vehicle. Object has no actions, only start point and destination</summary>
    /// ToDo: Implement AI driving
    {
        public Ego() : base() { }

        public Ego(Location spawnPoint, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(spawnPoint, initialSpeed)
        {
            Category = category;
        }

        public Ego(Location spawnPoint, Location destination, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(spawnPoint, initialSpeed)
        {
            Destination = destination;
            Category = category;
        }

        public Ego(Location spawnPoint, EntityModel model, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(spawnPoint, initialSpeed)
        {
            Model = model;
            Category = category;
        }


        public Ego(Location spawnPoint, Location destination, EntityModel model, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(spawnPoint, initialSpeed)
        {
            Model = model;
            Destination = destination;
            Category = category;
        }
        public Location Destination { get; set; }
        public EntityModel Model { get; set; }
        public VehicleCategory Category { get; set; }
    }
}
