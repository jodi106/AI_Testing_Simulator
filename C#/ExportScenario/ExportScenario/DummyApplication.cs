﻿using System;
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
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            WorldOptions worldOptions = new WorldOptions("2022-09-24T12:00:00", 100000, 0.85F, 0, 1.31, "free", "dry", 0, 1.0);

            Ego egoVehicle = new Ego(0, new Coord3D(255.7, -145.7, 0.3, 200), new EntityModel(0, "vehicle.volkswagen.t2", "notRelevant"));

            EntityModel adversary1 = new EntityModel(1, "vehicle.audi.tt");
            EntityModel adversary2 = new EntityModel(2, "vehicle.lincoln.mkz_2017");

            // Info: A waypoint object is not a waypoint from the .xosc
            List<Coord3D> routeAdversary2 = new List<Coord3D>();
            routeAdversary2.Add(new Coord3D(239, -169, 0.3, 0));
            routeAdversary2.Add(new Coord3D(300, -169, 0.3, 90));
            routeAdversary2.Add(new Coord3D(311, -158, 0.3, 180));
            routeAdversary2.Add(new Coord3D(311, -83, 0.3, 180));
            routeAdversary2.Add(new Coord3D(346, -65, 0.3, 90));
            routeAdversary2.Add(new Coord3D(381, 38, 0.3, 180));
            routeAdversary2.Add(new Coord3D(323, 10, 0.3, 270));
            routeAdversary2.Add(new Coord3D(17, 10, 0.3, 270));
            routeAdversary2.Add(new Coord3D(-23, 10, 0.3, 270));

            List<TriggerInfo> triggerW1 = new List<TriggerInfo>();
            triggerW1.Add(new TriggerInfo("StartTrigger", 0, "rising", 0));

            List<Waypoint> storyAdversary2 = new List<Waypoint>();
            storyAdversary2.Add(new Waypoint(1, new Coord3D(239, -169, 0.3, 0), new List<EntityModel> { adversary2 }, new ActionType("AssignRouteAction", routeAdversary2), triggerW1));
            //Path path_veh_1 = new Path(null, storyAdversary2); // TODO natalie
            Path path_veh_1 = new Path();
            Path path_veh_2 = new Path();
            

            //ToDo create OverallStartTrigger for Path
            List <Vehicle> vehicles = new List<Vehicle>();
            vehicles.Add(new Vehicle(1, new Coord3D(300, -172, 0.3, 160), adversary1, path_veh_1));
            vehicles.Add(new Vehicle(2, new Coord3D(255, -190, 0.3, 90), adversary2, path_veh_2));

            Path path_ped_1 = new Path();
            //Waypoint w1
            //Waypoint w2
            //Actions
            //Triggerlist
            //...

            List<Pedestrian> ped = new List<Pedestrian>();
            ped.Add(new Pedestrian(1, new Coord3D(255, -190, 0.8, 90), new EntityModel(1, "walker.pedestrian.0001"), path_ped_1));

            ScenarioInfo dummy = new ScenarioInfo("OurScenario3", ped, "Town04", worldOptions, egoVehicle, vehicles);
            BuildXML doc = new BuildXML(dummy);
            doc.CombineXML();
        }

    }
}
