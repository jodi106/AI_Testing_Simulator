/*
ScenarioInfo needs to have these variables:

Map Information:
- string map, e.g. "Town04"

-------------------------------------------

VEHICLES

Information about vehicles:
- int number_of_simulation_cars = 3; // excluding ego_vehicle !!!
- List<string> vehicle_model = new List<string>(); // index 0 is ego, index 1+ is simulation vehicles model
  vehicle_model.Add("vehicle.volkswagen.t2");
  vehicle_model.Add("vehicle.audi.tt");
  vehicle_model.Add("vehicle.audi.tt");
  vehicle_model.Add("vehicle.audi.tt");

Vehicle Spawn Coordinates:
    String[] x = { "255.7", "290", "255", "255" }; // index 0 is ego, index 1+ is simulation vehicle position
    String[] y = { "-145.7", "-172", "-190", "-210" };
    String[] z = { "0.3", "0.3", "0.3", "0.3" };
    String[] h = { "200", "180", "90", "90" };

How to control the ego vehicle:
- In the future we want to add the AI here
- string control_mode = "carla_auto_pilot_control"; // other value: "external_control"


----------------------------------------

PEDESTRIAN

Information about pedestrians:
- int number_of_pedestrians = 2;
- List<string> pedestrian_model = new List<string>(); 
  pedestrian_model.Add("walker.pedestrian.0001");

Pedestrian Spawn positions:
    String[] x_ped = { "265", "266"  };
    String[] y_ped = { "-165.1", "-164" };
    String[] z_ped = { "0.3", "0.3" };
    String[] h_ped = { "200", "90" };

-----------------------------------------

OTHER

Information about the weather:
- string cloudState = "free"; // possible values: cloudy, free, overcast, rainy
- double sunIntensity = 0.85; // the higher the more sun
- string precipitationType = "dry"; // possible values: dry, rain, snow
- double precipitationIntensity = 0.0; // 0.0 no rain


*/
