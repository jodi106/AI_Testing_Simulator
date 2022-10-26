using Assets.Enums;
using System.Transactions;
using UnityEditor;

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

        public Vehicle(Coord3D spawnPoint, EntityModel model, VehicleCategory category, Path path) : base(spawnPoint)
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
