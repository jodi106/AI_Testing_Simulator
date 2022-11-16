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
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            WorldOptions worldOptions = new WorldOptions("2022-09-24T12:00:00", 100000, 0.85F, 0, 1.31, "free", "dry", 0, 1.0);

            Ego egoVehicle = new Ego(new Coord3D(255.7, -145.7, 0.3, 200), new EntityModel("vehicle.volkswagen.t2"), 0);

            EntityModel adversary1 = new EntityModel("vehicle.audi.tt");
            EntityModel adversary2 = new EntityModel("vehicle.lincoln.mkz_2017");

            /*
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
            */

            List<TriggerInfo> triggerW1 = new List<TriggerInfo>();
            triggerW1.Add(new TriggerInfo("SimulationTimeCondition", 0, "greaterThan"));
            List<TriggerInfo> triggerW2 = new List<TriggerInfo>();
            triggerW2.Add(new TriggerInfo("DistanceCondition", "adversary2", "lessThan", 20, new Coord3D(250, 10, 0.3, 270)));
            List<TriggerInfo> triggerW3 = new List<TriggerInfo>();
            triggerW3.Add(new TriggerInfo("DistanceCondition", "adversary2", "lessThan", 20, new Coord3D(100, 10, 0.3, 270)));

            List<Waypoint> eventListAdversary2 = new List<Waypoint>();
            eventListAdversary2.Add(new Waypoint(1, new Coord3D(239, -169, 0.3, 0), new ActionType("AssignRouteAction", new List<Coord3D>()), triggerW1));
            eventListAdversary2.Add(new Waypoint(2, new Coord3D(250, 10, 0.3, 270), new ActionType("LaneChangeAction", 25, "adversary2", 1), triggerW2));
            eventListAdversary2.Add(new Waypoint(3, new Coord3D(100, 10, 0.3, 270), new ActionType("SpeedAction", 0, "step", 10.0, "time"), triggerW3)); // 10s bc. otherwise scenario stops before vehicle stopped

            Path path_veh_1 = new Path();
            // Update info:
            // when creating new Path object, list of positions from all Waypoints within the Path is created as RoutePositions attribute
            Path path_veh_2 = new Path(eventListAdversary2);
            // Set the Positions for AssignRouteAction. When first creating the Waypoint containing the AssignRouteAction, this is not possible as the other waypoints are not yet created.
            // This is why Waypoint1's ActionType "AssignRouteAction" is initialized with an empty list of Coordinates
            // With the current structure, AssignRouteAction always has to be the first Waypoint
            path_veh_2.EventList[0].ActionTypeInfo.Positions = path_veh_2.RoutePositions;
            // ToDo: Discuss how to effiently execute the process in line 56 when connecting gui and export. 


            List <Vehicle> vehicles = new List<Vehicle>();
            vehicles.Add(new Vehicle(new Coord3D(300, -172, 0.3, 160), adversary1, path_veh_1, 20.0));
            vehicles.Add(new Vehicle(new Coord3D(239, -169, 0.3, 0), adversary2, path_veh_2, 15.0));

            Path path_ped_1 = new Path();
            List<Pedestrian> ped = new List<Pedestrian>();
            ped.Add(new Pedestrian(new Coord3D(255, -190, 0.8, 90), new EntityModel("walker.pedestrian.0001"), path_ped_1, 1));

            ScenarioInfo dummy = new ScenarioInfo("OurScenario3", ped, "Town04", worldOptions, egoVehicle, vehicles);
            BuildXML doc = new BuildXML(dummy);
            doc.CombineXML();
        }

    }
}
