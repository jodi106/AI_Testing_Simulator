using Assets.Enums;
using System;

/**
 * Adversary := Vehicle, Motorcycle, Bike, or Pedestrian that has a pre-defined behavior (simulation vehicles, not AI)
 * */

namespace Entity
{
    [Serializable]
    public class Adversary : BaseEntity, ICloneable
    {
        private static int autoIncrementId = 0;

        public VehicleCategory Category { get; protected set; }
        public EntityModel Model { get; protected set; }
        public Path Path { get; set; }
        public StartRouteInfo StartRouteInfo { get; set; } // if != null that StartRouteInfo's Vehicle starts this Vehicle's route

        public Adversary(Location spawnPoint, double initialSpeedKMH, VehicleCategory category, EntityModel model, Path path, StartRouteInfo startRouteInfo = null)
            : base(string.Format("{0} {1}", "Adversary", ++autoIncrementId), spawnPoint, initialSpeedKMH)
        {
            Category = category;
            Model = model;
            Path = path;
            StartRouteInfo = startRouteInfo;
        }

        public Adversary()
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

        public object Clone()
        {
            Adversary cloneVehicle = new Adversary();
            cloneVehicle.Model = (EntityModel)this.Model.Clone();
            cloneVehicle.Path = (Path)this.Path.Clone();
            cloneVehicle.Category = this.Category;
            cloneVehicle.StartRouteInfo = this.StartRouteInfo; // // TODO copy value, not reference (but works anyway)

            //BaseEntity
            cloneVehicle.Id = string.Copy(this.Id);
            cloneVehicle.SpawnPoint = (Location)this.SpawnPoint.Clone();
            cloneVehicle.InitialSpeedKMH = this.InitialSpeedKMH;
            cloneVehicle.Color = this.Color;

            //I don't think we need BaseEntity.View here since its only for the export? 
            return cloneVehicle;
        }
    }
}
