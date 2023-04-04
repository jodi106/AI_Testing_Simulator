using System;

namespace Entity
{
    [Serializable]
    public class EntityModel : ICloneable
    /// <summary>Creates EntityModel object which contains informations about Model from Carla Blueprints</summary>
    {
        private static int autoIncrementId = 1;

        public EntityModel(string modelName)
        {
            Id = autoIncrementId++;
            DisplayName = modelName;
            CarlaName = modelName;
        }

        public EntityModel(string displayName, string carlaName)
        {
            Id = autoIncrementId++;
            DisplayName = displayName;
            CarlaName = carlaName;
        }

        public EntityModel()
        {

        }

        public int Id { get; set; }
        public string CarlaName { get; set; } // Name of the model: example "vehicle.lincoln.mkz_2017"
        public string DisplayName { get; set; } // GUI

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
