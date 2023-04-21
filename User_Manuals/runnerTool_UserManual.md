# runnerTool User Manual

runnerTool is a terminal based python application based on Carlas scenario_runner.py to run and evaluate multiple openSCENARIO (.xosc) files.

## Table of Contents

- [Description](#description)
- [How to set up](#how-to-set-up)
- [How to use](#how-to-use)

## Description

This manual provides basic guidance on how to use runnerTool as a user. See [runnerTool developer manual](https://github.com/jodi106/AI_Testing_Simulator/blob/main/Developer-Documentation/runnerTool_Developer_Documentation.md) for details on the code structure.
runnerTool can be used independent of the ScenarioEditor to run any .xosc file compatible with Carlas scenario_runner.py. It was developed for Carla version 0.9.13 but should also support newer versions (not tested).

## How to set up

**Recommended min system requirements (I7700, 16GB RAM, GTX1070)**

1. Make sure you have installed Carla [See How To Use](https://github.com/jodi106/AI_Testing_Simulator#how-to-setup).
2. [Donwload the latest runnerTool version](https://github.com/jodi106/AI_Testing_Simulator/releases) and place the runnerTool folder in a directory of your choosing.
3. Set paths in the config.json accordingly (use forward slashes to seperate dirs):
   * **PATH_TO_CARLA_ROOT:** Path to folder containing carla exe. It is located in "XX/CARLA/WindowsNoEditor" depending where you unpacked Carla to.
   * **PATH_TO_SCENARIO_RUNNER_ROOT:** Path to runnerTool folder from step 2.
   * **PATH_TO_XOSC_FILES:** Path to folder where .xosc files are stored. The default folder is "./scenarios" i.e. the "scenarios" folder in the runnerTool folder.
   * **PATH_TO_PYTHON_ENV:** Path to the Carla python env. The folder that contains the python.exe. E.g. "xx/Miniconda/envs/{NAME_OF_ENV}" or "xx/Anaconda3/envs/{NAME_OF_ENV}"
   * **CARLA_VERSION:** Carla version you are using. Default: "0.9.13"
4. Place the .xosc files you want to run and evaluate in the Folder you specified (PATH_TO_XOSC_FILES)

runnerTool automatically sets up scenario_runner.py with the paths provided in the config file.

## How to use

After completing How to set up you can now run 

* Starting runnerTool:

  * Using Anaconda Prompt:
     * activate your carla environment (conda activate {NAME_OF_ENV})
     * cd to runnerTool folder (cd {PATH_TO_SCENARIO_RUNNER_ROOT})
     * run runnerTool.py with your required parameters.   
     * Use "python runnerTool.py -h" to get list of available parameters

  * Using regular cmd terminal
     * cd to runnerTool folder (cd {PATH_TO_SCENARIO_RUNNER_ROOT})
     * run runnerTool.py with your required parameters.
     * run runnerTool.py with your required parameters. Use "{PATH_TO_PYTHON_ENV}/python runnerTool.py -h" to get list of parameters

* Evaluating results:

  After all scenarios were executed the terminal will show an overview of all scenarios, indicating whether they were passed succesfully by the Ego-Vehicle or not. 
  It is now possible to enter prompts, to evaluate the scenarios. Use "h" to receive info on possible inputs. To exit the evaluation and close runnerTool insert "0".
