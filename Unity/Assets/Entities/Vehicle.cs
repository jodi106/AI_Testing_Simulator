using Assets.Enums;
using System;


namespace Entity
{
    [Serializable]
    public class Vehicle : SimulationEntity, ICloneable
    {
        public Vehicle(Location spawnPoint, EntityModel model, Path path, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0)
            : base(spawnPoint, initialSpeed, category, model, path, null)
        {
        }

        public Vehicle()
        {

        }

        public new void setCategory(VehicleCategory category)
        {
            base.setCategory(category);
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

        
       
    }
}
