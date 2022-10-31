﻿using System;
using System.Xml;
using System.Xml.Linq;

// Needed values (todo get it from scenario_editor)
int number_of_simulation_cars = 2;
int number_of_pedestrians = 1;
String vehicle_model_ego = "vehicle.volkswagen.t2";
String vehicle_model_simulation = "vehicle.audi.tt";
String map = "Town04";
String[] x = { "255.7", "290", "255" }; // index 0 is ego, index 1+ is simulation vehicle position
String[] y = { "-145.7", "-172", "-190" };
String[] z = { "0.3", "0.3", "0.3" };
String[] h = {"200", "180", "90"};
String[] x_ped = { "265" }; 
String[] y_ped = { "-165.1" };
String[] z_ped = { "0.3" };
String[] h_ped = {"200" };
String control_mode = "carla_auto_pilot_control"; // var 1: to use rename file carla_autopilot.py in carla_auto_pilot_control.py
// The path is in the folder: \PythonAPI\scenario_runner-0.9.13\srunner\scenariomanager\actorcontrols
//control_mode = "external_control" # var 2: to use with manual_control.py
double speed = 3.0;
int start_after_x_seconds = 1;
int stop_after_x_seconds = 20;


void CreateXoscFile()
{
    XmlDocument root = new XmlDocument();
    XmlNode xmlVersion = root.CreateXmlDeclaration("1.0", null, null);
    root.AppendChild(xmlVersion);

    XmlNode openScenario = root.CreateElement("OpenSCENARIO");
    root.AppendChild(openScenario);

    // Define entities which are used (cars, pedestrians, bicicles, ...)
    DefineEntities defineEntities = new DefineEntities(root);
    defineEntities.AddBasicTags( openScenario, map);
    defineEntities.AddEntities(openScenario, number_of_simulation_cars, vehicle_model_ego, vehicle_model_simulation, number_of_pedestrians);

    XmlNode storyboard = root.CreateElement("Storyboard");
    openScenario.AppendChild(storyboard);

    // Place the defined entities at specific coordinates on the map
    InitEntities initEntities = new InitEntities(root);
    initEntities.AddInit(storyboard, number_of_simulation_cars, x, y, z, h, control_mode, number_of_pedestrians, x_ped, y_ped, z_ped, h_ped);

    // Start moving the entities: Todo make this more dynamic to be able to handle more different maneuvers.
    ManeuverEntities maneuverEntities = new ManeuverEntities(root);
    maneuverEntities.AddStoryAndAct(storyboard, number_of_simulation_cars, number_of_pedestrians, stop_after_x_seconds);

    root.Save("..\\..\\..\\OurScenario.xosc");
    root.Save(Console.Out);
}


CreateXoscFile();


