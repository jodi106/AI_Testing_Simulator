using Assets.Enums;

namespace Models
{
    public class Vehicle : BaseModel
    {
        public Vehicle() : base()
        {
        }

        public Vehicle(Location spawnPoint, VehicleCategory category, Path path) : base(spawnPoint)
        {
            Category = category;
            Path = path;
        }

        public Vehicle(int id, Location spawnPoint, VehicleCategory category, Path path) : base(id, spawnPoint)
        {
            Category = category;
            Path = path;
        }

        public Vehicle(Location spawnPoint, Model model, VehicleCategory category, Path path) : base(spawnPoint)
        {
            Model = model;
            Category = category;
            Path = path;
        }

        public Vehicle(int id, Location spawnPoint, Model model, VehicleCategory category, Path path) : base(id, spawnPoint)
        {
            Model = model;
            Category = category;
            Path = path;
        }

        public void setPosition(float x, float y)
        {
            SpawnPoint = new Location(x, y, 0, 0);
            View?.onChangePosition(SpawnPoint);
        }

        public IVehicleView View { get; set; }
        public Model Model { get; set; }
        public VehicleCategory Category { get; set; }
        public Path Path { get; set; }
    }
}
