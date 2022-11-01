using Assets.Enums;

namespace Models
{
    public class Ego : BaseModel
    {
        public Ego(Location spawnPoint, VehicleCategory category) : base(spawnPoint)
        {
            Category = category;
        }

        public Ego(int id, Location spawnPoint, VehicleCategory category) : base(id, spawnPoint)
        {
            Category = category;
        }

        public Ego(Location spawnPoint, Model model, VehicleCategory category) : base(spawnPoint)
        {
            Model = model;
            Category = category;
        }

        public Ego(int id, Location spawnPoint, Model model, VehicleCategory category) : base(id, spawnPoint)
        {
            Model = model;
            Category = category;
        }

        public Model Model { get; set; }
        public VehicleCategory Category { get; set; }
    }
}
