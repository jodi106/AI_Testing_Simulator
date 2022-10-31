using Assets.Enums;
using Dtos;

namespace Entities
{
    public class Ego : BaseEntity
    {
        public Ego() : base() { }
        public Ego(Coord3D spawnPoint, VehicleCategory category) : base(spawnPoint)
        {
            Category = category;
        }

        public Ego(int id, Coord3D spawnPoint, VehicleCategory category) : base(id,spawnPoint)
        {
            Category = category;
        }

        public Ego(Coord3D spawnPoint, EntityModel model, VehicleCategory category) : base(spawnPoint)
        {
            Model = model;
            Category = category;
        }

        public Ego(int id, Coord3D spawnPoint, EntityModel model, VehicleCategory category) : base(id, spawnPoint)
        {
            Model = model;
            Category = category;
        }

        public EntityModel Model { get; set; }
        public VehicleCategory Category { get; set; }
    }
}
