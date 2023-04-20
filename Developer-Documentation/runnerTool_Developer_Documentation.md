# runnerTool Developer Documentation

runnerTool is a terminal based python application based on CARLAs scenario_runner.py to run and evaluate multiple openSCENARIO (.xosc) files.

This manual provides basic guidance for developers that want to work with runnerTool. It gives general insight on the code structure and main features. 

[See runnerTool User Manual](https://github.com/jodi106/AI_Testing_Simulator/blob/main/User_Manuals/runnerTool_UserManual.rst) for instructions on how to set up and use runnerTool. 
runnerTool can be used independent of the ScenarioEditor to run any .xosc file compatible with CARLAs scenario_runner.py. 
It was developed for CARLA version 0.9.13 but should also support newer versions (not tested).

## Table of Contents

- [General Code Structure](#General-Code-Structure)
- [runnerTool.py](#runnerTool.py)
- [scenario_runner.py](#scenario_runner.py)
- [scenario_manager.py](#scenario_manager.py)
- [Hints for future implementations/ Changes](#Hints-for-future-implementations/-Changes)

## General Code Structure

runnerTool is build on top of CARLAs scenario_runner.py. See [CARLA ScenarioRunner](https://carla-scenariorunner.readthedocs.io/en/latest/) for detailed description of ScenarioRunner. Docstrings are available in the scripts for all implemented methods.

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

   Speed refers to the speed the scenarios are played with. Thereby this doesnt affect the outcome of the scenario evaluation. It simply shows more frames in less time. Refer to this [Github Issue](https://github.com/carla-simulator/carla/issues/457) for details. Note, that setting the speed too high (500-1000 depending on the map) might cause carla physics bugs, which will effect evaluation outcome.  Note, that changing the speed will not work with the --camera flag when running runnerTool, as the camera forces carla to run in regular speed. 
   
   The method creates string to adjust play speed of scenario. The string will be addeed as a parser argument to the subprocess command used in the runner() method. The value is passed first to scenario_runner.py via the parser argument and is then further passed on to scenario_manager.py where the change of speed is finally implemented. See chapter [scenario_manager.py](https://github.com/jodi106/AI_Testing_Simulator/blob/main/Developer-Documentation/runnerTool_Developer_Documentation.md#scenario_manager.py) below for details.
Note that setting speed to 100 again - after it was initially set to some other value - while CarlaUE4.exe is allready running, doesnt work. (simply close carla before new runnerTool session to fix)

* set_camera_perspective(): 

    RunnerTool uses the ["visualizer.py"](https://github.com/jodi106/AI_Testing_Simulator/blob/main/scripts/runnerTool/srunner/scenariomanager/actorcontrols/visualizer.py) module to create two camera sensors fixed to the ego vehicle (bird and ego perspective) and displays them in a seperate window when the user enabled the flag --camera. 
    
    Similar to the adjust_speed() method this methood creates a string aswell that will be addeed as a parser argument to the subprocess command used in the runner() method. The value is also passed first to scenario_runner.py via the parser argument and is then further passed on to scenario_manager.py where the visualilzer module is implemented. See chapter [scenario_manager.py](https://github.com/jodi106/AI_Testing_Simulator/blob/main/Developer-Documentation/runnerTool_Developer_Documentation.md#scenario_manager.py) below for details. 
    
* **runner()**: 

    This method runs all n .xosc files in specified dir by executing scenario_runner.py in n subsequent subprocesses. First it loads all .xosc files from the directory using get_xosc() method. If specefied by the user the files will be sorted by map names in the .xosc file. Then for each .xosc file it sets the agent via set_agent() if specified by user. Then it will create the cmd string for each file to be used in the subprocess that runs scenario_runner.py. Thereby it uses fixed and optional parser arguments:
     * #### Fixed (all vanilla scenario_runner parser arguments): 
       * **--openscenario {file}:** This enables running scenarios of type OpenSCENARIO using as specified .xosc {file}
       * **--reloadWorld:** This reloads the CARLA world for each new scenario, even if it runs on the same map. This is necessary as broken scenarios stopped by the timeout don't remove all actors from the map. So without reloading the world, new actors of the new scenario would just be spawned additionally - if it runs on the same map.
       * **--json:** This creates a result overview in json format indicating whether or not the ego vehicle passed all tests in the scenario specified in the .xosc file  
       * **--outputDir** {result_path}: This sets the output directory for the json file.
       
       There are multiple additional parser arguments that runnerTool doesnt use when running scenario_runner in the subprocess. See [scenario_runner.py docstrings](https://github.com/jodi106/AI_Testing_Simulator/blob/main/scripts/runnerTool/scenario_runner.py) for details.
    * #### Optional (additional parser arguments implemented with runnerTool):
      * **--speed:** enables setting the scenario speed as described in adjust_speed()
      * **--camera:** enables using the camera as described in set_camera_perspective()

    Using the cmd with above described arguments runner() crates a new subprocess, running the scenario_runner.py with the current .xosc file - with or without the output of the scenario_runnner.py script (user --debug argument). The subprocess will be terminated by defualt after 200 + 10 seconds or after user specified + 10 seconds via the --timeout argument. Thereby 10 seconds are added to account for the time it might take to switch and reload maps. The terminal output in case of scenario output does not include the extra 10 seconds.
    
    After all scenarios were run runner() calls the create_results_overview() method to display results.

* create_results_overview():
    
    This method is used to create scenario success overview of all scenario results. To do so it loads the .json files in results dir that have been created by scenario_editor.py using the --json flag. For each scenario it retrieves the overall scenario success status and prints them to the console and stores them in the Log. If a scenario wasn't succesfull it will be written in red. Aterwards the function user_specific_results() is called. 
    
    Note, that create_results_overview() is initialy called by runner(), when this is the case "call" == true. This variable is used to avoid storing the same files multiple times as create_results_overview() can be called multiple times from the user_specific_results() method. In this case "call" == False.
    
* user_specific_results():
    
    This method enables terminal based evaluation of the scenario after the runs are finished. It takes different user inputs to show the result overview again, show the failed scenarios, show specific scenarios induvidually in detail and show the available commands to the user. **This method runs infinitely until the user enters "0" wich terminates runnerTool.** 

#### Log
An object of type Log is created on initialization of a RunnerTool object. It is used to log outputs of runnerTool. See docstrings for method descriptions. 

## scenario_runner.py
This section only describes changes made to the file by runnerTool. See [CARLA ScenarioRunner](https://carla-scenariorunner.readthedocs.io/en/latest/) for documentation on the vanilla features.

runnerTool adds the following features to scenario_runner.py:
* set_global_variables() (lines 33-51): 

    This method is called directly at the beginning of the script to set the required environment variables for scenario_runner.py. It uses the paths in the runnerTool/config.json file specified by the user. 
* runnerTool args to parser (lines 596-598): 

    Adding the arguments "--speed" and "--camera" to the argument parser.
* adding runnerTool Params (lines 104-105,128): 

    Adding the values from "--speed" and "--camera" to local variables on ScenarioRunner objcet initialisation. Adding runnerTool parameters to the ScenarioManager object constructor.


## scenario_manager.py
This section only describes changes made to the file by runnerTool. See [CARLA ScenarioRunner](https://carla-scenariorunner.readthedocs.io/en/latest/) for documentation on the vanilla features.

runnerTool adds the following features to scenario_manager.py:
* set_speed() (lines 46,71,127-128, 141-150):

   Changes scenario display speed using the values described in previous sections that were passed through runnerTool.py and scenario_runner.py. It changes the fixed_delta_seconds parameter of the CARLA world as described in this [Github Issue](https://github.com/carla-simulator/carla/issues/457). To set the frames via this parameter you have to use the reciprocal (1/x) value of the desired frame rate. The default value is around 1/140 FPS. So to adjust the speed, 140 is adjusted by the user passed parameter. E.g., if the user passed "--speed 200" - essentially doubling the speed - the formular would be 1/(140/(200/100)). This essentially halves the amount of time that elapses between two sent frames.
   
   Note that 140 is only approximately the default value, i.e., 100% play speed, and might change depending on the mashine. Therefore the method sends None to the fixed_delta_seconds parameter if the user doesn't change the speed. None will cause the scenario to run exactly at normal speed.
* get_camera() (lines 46,70, 133, 152-158):

    This methods creates initial spectator position above ego vehicle upon loading the scenario in CARLA. It is not influenced by the --camera flag. And is not optional. (Can of course be disabled in the code if its not required). It simply gets the coordinates of the ego vehicle and spawns a spectator facing down above it. This method is called in the vanilla load_scenario() method of scenario_manager.py.  
* reset_camera() (currently not implemeted) (lines: 160-177, 192, 197-200, 207):

    Was initially used to update the spectator every k ticks to the new location of the ego vehicle. Very unstable and does not allow user to navigate in the CarlaUE4.exe. Was removed with runnerTool v1.01 and replaced by Visualizer module.
* Visualizer (lines: 46, 73, 104-106, 130-131, 239-241):

    This module provides an OpenCV based visualizer for camera sensors
attached to an actor. It exists in the vanilla version of ScenarioRunner but is not implemented directly in the code. Usually it gets called via a parameter in the OpenScenario file. RunnerTool implements it in a way that allows it to be used for every scenario run in runnerTool by simply inputting the --camera argument. See [Visualizer](https://github.com/jodi106/AI_Testing_Simulator/blob/main/scripts/runnerTool/srunner/scenariomanager/actorcontrols/visualizer.py) for description of the Visualizer class.  
