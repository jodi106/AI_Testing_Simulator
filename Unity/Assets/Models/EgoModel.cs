using Assets.Enums;
using Dtos;

namespace Models
{
    public class EgoModel : BaseModel
    {
        public EgoModel(Coord3D spawnPoint, VehicleCategory category) : base(spawnPoint)
        {
            Category = category;
        }

        public EgoModel(int id, Coord3D spawnPoint, VehicleCategory category) : base(id, spawnPoint)
        {
            Category = category;
        }

        public EgoModel(Coord3D spawnPoint, EntityModel model, VehicleCategory category) : base(spawnPoint)
        {
            Model = model;
            Category = category;
        }

        public EgoModel(int id, Coord3D spawnPoint, EntityModel model, VehicleCategory category) : base(id, spawnPoint)
        {
            Model = model;
            Category = category;
        }

        public EntityModel Model { get; set; }
        public VehicleCategory Category { get; set; }
    }
}
