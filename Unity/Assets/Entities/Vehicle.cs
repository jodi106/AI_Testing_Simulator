using Assets.Enums;

namespace Entity
{
    public class Vehicle : BaseEntity
    {
        private static int autoIncrementId = 0;

        public Vehicle(Location spawnPoint, EntityModel model, Path path, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0)
            : base(string.Format("{0} {1}", "Adversary", ++autoIncrementId), spawnPoint, initialSpeed)
        {
            Model = model;
            Path = path;
            Category = category;
        }

        public void setCategory(VehicleCategory category)
        {
            this.Category = category;
            this.View?.onChangeType(category);
        }

        public void setModel(EntityModel model)
        {
            this.Model = model;
            this.View?.onChangeModel(model);
        }

        public EntityModel Model { get; private set; }
        public VehicleCategory Category { get; private set; }
        public Path Path { get; set; }
    }
}
