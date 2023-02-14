import subprocess
import carla 
import time
import os
import sys
import json
#------------------------- SET INPUTS --------------------------------------------------------------------------------#

#PATH_TO_CARLA_ROOT = "C:/CARLA/WindowsNoEditor"
#PATH_TO_SCENARIO_RUNNER_ROOT = "C:/Users/jonas/OneDrive - bwedu/Studium_Master/2_HWS_2022-23/AI Testing Simulator/code/AI_Testing_Simulator/scripts/runnerTool"
#PATH_TO_XOSC_FILES = "C:/Users/jonas/OneDrive - bwedu/Studium_Master/2_HWS_2022-23/AI Testing Simulator/code/AI_Testing_Simulator/scripts/runnerTool/scenarios"
#CARLA_VERSION = "0.9.13"

#---------------------------------------------------------------------------------------------------------------------#
#---------------------------------------------------------------------------------------------------------------------#
#os.environ["CARLA_ROOT"] = PATH_TO_CARLA_ROOT
#os.environ["SCENARIO_RUNNER_ROOT"] = PATH_TO_SCENARIO_RUNNER_ROOT
#sys.path.append(os.getenv("CARLA_ROOT")+"/PythonAPI/carla/dist/carla-" + CARLA_VERSION + ".egg")
#sys.path.append(os.getenv("CARLA_ROOT")+"/PythonAPI/carla/agents")
#sys.path.append(os.getenv("CARLA_ROOT")+"/PythonAPI/carla")
#import scenario_runner
#---------------------------------------------------------------------------------------------------------------------#

# SET PATHS IN CONFIG.JSON FILE # (todo: implement error catching if paths are missing)

class RunnerTool(object):

    def __init__(self):
        self.config = self.get_config()
        self.create_result_dir()

    def get_config(self):
        with open(os.path.join(sys.path[0],'config.json'), 'r') as f:
                conf = json.load(f)
        return conf

    def start_carla(self):
        try:
            client = carla.Client('localhost', 2000) 
            world = client.get_world()
            print("INFO: Carla is already running, starting scenario..")
        except Exception:
            print("INFO: Carla not Running, Starting exe..")
            self.carla_exe = subprocess.Popen(self.config["PATH_TO_CARLA_ROOT"]+"/CarlaUE4.exe")
            time.sleep(10)
            client = carla.Client('localhost', 2000) 
            world = client.get_world()

    def runner(self):
        conf = self.config
        
        folder_addr = conf["PATH_TO_XOSC_FILES"]
        file_list = os.listdir(folder_addr)

        print("INFO: Found scenarios in directory:\n" + '\n'.join([x for x in file_list if ".xosc" in x]))
        for file in file_list:
            if ".xosc" in file:
                print("INFO: Running scenario -> {scenario}".format(scenario=file))
                openscenario = folder_addr + "/" + file

                cmd = """cd \"{runner_root}\"\
                            &set CONDAPATH=\"{conda_root}\"\
                            &set ENVNAME={envname}\
                            &if %ENVNAME%==base (set ENVPATH=%CONDAPATH%) else (set ENVPATH=%CONDAPATH%/envs/%ENVNAME%)\
                            &call %CONDAPATH%/Scripts/activate.bat %ENVPATH%\
                            &python scenario_runner.py --openscenario \"{file}\" --json --outputDir \"{result_path}\"
                        """.format(runner_root= conf["PATH_TO_SCENARIO_RUNNER_ROOT"],conda_root=conf["PATH_TO_CONDA_ROOT"],envname=conf["ENVNAME"],file=openscenario, result_path=self.results_path)
                result = subprocess.run(cmd, shell=True, capture_output=True)
                self.print_subprocess_output(result)

    def print_subprocess_output(self, result):

        if "Not all scenario tests were successful" in result.stdout.decode("utf-8"):
            print("INFO: Not all scenario tests were successful")
        elif "All scenario tests were passed successfully" in result.stdout.decode("utf-8"):
            print("INFO: All scenario tests were passed successfully")


    def create_result_dir(self):
        self.results_path = "{runner_root}/results".format(runner_root=self.config["PATH_TO_SCENARIO_RUNNER_ROOT"])

        if not os.path.exists(self.results_path):
            os.makedirs(self.results_path)
            print("INFO: Created results directory.")
        else:
            print("INFO: Results directory found.")

def main():
    runner_tool = RunnerTool()
    runner_tool.start_carla()
    runner_tool.runner()

  
if __name__ == "__main__":
    sys.exit(main())