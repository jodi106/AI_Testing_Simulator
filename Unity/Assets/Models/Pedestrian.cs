using Assets.Enums;

namespace Models
{
    public class Pedestrian : BaseModel
    {

        public Pedestrian(Location spawnPoint, PedestrianType type, Path path) : base(spawnPoint)
        {
            Type = type;
            Path = path;
        }

        public Pedestrian(int id, Location spawnPoint, PedestrianType type, Path path) : base(id, spawnPoint)
        {
            Type = type;
            Path = path;
        }

        public Pedestrian(Location spawnPoint, Model model, PedestrianType type, Path path) : base(spawnPoint)
        {
            Model = model;
            Type = type;
            Path = path;
        }

        public Pedestrian(int id, Location spawnPoint, Model model, PedestrianType type, Path path) : base(id, spawnPoint)
        {
            Model = model;
            Type = type;
            Path = path;
        }

        public Model Model { get; set; }
        public PedestrianType Type { get; set; }
        public Path Path { get; set; }
    }

}