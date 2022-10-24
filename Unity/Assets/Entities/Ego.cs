using Assets.Enums;

namespace Entities
{
    public class Ego : BaseEntity
    {
        public Ego(Coord3D spawnPoint, VehicleCategory category) : base(spawnPoint)
        {
            Category = category;
        }
        public Ego(Coord3D spawnPoint, EntityModel model, VehicleCategory category) : base(spawnPoint)
        {
            Model = model;
            Category = category;
        }

        public EntityModel Model { get; }
        public VehicleCategory Category { get; }
    }
}
