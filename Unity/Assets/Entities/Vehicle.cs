using Assets.Enums;
using System;


namespace Entity
{
    public class Vehicle : BaseEntity, ICloneable
    {
        private static int autoIncrementId = 0;

        public Vehicle(Location spawnPoint, EntityModel model, Path path, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0)
            : base(string.Format("{0} {1}", "Adversary", ++autoIncrementId), spawnPoint, initialSpeed)
        {
            Model = model;
            Path = path;
            Category = category;
        }

        public Vehicle()
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
            Vehicle cloneVehicle = new Vehicle();
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

        public EntityModel Model { get; private set; }
        public VehicleCategory Category { get; private set; }
        public Path Path { get; set; }
        public StartRouteInfo StartRouteInfo { get; set; } // if != null that StartRouteVehicle starts this Vehicle's route
        //public Vehicle StartRouteVehicle { get; set; } // if != null that StartRouteVehicle starts this Vehicle's route
    }
}
