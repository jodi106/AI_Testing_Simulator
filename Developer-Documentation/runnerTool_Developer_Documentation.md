# runnerTool Developer Documentation

runnerTool is a terminal based python application based on Carlas scenario_runner.py to run and evaluate multiple openSCENARIO (.xosc) files.

This manual provides basic guidance for developers that want to work with runnerTool. It gives general insight on the code structure and main features. 

[See runnerTool User Manual](https://github.com/jodi106/AI_Testing_Simulator/blob/main/User_Manuals/runnerTool_UserManual.rst) for instructions on how to set up and use runnerTool. 
runnerTool can be used independent of the ScenarioEditor to run any .xosc file compatible with Carlas scenario_runner.py. 
It was developed for Carla version 0.9.13 but should also support newer versions (not tested).

## Table of Contents

- [General Code Structure](#General-Code-Structure)
- [runnerTool.py](#runnerTool.py)
- [scenario_runner.py](#scenario_runner.py)
- [scenario_manager.py](#scenario_manager.py)
- [Hints for future implementations/ Changes](#Hints-for-future-implementations/-Changes)

## General Code Structure

runnerTool is build on top of Carlas scenario_runner.py. See [CARLA ScenarioRunner](https://carla-scenariorunner.readthedocs.io/en/latest/) for detailed description of ScenarioRunner. Docstrings are available in the scripts for all implemented methods.

The following scripts have been added/modified for runnerTool compared to the vanilla CARLA ScenarioRunner:
1. [runnerTool.py](https://github.com/jodi106/AI_Testing_Simulator/blob/main/scripts/runnerTool/runnerTool.py): The majority of the code for runnerTool is implemented here.
2. [scenario_runner.py](https://github.com/jodi106/AI_Testing_Simulator/blob/main/scripts/runnerTool/scenario_runner.py): Setting Global Variables and using runnerTool Parser arguments. 
3. [scenario_manager.py](https://github.com/jodi106/AI_Testing_Simulator/blob/main/scripts/runnerTool/srunner/scenariomanager/scenario_manager.py): Camera and Speed settings

## runnerTool.py

This is the main module of runnerTool. Contains main method, RunnerTool class and Log class.

#### Main Method
The main method parses the arguments provided by the user, initiates an object of type RunnerTool using the parsed arguments and calls the methods RunnerTool.start_carla() and RunnerTool.runner().

#### RunnerTool
The RunnerTool class includes the most important methods used for runnerTool. The following will describe the most important methods. See docstrings in the code for descriptioins of all methods.

* set_agent(): wip
* start_carla(): wip 
* adjust_speed(): wip
* set_camera_perspective(): wip
* **runner()**: wip
* create_results_overview(): wip
* user_specific_results(): wip

#### Log
An object of type Log is created on initialization of a RunnerTool object. It is used to log outputs of runnerTool. See docstrings for method descriptions. 

## scenario_runner.py
wip

## scenario_manager.py
wip

## Hints for future implementations/ Changes
wip

```
put code here
```

