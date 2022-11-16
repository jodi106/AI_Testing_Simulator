using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class EntityModel
    /// <summary>Creates EntityModel object which contains informations about Model from Carla Blueprints</summary>
    {
        private static int autoIncrementId = 1;
        public EntityModel(string modelName)
        {
            Id = autoIncrementId++;
            Name = modelName;
            DisplayName = modelName;
        }

        public EntityModel(string modelName, string displayName)
        {
            Id = autoIncrementId++;
            Name = modelName;
            DisplayName = displayName;
        }

        public int Id { get; set; }
        public string Name { get; set; } // Name of the model: example "vehicle.lincoln.mkz_2017"
        public string DisplayName { get; set; } // Not relevant for us

    }
}
