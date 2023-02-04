namespace Entity
{ 
    public class EntityModel
    /// <summary>Creates EntityModel object which contains informations about Model from Carla Blueprints</summary>
    {
        private static int autoIncrementId = 1;
        public EntityModel(string modelName)
        {
            Id = autoIncrementId++;
            DisplayName = modelName;
            CarlaName = "vehicle.nissan.micra";
        }

        public EntityModel(int id, string modelName)
        {
            Id = autoIncrementId++;
            DisplayName = modelName;
            CarlaName = modelName;
        }

        public EntityModel(int id, string displayName, string carlaName)
        {
            Id = autoIncrementId++;
            DisplayName = displayName;
            CarlaName = carlaName;
        }

        public EntityModel(string displayName, string carlaName)
        {
            Id = autoIncrementId++;
            DisplayName = displayName;
            CarlaName = carlaName;
        }

        public int Id { get; set; }
        public string CarlaName { get; set; } // Name of the model: example "vehicle.lincoln.mkz_2017"
        public string DisplayName { get; set; } // GUI

    }
}
