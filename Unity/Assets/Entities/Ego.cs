using Assets.Enums;

namespace Entity
{
    public class Ego : BaseEntity
    /// <summary>Create Ego vehicle. Object has no actions, only start point and destination</summary>
    /// ToDo: Implement AI driving
    {

        public Ego(Location spawnPoint, EntityModel model, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base("Ego Vehicle", spawnPoint, initialSpeed)
        {
            Model = model;
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

        public Location Destination { get; set; }
        public EntityModel Model { get; private set; }
        public VehicleCategory Category { get; set; }
    }
}
