# AI Testing Simulator

Joint team project of University of Mannheim and the University Babeș-Bolyai of Cluj-Napoca

## Table of Contents

- [Description](#description)
- [How to setup](#how-to-setup)
- [How to use](#how-to-use)
- [Knnown Issues](#known-issues)
- [Credits](#credits)

## Description

The goal of this international team project was the development of a simulated testing environment based on an already existing open-source driving simulator for autonomous driving AIs. The key features are the following: 
1. [ScenarioEditor](https://github.com/jodi106/AI_Testing_Simulator/releases/tag/ScenarioEditor_v1.0): An easy to use OpenSCENARIO format (.xosc) scenario Editor to build short traffic situations to verify if the AI under test is able to safely handle them. 
2. [runnerTool](https://github.com/jodi106/AI_Testing_Simulator/releases/tag/runnerTool_v1.02): A tool which is able to run and evaluate multiple OpenSCENARIO scenarios in Carla.

## How to Setup

### How to setup up the Scenario Editor
1. Download the latest ScenarioEditor release [from here](https://github.com/jodi106/AI_Testing_Simulator/releases/latest)
2. Extract the .zip file to a directory of your choice
3. Run the AI_Testing_Simulator.exe file

General Instructions on how to use the Scenario Editor can be found [here (TODO)](https://example.com/)

### How to setup Carla 0.9.13 (Windows)
We suggest using the Package Installation 0.9.13 for convenient and quick setup. Future verions might cause conflicts (not tested).
1. Create a new python environment
2. Read the ["Before you begin"](https://carla.readthedocs.io/en/0.9.13/start_quickstart/) section in the Carla documentation to check prerequisits.
3. Install python modules "pygame numpy" and "carla" to your python environment. 
   ```
   pip3 install --user pygame numpy
   pip3 install carla
   ```
4. Download and unzip the package version 0.9.13 from [Github](https://github.com/carla-simulator/carla/blob/master/Docs/download.md).
5. Use runnerTool for convenient scenario running.

Refer to the official [Carla Documentation](https://carla.readthedocs.io/en/0.9.13/start_quickstart/) for more detailed instructions and Linux setup.

##### If you are using Conda and have problems with shapely and arcade try the following:
```
conda install -c conda-forge shapely
pip install arcade
```

### How to setup the RunnerTool
* [See runnerTool Manual](https://github.com/jodi106/AI_Testing_Simulator/blob/main/User_Manuals/runnerTool_UserManual.md)

## How to use
##### User Manuals:
* [ScenarioEditor Manual (wip)](https://github.com/jodi106/AI_Testing_Simulator/blob/main/User_Manuals/)
* [runnerTool Manual](https://github.com/jodi106/AI_Testing_Simulator/blob/main/User_Manuals/runnerTool_UserManual.md)

##### Code Documentations for developers:
* [ScenarioEditor Developer Documentation (wip)](https://github.com/jodi106/AI_Testing_Simulator/tree/main/Developer-Documentation)
* [runnerTool Developer Documentation](https://github.com/jodi106/AI_Testing_Simulator/blob/main/Developer-Documentation/runnerTool_Developer_Documentation.md)
* [Docs (wip)](https://github.com/jodi106/AI_Testing_Simulator/tree/main/Docs/html)

## Known Issues
* Runtime Error when starting Carla:
    ```
    RuntimeError: time-out of 5000ms while waiting for the simulator, make sure the simulator is ready and connected to localhost:2000
    ```
    
    Occurs when starting CarlaUE4.exe. If error occurs check task manager for already running “Carla UE4” background process. Kill it and restart CarlaUE4.exe should fix it.
    
* Ego-Vehicle doesn't move although a target has been defined in ScenarioEditor

   ScenarioEditor uses ["simple_vehicle_control"](https://github.com/jodi106/AI_Testing_Simulator/blob/main/scripts/runnerTool/srunner/scenariomanager/actorcontrols/simple_vehicle_control.py) without any additional parameters as its default KI. On some Spawn-Waypoints set for the ego vehicle, the KI doesn't start driving correctly. Make minor changes to the Spawn-Waypoints x-coordinates to fix that or use a different drivig KI. Might also be a general CARLA problem.
   
* Route lane changes sometimes don't work on intersections:

    CARLA has problems with Intersections sometimes. Instead of making the lane change on the intersecttion, the vehicle will do it after it. We are currently not aware of a fix. 

## Credits

This project started in September 2022 as a cooperation of University of Mannheim and UBB Cluj. The team consists of:

- [Felix (Uni MA)](https://github.com/felixkroemer/)
- [Jonas (Uni MA)](https://github.com/jodi106/)
- [Natalie (Uni MA)](https://github.com/Natalie-UniMA/)
- [Stefan (Uni MA)](https://github.com/StayFN/)
- [Armin (UBB)](https://github.com/ArminT28/)
- [David (UBB)](https://github.com/tropper26/)
