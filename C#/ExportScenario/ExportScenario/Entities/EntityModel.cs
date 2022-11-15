using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class EntityModel
    {
        public EntityModel(int id, string modelName)
        {
            Id = id;
            Name = modelName;
            DisplayName = modelName;
        }

        public EntityModel(int id, string modelName, string displayName)
        {
            Id = id;
            Name = modelName;
            DisplayName = displayName;
        }

        public int Id { get; set; }
        public string Name { get; set; } // Name of the model: example "vehicle.lincoln.mkz_2017"
        public string DisplayName { get; set; } // Not relevant for us

    }
}
