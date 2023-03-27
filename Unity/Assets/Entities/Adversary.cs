using Assets.Enums;
using System;

namespace Entity
{
    [Serializable]

    ///<summary>
    ///Entity representing an adversary (Vehicle, Motorcycle, Bike, or Pedestrian) with a pre-defined behavior in simulation (not AI)
    ///</summary>
    ///
    public class Adversary : BaseEntity, ICloneable
    {
        private static int autoIncrementId = 0;

        public AdversaryCategory Category { get; protected set; }
        public EntityModel Model { get; protected set; }
        public Path Path { get; set; }
        public StartPathInfo StartPathInfo { get; set; } // if != null that StartPathInfo's Vehicle starts this Vehicle's route

        ///<summary>
        ///Constructor for the Adversary class
        ///</summary>
        ///<param name="spawnPoint">The location of the adversary's spawn point</param>
        ///<param name="initialSpeedKMH">The initial speed of the adversary in kilometers per hour</param>
        ///<param name="category">The category of the adversary (Vehicle, Motorcycle, Bike, or Pedestrian)</param>
        ///<param name="model">The model of the adversary</param>
        ///<param name="path">The path of the adversary</param>
        ///<param name="startRouteInfo">Information about how and when the path of the adversary's vehicle is started, if null it starts after 0 sec</param>
        public Adversary(Location spawnPoint, double initialSpeedKMH, AdversaryCategory category, EntityModel model, Path path, StartPathInfo startRouteInfo = null)
            : base(string.Format("{0} {1}", "Adversary", ++autoIncrementId), spawnPoint, initialSpeedKMH)
        {
            Category = category;
            Model = model;
            Path = path;
            StartPathInfo = startRouteInfo;
        }

        ///<summary>
        ///Default constructor for the Adversary class
        ///</summary>
        public Adversary()
        {

        }

        public static void resetAutoIncrementID()
        {
            autoIncrementId = 0;
        }


        ///<summary>
        ///Sets the model of the adversary
        ///</summary>
        ///<param name="model">The model to set for the adversary</param>
        public void setModel(EntityModel model)
        {
            this.Model = model;
            this.View?.onChangeModel(model);
        }


        ///<summary>
        ///Sets the category of the adversary
        ///</summary>
        ///<param name="category">The category to set for the adversary</param>
        public void setCategory(AdversaryCategory category)
        {
            this.Category = category;
            this.View?.onChangeCategory(category);
        }


        ///<summary>
        ///Creates a clone of the adversary object (deep copy)
        ///</summary>
        ///<returns>A cloned (deepcopy) adversary object</returns>
        public object Clone()
        {
            Adversary cloneVehicle = new Adversary();
            cloneVehicle.Model = (EntityModel)this.Model.Clone();
            cloneVehicle.Path = (Path)this.Path.Clone();
            cloneVehicle.Category = this.Category;
            cloneVehicle.StartPathInfo = this.StartPathInfo; // // TODO copy value, not reference (but works anyway)

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
