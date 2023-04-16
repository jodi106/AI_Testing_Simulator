# AI Testing Simulator

Joint team project of University of Mannheim and the University Babeș-Bolyai of Cluj-Napoca

## Table of Contents

- [Description](#description)
- [How to setup](#how-to-setup)
- [How to use](#how-to-use)
- [Credits](#credits)

## Description

The goal of this international team project is the development of a simulated testing environment based on an already existing open-source driving simulator for autonomous driving AIs. The key features are the following: An easy to use scenario Editor to build simulate short traffic situations to verify if the AI under test is able to safely handle them. A tool which is able to record and process the sensor data which is produced by the simulated cars sensors during the test scenarios. A rudimentary driving AI which can be customized to do several dangerous or faulty behaviors which will be used to check wether sets of test cases are able to detect the dangerous or faulty behavior.

## How to Setup

### How to setup up the Scenario Editor
1. Download the latest release (Build_JJJJ.MM.DD.zip) [from here](https://github.com/jodi106/AI_Testing_Simulator/releases/latest)
2. Extract the .zip file to a directory of your choice
3. Run the AI_Testing_Simulator.exe file

General Instructions on how to use the Scenario Editor can be found [here (TODO)](https://example.com/)
runnerTool User Manual
Scenario Editor User Manual


### How to setup the RunnerTool


### How to setup Carla and Scenario Runner (Windows)
We refer to the official [Carla Documentation](https://carla.readthedocs.io/en/latest/start_quickstart/)
And the official [Scenario Runner Documentation](https://carla-scenariorunner.readthedocs.io/en/latest/getting_scenariorunner/#a.-download-a-scenariorunner-release)

#### Common Problems
##### You need to set the following PATH Variables:

```
set CARLA_ROOT=/path/to/your/carla/installation
set PYTHONPATH=$PYTHONPATH:${CARLA_ROOT}/PythonAPI/carla/dist/carla-.egg:${CARLA_ROOT}/PythonAPI/carla/agents:${CARLA_ROOT}/PythonAPI/carla
set CARLA_ROOT=\path\to\your\carla\installation
set SCENARIO_RUNNER_ROOT=\path\to\your\scenario\runner\installation
set PYTHONPATH=%CARLA_ROOT%\PythonAPI\carla\dist\carla-0.9.13.egg
set PYTHONPATH=%PYTHONPATH%;%CARLA_ROOT%\PythonAPI\carla\agents
set PYTHONPATH=%PYTHONPATH%;%CARLA_ROOT%\PythonAPI\carla
```

##### If you are using Conda and have problems with shapely and arcade try the following:
```
conda install -c conda-forge shapely
pip install arcade
```

## How to use

*TODO*

## Credits

This project started in September 2022 as a cooperation of University of Mannheim and UBB Cluj. The team consists of:

- [Armin](https://github.com/ArminT28/)
- [David](https://github.com/tropper26/)
- [Felix](https://github.com/felixkroemer/)
- [Jonas](https://github.com/jodi106/)
- [Natalie](https://github.com/Natalie-UniMA/)
- [Stefan](https://github.com/StayFN/)
