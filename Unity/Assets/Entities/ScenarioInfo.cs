using Assets.Enums;
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

        /// <summary>
        /// Converts the Model Representation, where Pedestrians are also Vehicles to the Scenario Representation, 
        /// where Pedestrians are represented as their own Objects.
        /// Doesn't Deepcopy everything just the Properties that need to be deepcopied in order not to modify the original object.
        /// </summary>
        /// <returns>ScenarioInfo Object ready for XML-Export</returns>
        public object Clone()
        {
            string CopyPath = string.Copy(this.Path); //Value
            string CopyMapURL = string.Copy(this.MapURL); //Value
            WorldOptions CopyWorldOptions = this.WorldOptions; //Ref
            ObservableCollection<Pedestrian> exPedestrians = new(); //Value but contains Path Ref
            ObservableCollection<Vehicle> exVehicles = new(); // Value but contains Ref Obj. 
            Ego CopyEgoVehicle = this.EgoVehicle; //Ref
            List<BaseEntity> CopyAllEntities = this.allEntities; //Ref
            
            foreach (Vehicle v in this.Vehicles)
            {
                if (v.Category == VehicleCategory.Pedestrian)
                {
                    //Didn't implement ICloneable interface, since Path can be reference to the Model Object. 
                    Pedestrian CopyPedestrian = new Pedestrian
                        (
                            new Location(v.SpawnPoint.X, v.SpawnPoint.Y, v.SpawnPoint.Z, v.SpawnPoint.Rot), //Value
                            new EntityModel(string.Copy(v.Id), "walker.pedestrian.0001"), //Value
                            v.Path, //Ref
                            PedestrianType.Girl, //Value
                            v.InitialSpeed //Value
                        );

                    exPedestrians.Add(CopyPedestrian);
                }
                else
                {
                    //Same here, ref should be enough
                    exVehicles.Add(v); //Ref
                }
            }


            return new ScenarioInfo
            {
                Path = CopyPath,
                Pedestrians = exPedestrians,
                MapURL = CopyMapURL,
                WorldOptions = CopyWorldOptions,
                EgoVehicle = CopyEgoVehicle,
                Vehicles = exVehicles,
                allEntities = CopyAllEntities
            };
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