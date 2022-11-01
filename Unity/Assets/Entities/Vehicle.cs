using Assets.Enums;

namespace Entities
{
    public class Vehicle : BaseEntity
    {
        public Vehicle() : base() { }

        public Vehicle(Coord3D spawnPoint, Path path) : base(spawnPoint)
        {
            Path = path;
        }

        public Vehicle(int id, Coord3D spawnPoint, Path path) : base(id, spawnPoint)
        {
            Path = path;
        }

        public Vehicle(Coord3D spawnPoint, EntityModel model,  Path path) : base(spawnPoint)
        {
            Model = model;
            Path = path;
        }

        public Vehicle(int id, Coord3D spawnPoint, EntityModel model,  Path path) : base(id, spawnPoint)
        {
            Model = model;
            Path = path;
        }

        public EntityModel Model { get; set; }
        public Path Path { get; set; }
    }
}
