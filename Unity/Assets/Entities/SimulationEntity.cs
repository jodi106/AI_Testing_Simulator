using Assets.Enums;
using System;


namespace Entity
{
    [Serializable]
    public class SimulationEntity : BaseEntity
    {
        private static int autoIncrementId = 0;

        public EntityModel Model { get; protected set; }
        public Path Path { get; set; }
        public StartRouteInfo StartRouteInfo { get; set; } // if != null that StartRouteInfo's Vehicle starts this Vehicle's route

        public SimulationEntity(Location spawnPoint, double initialSpeedKMH, EntityModel model, Path path, StartRouteInfo startRouteInfo)
            : base(string.Format("{0} {1}", "Adversary", ++autoIncrementId), spawnPoint, initialSpeedKMH)
        {
            Model = model;
            Path = path;
            StartRouteInfo = startRouteInfo;
            //CurrentSpeedKMH = initialSpeedKMH; // TODO keep or drop
        }

        public SimulationEntity()
        {

        }

        public void setModel(EntityModel model)
        {
            this.Model = model;
            this.View?.onChangeModel(model);
        }
    }
}
