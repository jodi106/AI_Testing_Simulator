using System.Collections.ObjectModel;

namespace Models
{
    public class ScenarioInfo
    {
        public string Name { get; set; }
        public ObservableCollection<Pedestrian> Pedestrians { get; set; }
        public string MapURL { get; set; }
        public WorldOptions WorldOptions { get; set; }
        public Ego EgoVehicle { get; set; }
        public ObservableCollection<Vehicle> Vehicles { get; set; }

        public ScenarioInfo()
        {
            Name = null;
            Pedestrians = new ObservableCollection<Pedestrian>();
            MapURL = null;
            WorldOptions = new WorldOptions(0, 0, 0.5f, "free", "dry", 0);
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

        public WorldOptions SetWorldOptionsByParameters(float rainIntensity, float fogIntensity, float sunIntensity, string cloudState, string precipitation, float precipitationIntensity)
        {
            WorldOptions = new WorldOptions(rainIntensity, fogIntensity, sunIntensity, cloudState, precipitation, precipitationIntensity);

            return WorldOptions;
        }

    }
}