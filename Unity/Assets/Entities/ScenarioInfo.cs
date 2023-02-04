using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Entity
{
    public class ScenarioInfo
    /// <summary>Create ScenarioInfo Obejct. Contains all GUI-Userinputs</summary>
    {
        public ScenarioInfo()
        {
            Path = null;
            Pedestrians = new ObservableCollection<Pedestrian>();
            MapURL = null;
            WorldOptions = new WorldOptions();
            EgoVehicle = null;
            Vehicles = new ObservableCollection<Vehicle>();
            allEntities = new List<BaseEntity>();
            attachListener();
        }

        public ScenarioInfo(string path, ObservableCollection<Pedestrian> pedestrians, string mapURL, WorldOptions worldOptions, Ego egoVehicle, ObservableCollection<Vehicle> vehicles)
        {
            Path = path;
            Pedestrians = pedestrians;
            MapURL = mapURL;
            WorldOptions = worldOptions;
            EgoVehicle = egoVehicle;
            Vehicles = vehicles;
        }

        void attachListener()
        {
            Vehicles.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs args) =>
            {
                if (args.NewItems is not null)
                {
                    foreach (var item in args.NewItems)
                    {
                        allEntities.Add((Vehicle)item);
                    };
                }
                if (args.OldItems is not null)
                {
                    foreach (var item in args.OldItems)
                    {
                        allEntities.Remove((Vehicle)item);
                    };
                }
            };
        }

        public void setEgo(Ego ego)
        {
            if(EgoVehicle is not null)
            {
                this.allEntities.RemoveAt(0);
            }
            if (ego is not null)
            {
                this.allEntities.Insert(0, ego);
            }
            this.EgoVehicle = ego;
            onEgoChanged();
        }

        public string Path { get; set; }
        public ObservableCollection<Pedestrian> Pedestrians { get; set; }
        public string MapURL { get; set; }
        public WorldOptions WorldOptions { get; set; } // MapRepository.cs has possible Maps. 
        public Ego EgoVehicle { get; private set; }
        public ObservableCollection<Vehicle> Vehicles { get; set; }

        public List<BaseEntity> allEntities { get; private set; }

        public System.Action onEgoChanged;

    }
}