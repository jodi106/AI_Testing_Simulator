using Assets.Enums;
using Dtos;

namespace Entities
{
    public class Vehicle : BaseEntity
    {
        public Vehicle() : base()
        {
        }

        public Vehicle(Coord3D spawnPoint, VehicleCategory category, Path path) : base(spawnPoint)
        {
            Category = category;
            Path = path;
        }

        public Vehicle(int id, Coord3D spawnPoint, VehicleCategory category, Path path) : base(id, spawnPoint)
        {
            Category = category;
            Path = path;
        }

        public Vehicle(Coord3D spawnPoint, EntityModel model, VehicleCategory category, Path path) : base(spawnPoint)
        {
            Model = model;
            Category = category;
            Path = path;
        }

        public Vehicle(int id, Coord3D spawnPoint, EntityModel model, VehicleCategory category, Path path) : base(id, spawnPoint)
        {
            Model = model;
            Category = category;
            Path = path;
        }

        public EntityModel Model { get; }
        public VehicleCategory Category { get; }
        public Path Path { get; set; }
    }
}
