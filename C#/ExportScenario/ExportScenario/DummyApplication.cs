using System;
using System.Collections.Generic;
using System.Text;

using ExportScenario.Entities;
using ExportScenario.XMLBuilder;

namespace ExportScenario
{
    class DummyApplication
    {
        public static void Main()
        {
            WorldOptions worldOptions = new WorldOptions("2022-09-24T12:00:00", 100000, 0.85F, 0, 1.31, "free", "dry", 0, 1.0);

            Ego egoVehicle = new Ego(0, new Coord3D(255.7, -145.7, 0.3, 200), new EntityModel(0, "vehicle.volkswagen.t2", "notRelevant"));

            Path path_veh_1 = new Path();
            Path path_veh_2 = new Path();
            List<Vehicle> vehicles = new List<Vehicle>();
            vehicles.Add(new Vehicle(1, new Coord3D(300, -172, 0.3, 160), new EntityModel(1, "vehicle.audi.tt"), path_veh_1));
            vehicles.Add(new Vehicle(2, new Coord3D(255, -190, 0.3, 90), new EntityModel(2, "vehicle.audi.tt"), path_veh_2));

            Path path_ped_1 = new Path();
            List<Pedestrian> ped = new List<Pedestrian>();
            ped.Add(new Pedestrian(1, new Coord3D(255, -190, 0.8, 90), new EntityModel(1, "walker.pedestrian.0001"), path_ped_1));

            ScenarioInfo dummy = new ScenarioInfo("OurScenario3", ped, "Town04", worldOptions, egoVehicle, vehicles);
            BuildXML doc = new BuildXML(dummy);
            doc.CombineXML();
        }

    }
}
