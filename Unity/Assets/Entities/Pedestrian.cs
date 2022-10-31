using Assets.Enums;
using Dtos;

namespace Entities
{
    public class Pedestrian : BaseEntity
    {
        public Pedestrian() : base() { }
        public Pedestrian(Coord3D spawnPoint, PedestrianType type, Path path) : base(spawnPoint)
        {
            Type = type;
            Path = path;
        }

        public Pedestrian(int id, Coord3D spawnPoint, PedestrianType type, Path path) : base(id,spawnPoint)
        {
            Type = type;
            Path = path;
        }

        public Pedestrian(Coord3D spawnPoint, EntityModel model, PedestrianType type, Path path) : base(spawnPoint)
        {
            Model = model;
            Type = type;
            Path = path;
        }

        public Pedestrian(int id, Coord3D spawnPoint, EntityModel model, PedestrianType type, Path path) : base(id, spawnPoint)
        {
            Model = model;
            Type = type;
            Path = path;
        }

        public EntityModel Model { get; set; }
        public PedestrianType Type { get; set; }
        public Path Path { get; set; }
    }

}