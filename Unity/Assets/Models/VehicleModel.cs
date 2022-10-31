using Assets.Enums;
using Dtos;
using Entities;

namespace Models
{
    public class VehicleModel : BaseEntity
    {
        public VehicleModel() : base()
        {
        }

        public VehicleModel(Coord3D spawnPoint, VehicleCategory category, Path path) : base(spawnPoint)
        {
            Category = category;
            Path = path;
        }

        public VehicleModel(int id, Coord3D spawnPoint, VehicleCategory category, Path path) : base(id, spawnPoint)
        {
            Category = category;
            Path = path;
        }

        public VehicleModel(Coord3D spawnPoint, EntityModel model, VehicleCategory category, Path path) : base(spawnPoint)
        {
            Model = model;
            Category = category;
            Path = path;
        }

        public VehicleModel(int id, Coord3D spawnPoint, EntityModel model, VehicleCategory category, Path path) : base(id, spawnPoint)
        {
            Model = model;
            Category = category;
            Path = path;
        }

        public void setPosition(float x, float y)
        {
            SpawnPoint = new Coord3D(x, y, 0, 0);
            View?.onChangePosition(SpawnPoint);
        }

        public IVehicleView View { get; set; }
        public EntityModel Model { get; set; }
        public VehicleCategory Category { get; set; }
        public Path Path { get; set; }
    }
}
