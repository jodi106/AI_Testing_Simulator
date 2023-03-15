using Assets.Enums;
using System;

namespace Entity
{
    [Serializable]
    public class Pedestrian : SimulationEntity
    /// <summary>Creates Pedestrian Object. Contains all Pedestiran-Entity specific info created by Gui-User.</summary>
    {
        public PedestrianType Type { get; set; }

        public Pedestrian(Location spawnPoint, Path path, PedestrianType pedestrianType = PedestrianType.Null, double initialSpeedKMH = 0)
            : base(spawnPoint, initialSpeedKMH, null, path, null)
        {
            Path = path;
            Type = pedestrianType;
        }

        public Pedestrian(Location spawnPoint, EntityModel model, Path path, PedestrianType pedestrianType = PedestrianType.Null, double initialSpeedKMH = 0)
            : base(spawnPoint, initialSpeedKMH, model, path, null)
        {
            Model = model;
            Path = path;
            Type = pedestrianType;
        }

        /// Used for Copy
        public Pedestrian(Location spawnPoint, EntityModel model, Path path, PedestrianType pedestrianType, double initialSpeedKMH, string id, StartRouteInfo startRouteInfo)
            : base(spawnPoint, initialSpeedKMH, null, path, null)
        {
            Model = model;
            Path = path;
            Type = pedestrianType;
            Id = id;
            StartRouteInfo = startRouteInfo;
        }
    }
}
