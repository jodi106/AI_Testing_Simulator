using System.Collections.ObjectModel;

namespace Entity
{
    public class ScenarioInfo
    /// <summary>Create ScenarioInfo Obejct. Contains all GUI-Userinputs</summary>
    {
        public ScenarioInfo()
        {
            Name = null;
            Pedestrians = new ObservableCollection<Pedestrian>();
            MapURL = null;
            WorldOptions = null;
            EgoVehicle = null;
            Vehicles = new ObservableCollection<Vehicle>();
        }

        public ScenarioInfo(string name, ObservableCollection<Pedestrian> pedestrians, string mapURL, WorldOptions worldOptions, Ego egoVehicle, ObservableCollection<Vehicle> vehicles)
        {
            Name = name;
            Pedestrians = pedestrians;
            MapURL = mapURL;
            WorldOptions = worldOptions;
            EgoVehicle = egoVehicle;
            Vehicles = vehicles;
        }

        public string Name { get; set; }
        public ObservableCollection<Pedestrian> Pedestrians { get; set; }
        public string MapURL { get; set; }
        public WorldOptions WorldOptions { get; set; } // MapRepository.cs has possible Maps. 
        public Ego EgoVehicle { get; set; }
        public ObservableCollection<Vehicle> Vehicles { get; set; }
        
    }
}
