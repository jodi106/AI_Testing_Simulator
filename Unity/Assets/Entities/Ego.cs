using Assets.Enums;
using System;

namespace Entity
{
    [Serializable]

        /// <summary>Create Ego vehicle. Object has no actions, only start point and destination</summary>

    public class Ego : BaseEntity, ICloneable
    {



        /// <summary>
        /// Creates an Ego vehicle object with a spawn point, model, category, and initial speed.
        /// Object has no actions, only start point and destination
        /// </summary>
        /// <param name="spawnPoint">The spawn point for the Ego vehicle.</param>
        /// <param name="model">The model of the Ego vehicle.</param>
        /// <param name="category">The category of the Ego vehicle.</param>
        /// <param name="initialSpeedKMH">The initial speed of the Ego vehicle in km/h.</param>
        public Ego(Location spawnPoint, EntityModel model, AdversaryCategory category = AdversaryCategory.Null, double initialSpeedKMH = 0) : base("Ego Vehicle", spawnPoint, initialSpeedKMH)
        {
            Model = model;
            Category = category;
            Agent = "simple_vehicle_control"; 
        }


        /// <summary>
        /// Creates an empty Ego vehicle object.
        /// </summary>
        public Ego()
        {
            
        }


        /// <summary>
        /// Sets the category of the Ego vehicle.
        /// </summary>
        /// <param name="category">The category of the Ego vehicle.</param>
        public void setCategory(AdversaryCategory category)
        {
            this.Category = category;
            this.View?.onChangeCategory(category);
        }

        /// <summary>
        /// Sets the model of the Ego vehicle.
        /// </summary>
        /// <param name="model">The model of the Ego vehicle.</param>
        public void setModel(EntityModel model)
        {
            this.Model = model;
            this.View?.onChangeModel(model);
        }


        /// <summary>
        /// Clones the Ego vehicle object.
        /// </summary>
        /// <returns>The cloned deepcopied Ego vehicle object.</returns>
        public object Clone()
        {
            Ego cloneEgo = new();
            cloneEgo.Destination = (Location)this.Destination.Clone();
            cloneEgo.Model = (EntityModel)this.Model.Clone();
            cloneEgo.Category = this.Category;
            cloneEgo.Agent = this.Agent;

            return cloneEgo;
        }

        public Location Destination { get; set; }
        public EntityModel Model { get; private set; }
        public AdversaryCategory Category { get; set; }
        public string Agent { get; set; } // "external_control", "simple_vehicle_control", ...
    }
}
