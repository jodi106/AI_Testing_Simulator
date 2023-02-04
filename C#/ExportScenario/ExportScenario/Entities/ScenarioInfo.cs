using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class ScenarioInfo
    /// <summary>Create ScenarioInfo Obejct. Contains all GUI-Userinputs</summary>
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
