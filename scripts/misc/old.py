import subprocess
import carla 
import time
import os
import sys
import py_trees
import copy
from argparse import Namespace



def clear_blackboard(self):
    blackboard = py_trees.blackboard.Blackboard()
    blackboard_keys = copy.copy(blackboard.__dict__)
    delete = py_trees.blackboard.ClearBlackboardVariable()
    for k in blackboard_keys.keys():
        delete.variable_name = k
        delete.initialise()
    print("Blackboard cleared")
    print(blackboard.__dict__.keys())

def run_multiple_scenarios(self):

    folder_addr = PATH_TO_XOSC_FILES
    self.file_list = os.listdir(folder_addr)

    for file in self.file_list:
        #gc.collect()
        if ".xosc" in file:
            openscenario = folder_addr + "/" + file

            args = Namespace(additionalScenario='', agent=None, agentConfig='', configFile='', debug=False, file=False, 
                        host='127.0.0.1', json=True, junit=False, list=False, openscenario=openscenario, 
                        openscenarioparams=None, output=True, outputDir=PATH_TO_XOSC_FILES, port='2000', randomize=False, record='', reloadWorld=True, repetitions=1, route=None, scenario=None, 
                        sync=False, timeout='10.0', trafficManagerPort='8000', trafficManagerSeed='0', waitForEgo=False)

            scenario_runner.main(args)
