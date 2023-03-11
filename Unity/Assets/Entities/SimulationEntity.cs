using Assets.Enums;
using System;


namespace Entity
{
    [Serializable]
    public class SimulationEntity : BaseEntity
    {
        private static int autoIncrementId = 0;

        public VehicleCategory Category { get; protected set; }
        public EntityModel Model { get; protected set; }
        public Path Path { get; set; }
        public StartRouteInfo StartRouteInfo { get; set; } // if != null that StartRouteInfo's Vehicle starts this Vehicle's route

        public SimulationEntity(Location spawnPoint, double initialSpeedKMH, VehicleCategory category,  EntityModel model, Path path, StartRouteInfo startRouteInfo)
            : base(string.Format("{0} {1}", "Adversary", ++autoIncrementId), spawnPoint, initialSpeedKMH)
        {
            Category = category;
            Model = model;
            Path = path;
            StartRouteInfo = startRouteInfo;
        }

        public SimulationEntity()
        {

        }

        public void setModel(EntityModel model)
        {
            this.Model = model;
            this.View?.onChangeModel(model);
        }

        public void setCategory(VehicleCategory category)
        {
            this.Category = category;
            this.View?.onChangeCategory(category);
        }
    }
}
