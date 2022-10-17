namespace Entities
{
    public class Pedestrian : BaseEntity
    {
        public Pedestrian(int id, Coord3D spawnPoint, PedestrianOptions options, Path path) : base(id, spawnPoint)
        {
            Options = options;
            Path = path;
        }

        public PedestrianOptions Options { get; set; }
        public Path Path { get; set; }
    }

}