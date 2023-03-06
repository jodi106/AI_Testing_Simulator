using Assets.Enums;
using System;

namespace Entity
{
    [Serializable]
    public class Ego : BaseEntity, ICloneable
    /// <summary>Create Ego vehicle. Object has no actions, only start point and destination</summary>
    /// ToDo: Implement AI driving
    {

        public Ego(Location spawnPoint, EntityModel model, VehicleCategory category = VehicleCategory.Null, double initialSpeedKMH = 0) : base("Ego Vehicle", spawnPoint, initialSpeedKMH)
        {
            Model = model;
            Category = category;
        }

        public Ego()
        {

        }

        public void setCategory(VehicleCategory category)
        {
            this.Category = category;
            this.View?.onChangeCategory(category);
        }

        
        public void setModel(EntityModel model)
        {
            this.Model = model;
            this.View?.onChangeModel(model);
        }

        public object Clone()
        {
            Ego cloneEgo = new();
            cloneEgo.Destination = (Location)this.Destination.Clone();
            cloneEgo.Model = (EntityModel)this.Model.Clone();
            cloneEgo.Category = this.Category;

            return cloneEgo;
        }

        public Location Destination { get; set; }
        public EntityModel Model { get; private set; }
        public VehicleCategory Category { get; set; }
    }
}
