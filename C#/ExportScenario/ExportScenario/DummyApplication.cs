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
            // To have right number format e.g. 80.5 instead of 80,5
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            // Global Weather
            WorldOptions worldOptions = new WorldOptions("2022-09-24T12:00:00", 100000, 0.85F, 0, 1.31, "free", "dry", 0, 1.0);

            // Spawn AI Ego Vehicle
            Ego egoVehicle = new Ego(new Coord3D(258.0, -145.7, 0.3, 30), new EntityModel("vehicle.volkswagen.t2"), 0);


            // SIMULATION VEHICLES:

            // Define when to trigger a Waypoint Action. Each Waypoint has one TriggerList.
            List<TriggerInfo> triggerW1 = new List<TriggerInfo>();
            triggerW1.Add(new TriggerInfo("SimulationTimeCondition", 0, "greaterThan"));
            List<TriggerInfo> triggerW2 = new List<TriggerInfo>();
            triggerW2.Add(new TriggerInfo("DistanceCondition", "adversary2", "lessThan", 20, new Coord3D(250, 10, 0.3, 270)));
            List<TriggerInfo> triggerW3 = new List<TriggerInfo>();
            triggerW3.Add(new TriggerInfo("DistanceCondition", "adversary2", "lessThan", 20, new Coord3D(100, 10, 0.3, 270)));

            // Define what can happen on each Waypoint
            List<Waypoint> eventListAdversary2 = new List<Waypoint>();
            eventListAdversary2.Add(new Waypoint(1, new Coord3D(239, -169, 0.3, 0), new ActionType("AssignRouteAction", new List<Coord3D>()), triggerW1));
            eventListAdversary2.Add(new Waypoint(2, new Coord3D(250, 10, 0.3, 270), new ActionType("LaneChangeAction", 25, "adversary2", 1), triggerW2));
            eventListAdversary2.Add(new Waypoint(3, new Coord3D(100, 10, 0.3, 270), new ActionType("SpeedAction", 0, "step", 10.0, "time"), triggerW3)); // 10s bc. otherwise scenario stops before vehicle stopped

            // Add the Waypoint List to the Path of a Vehicle
            Path path_veh_1 = new Path();
            // Update info:
            // when creating new Path object, list of positions from all Waypoints within the Path is created as RoutePositions attribute
            Path path_veh_2 = new Path(eventListAdversary2);
            // Set the Positions for AssignRouteAction. When first creating the Waypoint containing the AssignRouteAction, this is not possible as the other waypoints are not yet created.
            // This is why Waypoint1's ActionType "AssignRouteAction" is initialized with an empty list of Coordinates
            // With the current structure, AssignRouteAction always has to be the first Waypoint
            path_veh_2.EventList[0].ActionTypeInfo.Positions = path_veh_2.RoutePositions;
            // ToDo: Discuss how to effiently execute the process in line 56 when connecting gui and export. 

            // Spawn Simulation Vehicles with settings from above
            List <Vehicle> vehicles = new List<Vehicle>();
            vehicles.Add(new Vehicle(new Coord3D(300, -172, 0.3, 160), new EntityModel("vehicle.audi.tt"), path_veh_1, 20.0));
            vehicles.Add(new Vehicle(new Coord3D(239, -169, 0.3, 0), new EntityModel("vehicle.lincoln.mkz_2017"), path_veh_2, 15.0));


            // SIMULATION PEDESTRIANS:

            Path path_ped_1 = new Path();
            List<Pedestrian> ped = new List<Pedestrian>();
            ped.Add(new Pedestrian(new Coord3D(255, -190, 0.8, 90), new EntityModel("walker.pedestrian.0001"), path_ped_1, 1));


            // Combine every information into one ScenarioInfo Instance
            ScenarioInfo dummy = new ScenarioInfo("OurScenario3", ped, "Town04", worldOptions, egoVehicle, vehicles);

            // Create .xosc file
            BuildXML doc = new BuildXML(dummy);
            doc.CombineXML();
        }

    }
}

// TODO: with these (less) waypoints, the vehicle uses another path. do we already know this path in the GUI?