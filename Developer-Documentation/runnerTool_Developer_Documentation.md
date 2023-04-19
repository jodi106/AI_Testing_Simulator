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

* set_agent(): 

   Sequentially changes HeroAgent in .xosc file to user specified string. This method allows to change the Agent (self driving AI) that is used for the ego vehicle in all scenarios of a runnerTool run. ScenarioEditor uses ["simple_vehicle_control"](https://github.com/jodi106/AI_Testing_Simulator/blob/main/scripts/runnerTool/srunner/scenariomanager/actorcontrols/simple_vehicle_control.py) without additional arguments as default agent. So basically it simply enables the ego vehicle to reach the set destination via the road network with a set speed without considering any traffic elements.  
   
   Note, that by default no additional parameters are set in the .xosc files created by ScenarioEditor to avoid clash when changing agents. Some agents such as the simple_vehicle_control allow for enabling/disabling certain behaviors of the ego vehicle by adding arguments in the .xosc file. One could extend the set_agent() method to change/add parameters to the .xosc file depending on the selected agent if necessary. **In pricipal one could also set the parameters by hardcoding them directly in the python file of the agent to avoid having to change the .xosc file.** Additional parameters for the simple_vehicle_control can be added to the .xosc files like this (lines enclosed by "*"): 

   ```
              <AssignControllerAction>
                <Controller name="HeroAgent">
                  <Properties>
                    <Property name="module" value="simple_vehicle_control" />
                    
                    **<Property name="consider_obstacles" value="true"/>**
                    ...
                    **<Property name="{ARG_NAME}" value="true"/>**
                    
                  </Properties>
                </Controller>
              </AssignControllerAction>
   ```

* start_carla(): 

   Starts Carla.exe if not already running. To do so the [subprocess](https://docs.python.org/3/library/subprocess.html) module is used. The subprocess module basically executes a command that opens the CarlaUE4.exe in an independet new terminal. Can be started with low or normal ("epic") graphic render quality (Only works if carla is not running when executing runnerTool). To start Carla subprocess.Popen() is used. This method fires the command but doesnt wait for the subprocess to finish (i.e., CarlaUE4.exe to close). This is required because we want to have the CarlaUE4.exe running while executing further commands. At the moment a sleep timer of 10 seconds is hardcoded in the start_carla() method. This means that runnerTool waits 10 seconds for Carla to start. On a computer fulfilling the recommended min system requirements (I7700, 16GB RAM, GTX1070) this is enough (You might want to consider increasing the value if on a weak computer).     
   
* adjust_speed(): 

   Speed refers to the speed the scenarios are played with. Thereby this doesnt affect the outcome of the scenario evaluation. It simply shows more frames in less time refer to this [Github Issue](https://github.com/carla-simulator/carla/issues/457) for details. Note, that setting the speed to high might cause carla physics bugs, which will effect evaluation outcome. 
   
   The method creates string to adjust play speed of scenario. The string will be addeed as a parser argument to the subprocess command used in the runner() method. The value is passed first to scenario_runner.py via the parser argument and is then further passed on to scenario_manager.py where the change of speed is finally implemented see chapter scenario_manager.py below for details.
Note that setting speed to 100 again - after it was initially set to some other value - while CarlaUE4.exe is allready running, doesnt work. (simply close carla before new runnerTool session to fix)
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

