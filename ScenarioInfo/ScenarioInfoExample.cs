using System.Reflection.Metadata;
using System;
using Entities;


public class ScenarioInfoExample1{

    public ScenarioInfo m_exampleScenario1 {get;set;}
    
    public ScenarioInfoExample1(){

        ////////WorldOptions////////
        SetWorldOptions(this.m_exampleScenario1, 0, 0);
        m_exampleScenario1.MapURL = "Town04";

        
        ////////Vehicles////////
        // Hero / Ego
        Ego egoVehicle = new Ego();
        egoVehicle.SpawnPoint = Coord3D(255.7, -145.7, 0.3, 200);
        egoVehicle.Category = "car";
        egoVehicle.Model = "vehicle.volkswagen.t2";
        
        // adversary0
        Vehicle adversary0 = new Vehicle();
        adversary0.SpawnPoint = Coord3D(290, -172, 0.3, 180);
        adversary0.Category = "car";
        adversary0.Model = "vehicle.audi.tt";
        m_exampleScenario1.Vehicles.Add(adversary0);

        //adversary1
        Vehicle adversary1 = new Vehicle();
        adversary1.SpawnPoint = Coord3D(255, -190, 0.3, 90);
        adversary1.Category = "car";
        adversary1.Model = "vehicle.audi.tt";

        ////////Pedestrians////////
        // adversary_pedestrian0
        Pedestrian adversaryPedestrian0 = new Pedestrian();
        adversaryPedestrian0.SpawnPoint = Coord3D(265, -165-1, 0.3, 200);
        adversaryPedestrian0.Category = "car";
        adversaryPedestrian0.Model = "walker.pedestrian.0001";

        ////////Events////////
        Event adversaryAcceleratesEvent = new Event();
        adversaryAcceleratesEvent.ActionType = "AccelerateManeuver";
        adversaryAcceleratesEvent.InvolvedEntities.Add(adversary0);
        adversaryAcceleratesEvent.InvolvedEntities.Add(adversary1);
        //TODO ActionType, hwo should we define them? F.e. StartTime, Acceleareate, How Conditions? 

        m_exampleScenario1.EgoVehicle = EgoVehicleHero;
        m_exampleScenario1.Vehicles.Add(adversary0);
        m_exampleScenario1.Vehicles.Add(adversary1);
        m_exampleScenario1.Vehicles.Add(adversaryPedestrian0);



        /*public Event(int id, Coord position, List<BaseEntity> involvedEntities, string actionType)
        {
            Id = id;
            Position = position;
            InvolvedEntities = involvedEntities;
            ActionType = actionType;
        }

        */

    }



    private void SetWorldOptions(ScenarioInfo exampleScenario, float rainIntensity, float fogIntensity){

        if ((1 <= rainIntensity && 0 <= rainIntensity) && 1 <= fogIntensity && 0 <= fogIntensity){
            WorldOptions worldOptions = new WorldOptions(rainIntensity, fogIntensity);
            exampleScenario.WorldOptions = worldOptions;
        }

        throw new ArgumentOutOfRangeException("rainIntensity and fogIntensity must be between 0.0 and 1.0");

    }

}
/*
    public class ScenarioInfo
    {
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

*/