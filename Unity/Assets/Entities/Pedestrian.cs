using Assets.Enums;
using System;

namespace Entity
{
    [Serializable]
    public class Pedestrian : SimulationEntity
    /// <summary>Creates Pedestrian Object. Contains all Pedestiran-Entity specific info created by Gui-User.</summary>
    {

        public Pedestrian(Location spawnPoint, Path path, VehicleCategory category = VehicleCategory.Pedestrian, double initialSpeedKMH = 0)
            : base(spawnPoint, initialSpeedKMH, category, null, path, null)
        {
        }

        public Pedestrian(Location spawnPoint, EntityModel model, Path path, VehicleCategory category = VehicleCategory.Pedestrian, double initialSpeedKMH = 0)
            : base(spawnPoint, initialSpeedKMH, category, model, path, null)
        {
        }

        /// Used for Copy
        public Pedestrian(Location spawnPoint, EntityModel model, Path path, VehicleCategory category, double initialSpeedKMH, string id, StartRouteInfo startRouteInfo)
            : base(spawnPoint, initialSpeedKMH, category, null, path, null)
        {
            Model = model;
            Path = path;
            Category = category;
            Id = id;
            StartRouteInfo = startRouteInfo;
        }
    }
}
