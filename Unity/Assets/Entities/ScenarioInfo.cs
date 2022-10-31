using Dtos;
using System.Collections.Generic;

namespace Entities
{
    public class ScenarioInfo
    {
        public ScenarioInfo()
        {
        }

        public ScenarioInfo(string name, List<Pedestrian> pedestrians, string mapURL, WorldOptions worldOptions, Ego egoVehicle, List<Vehicle> vehicles)
        {
            Name = name;
            Pedestrians = pedestrians;
            MapURL = mapURL;
            WorldOptions = worldOptions;
            EgoVehicle = egoVehicle;
            Vehicles = vehicles;
        }

        public string Name { get; set; }
        public List<Pedestrian> Pedestrians { get; set; }
        public string MapURL { get; set; }
        public WorldOptions WorldOptions { get; set; }
        public Ego EgoVehicle { get; set; }
        public List<Vehicle> Vehicles { get; set; }
    }
}