using System;

namespace Entity
{
    [Serializable]
    public class EntityModel : ICloneable
    /// <summary>Creates EntityModel object which contains informations about Model from Carla Blueprints</summary>
    {
        private static int autoIncrementId = 1;


        /// <summary>
        /// Creates an EntityModel object with the given model name.
        /// </summary>
        /// <param name="modelName">The name of the model.</param
        public EntityModel(string modelName)
        {
            Id = autoIncrementId++;
            DisplayName = modelName;
            CarlaName = modelName;
        }


        /// <summary>
        /// Creates an EntityModel object with the given display name and Carla name.
        /// </summary>
        /// <param name="displayName">The display of the model.</param>
        /// <param name="carlaName">The Carla name of the model.</param>
        public EntityModel(string displayName, string carlaName)
        {
            Id = autoIncrementId++;
            DisplayName = displayName;
            CarlaName = carlaName;
        }


        /// <summary>
        /// Creates an empty EntityModel object.
        /// </summary>
        public EntityModel()
        {

        }

        public int Id { get; set; }
        public string CarlaName { get; set; } // Name of the model: example "vehicle.lincoln.mkz_2017"
        public string DisplayName { get; set; } // GUI


        /// <summary>
        /// Clones the EntityModel object (deepcopy).
        /// </summary>
        /// <returns>A cloned deepcopy of the EntityModel object.</returns>
        public object Clone()
        {
            EntityModel cloneEntityModel = new();
            cloneEntityModel.Id = this.Id;
            cloneEntityModel.CarlaName = string.Copy(this.CarlaName);
            cloneEntityModel.DisplayName = string.Copy(this.DisplayName);

            return cloneEntityModel;
        }
    }
}
