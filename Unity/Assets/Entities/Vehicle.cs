namespace Entities
{
    public class Vehicle : BaseEntity
    {
        public Vehicle(int id, Coord3D spawnPoint, VehicleOptions options, Path path) : base(id, spawnPoint)
        {
            Options = options;
            Path = path;
        }

        public VehicleOptions Options { get; set; }
        public Path Path { get; set; }
    }
}
