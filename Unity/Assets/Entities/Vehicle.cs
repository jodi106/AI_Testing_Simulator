using Assets.Enums;

namespace Entity
{
    public class Vehicle : BaseEntity
    {
        private static int autoIncrementId = 0;
        public Vehicle() : base(++autoIncrementId) { }

        public Vehicle(Location spawnPoint, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(++autoIncrementId,spawnPoint, initialSpeed)
        {
            Category = category;
        }

        public Vehicle(Location spawnPoint, Path path, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(++autoIncrementId, spawnPoint, initialSpeed)
        {
            Path = path;
            Category = category;
        }

        public Vehicle(Location spawnPoint, EntityModel model, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(++autoIncrementId, spawnPoint, initialSpeed)
        {
            Model = model;
            Category = category;
        }


        public Vehicle(Location spawnPoint, EntityModel model, Path path, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(++autoIncrementId, spawnPoint, initialSpeed)
        {
            Model = model;
            Path = path;
            Category = category;

        }

        public void SetSpawnPoint(Location location)
        {
            SpawnPoint = location;
            View?.onChangePosition(SpawnPoint);
        }
        public IVehicleView View { get; set; }
        public EntityModel Model { get; set; }
        public VehicleCategory Category { get; set; }
        public Path Path { get; set; }
    }
}
