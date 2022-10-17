namespace Entities
{
    public class Ego : BaseEntity
    {
        public Ego(int id, Coord3D spawnPoint,VehicleOptions options) : base(id, spawnPoint)
        {
            Options = options;
        }

        public VehicleOptions Options { get; set; }
    }
}
