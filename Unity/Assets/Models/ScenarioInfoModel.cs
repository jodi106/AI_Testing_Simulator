using Dtos;
using System.Collections.ObjectModel;

namespace Models
{
    public class ScenarioInfoModel
    {
        public string Name { get; set; }
        public ObservableCollection<PedestrianModel> Pedestrians { get; set; }
        public string MapURL { get; set; }
        public WorldOptions WorldOptions { get; set; }
        public EgoModel EgoVehicle { get; set; }
        public ObservableCollection<VehicleModel> Vehicles { get; set; }

        public ScenarioInfoModel()
        {
            Name = null;
            Pedestrians = new ObservableCollection<PedestrianModel>();
            MapURL = null;
            WorldOptions = new WorldOptions(0, 0, 0.5f, "free", "dry", 0);
            EgoVehicle = null;
            Vehicles = new ObservableCollection<VehicleModel>();
        }
        public ScenarioInfoModel(string name, ObservableCollection<PedestrianModel> pedestrians, string mapURL, WorldOptions worldOptions, EgoModel egoVehicle, ObservableCollection<VehicleModel> vehicles)
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