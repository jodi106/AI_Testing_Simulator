import subprocess
import carla 
import time
import os
import sys
import py_trees
from argparse import Namespace

#------------------------- SET INPUTS --------------------------------------------------------------------------------#

PATH_TO_CARLA_ROOT = "C:/CARLA/WindowsNoEditor"
PATH_TO_SCENARIO_RUNNER_ROOT = "C:/Users/jonas/OneDrive - bwedu/Studium_Master/2_HWS_2022-23/AI Testing Simulator/code/AI_Testing_Simulator/scripts/runnerTool"
PATH_TO_XOSC_FILES = "C:/Users/jonas/OneDrive - bwedu/Studium_Master/2_HWS_2022-23/AI Testing Simulator/code/AI_Testing_Simulator/scripts/runnerTool/scenarios"
CARLA_VERSION = "0.9.13"

#---------------------------------------------------------------------------------------------------------------------#
#---------------------------------------------------------------------------------------------------------------------#
os.environ["CARLA_ROOT"] = PATH_TO_CARLA_ROOT
os.environ["SCENARIO_RUNNER_ROOT"] = PATH_TO_SCENARIO_RUNNER_ROOT
sys.path.append(os.getenv("CARLA_ROOT")+"/PythonAPI/carla/dist/carla-" + CARLA_VERSION + ".egg")
sys.path.append(os.getenv("CARLA_ROOT")+"/PythonAPI/carla/agents")
sys.path.append(os.getenv("CARLA_ROOT")+"/PythonAPI/carla")
import scenario_runner
#---------------------------------------------------------------------------------------------------------------------#


class RunnerTool(object):

    def __init__(self):
        pass

    def start_carla(self):
        self.carla_exe = subprocess.Popen(PATH_TO_CARLA_ROOT+"/CarlaUE4.exe")
        time.sleep(10)
        client = carla.Client('localhost', 2000) 
        world = client.load_world('Town04')

        # implement error caching #

    def run_multiple_scenarios(self):

        folder_addr = PATH_TO_XOSC_FILES
        self.file_list = os.listdir(folder_addr)

        for file in self.file_list:
            if ".xosc" in file:
                openscenario = folder_addr + "\\" + file

                args = Namespace(additionalScenario='', agent=None, agentConfig='', configFile='', debug=False, file=False, 
                            host='127.0.0.1', json=False, junit=False, list=False, openscenario=openscenario, 
                            openscenarioparams=None, output=False, outputDir='', port='2000', randomize=False, record='', reloadWorld=True, repetitions=1, route=None, scenario=None, 
                            sync=False, timeout='10.0', trafficManagerPort='8000', trafficManagerSeed='0', waitForEgo=False)

                scenario_runner.main(args)
        
def main():
    runner_tool = RunnerTool()
    runner_tool.start_carla()
    runner_tool.run_multiple_scenarios()

if __name__ == "__main__":
    sys.exit(main())