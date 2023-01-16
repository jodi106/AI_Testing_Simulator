import subprocess
import carla 
import time
import os
import sys
import py_trees

os.environ["CARLA_ROOT"] = 'C:/CARLA/WindowsNoEditor'
os.environ["SCENARIO_RUNNER_ROOT"] = "C:/Users/jonas/OneDrive - bwedu/Studium_Master/2_HWS_2022-23/AI Testing Simulator/code/AI_Testing_Simulator/scripts/runnerTool"
sys.path.append(os.getenv("CARLA_ROOT")+"/PythonAPI/carla/dist/carla-0.9.13.egg")
sys.path.append(os.getenv("CARLA_ROOT")+"/PythonAPI/carla/agents")
sys.path.append(os.getenv("CARLA_ROOT")+"/PythonAPI/carla")

import scenario_runner
from argparse import Namespace

cmd = "C:\\CARLA\\WindowsNoEditor\\CarlaUE4.exe"

process = subprocess.Popen(cmd)
time.sleep(10)

client = carla.Client('localhost', 2000) 
world = client.load_world('Town04')


def run_multiple_scenarios(openscenario):

    

    """     spectator = world.get_spectator()
    spectator.set_location(carla.Location(x = 255, y=-173, z=40)) """

    args = Namespace(additionalScenario='', agent=None, agentConfig='', configFile='', debug=False, file=False, 
                host='127.0.0.1', json=False, junit=False, list=False, openscenario=openscenario, 
                openscenarioparams=None, output=False, outputDir='', port='2000', randomize=False, record='', reloadWorld=True, repetitions=1, route=None, scenario=None, 
                sync=False, timeout='10.0', trafficManagerPort='8000', trafficManagerSeed='0', waitForEgo=False)



    scenario_runner.main(args)
    

scenarios = ['C:\\CARLA\\scenario_runner-master\\srunner\\examples\\OurStartStopLC.xosc','C:\\CARLA\\scenario_runner-master\\srunner\\examples\\OurStartStopLC.xosc']

for scenario in scenarios:
    print("current dir::")
    print(dir())
    print(py_trees.blackboard.Blackboard().__str__())
    run_multiple_scenarios(scenario)
    print(py_trees.blackboard.Blackboard().__str__())
