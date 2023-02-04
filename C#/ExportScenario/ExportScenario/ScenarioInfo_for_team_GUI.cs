/*
ScenarioInfo needs to have these variables:
Map Information:
- string map, e.g. "Town04"
- string scenario_name, A name for the created scneario

-------------------------------------------
VEHICLES
- initial Speed
- rotatio
How to control the ego vehicle:
- In the future we want to add the AI here
- string control_mode = "carla_auto_pilot_control"; // other value: "external_control"
----------------------------------------
PEDESTRIAN

-----------------------------------------
OTHER
Information about the weather:
- string cloudState = worldOptions.CloudState; // possible values: cloudy, free, overcast, rainy
- double sunIntensity = worldOptions.SunIntensity; // Illuminance of the sun, direct sunlight is around 100,00 lx. Unit: lux; Range: [0..inf[.
- string precipitationType = worldOptions.PrecipitationTypes; // possible values: dry, rain, snow
- double precipitationIntensity = worldOptions.PrecipitationIntensity; // 0.0 no rain
- string dateTime = worldOptions.Date_Time; // Format: "2019-06-25T12:00:00"
- double sunAzimuth = worldOptions.SunAzimuth; // Azimuth of the sun, counted counterclockwise, 0=north, PI/2 = east, PI=south, 3/2 PI=west. Unit: radian; Range: [0..2PI].
- double sunElevation = worldOptions.SunElevation; // Solar elevation angle, 0=x/y plane, PI/2=zenith. Unit: rad; Range: [-PI..PI].
- double fogVisualRange = worldOptions.FogVisualRange; // Unit: m; Range: [0..inf[.
- double frictionScaleFactor = worldOptions.FrictionScaleFactor; // Friction scale factor. Range: [0..inf[

NEW
- string dateTime = "2019-06-25T12:00:00";
- int sunAzimuth = 0;
- double fogVisualRange = 100000.0;
-----------------------------------------
ACTIONS
Route:
- List<string[]> waypoints, the 'points' on a route of a vehicle or pedestrian
  string[] has always length 4. It contains the coordinates x,y,z,h
Speed:  
    If you want to change the speed of a vehicle/pedestrian.
    Or If you want to start/stop a vehicle/pedestrian.
- double speedActionDynamicsValue = "2.0" // steps how fast to change the speed. 
                              // The higher the value the faster the vehicle will be at the target speed
- double absoluteTargetSpeedValue = "0.0" // how fast the vehicle/pedestrian should be. 0 is stop.
Lane Change of a vehicle:
- double laneChangeActionDynamicsValue = "25.0" // how fast to change the lane
- string entityRef = "adversary0" // which vehicle will change the lane
- int relativeTargetLaneValue = "-1" // example: -1 change one lane
Acquire Position:
- coordinates x,y,z,h
- ?
*/