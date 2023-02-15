import subprocess
import carla 
import time
import os
import sys
import json
#------------------------- SET INPUTS --------------------------------------------------------------------------------#

# SET PATHS IN CONFIG.JSON FILE # 
# 
# TODOS: 
# Implement error catching if paths are missing
# Set spectator to ego vehicle position
# Create one file containing all results (delete seperate json files?)
# Add docstrings

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
                try:
                    print("INFO: Running scenario -> {scenario}...".format(scenario=file))
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
                except Exception as e:
                    print("ERROR: An error occured while running scenario {s_name}.".format(s_name=file))
                    show = input("Show error message?(y/n): ")
                    if show == "y":
                        print(e)
                    else:
                        print("PROMT NOCH NICHT DEFINIERT")
        
        self.create_results_overview()

    def print_subprocess_output(self, result):

        if "Not all scenario tests were successful" in result.stdout.decode("utf-8"):
            print("INFO: Not all scenario tests were successful")
        elif "All scenario tests were passed successfully" in result.stdout.decode("utf-8"):
            print("INFO: All scenario tests were passed successfully")

        if "exception" in result.stderr.decode("utf-8"):
            print(result.stderr.decode("utf-8"))

    def create_result_dir(self):
        self.results_path = "{runner_root}/results".format(runner_root=self.config["PATH_TO_SCENARIO_RUNNER_ROOT"])

        if not os.path.exists(self.results_path):
            os.makedirs(self.results_path)
            print("INFO: Created results directory.")
        else:
            print("INFO: Results directory found.")

    def load_results(self):

        file_list = os.listdir(self.results_path)

        results_dict = {}

        for file in file_list:
            if ".json" in file:
                scenario_results = self.results_path + "/" + file
                with open(scenario_results, 'r') as f:
                    results_dict[file] = json.load(f)

        return results_dict

    def create_results_overview(self):
        result_dict = self.load_results()
        count = 1

        print("----------------- RESULTS OVERVIEW -----------------")
        print("\tSCNEARIO NAMES\t | \tSUCCESS")

        for scenario in result_dict.keys():
            print("\t" + str(count) + ". " + result_dict[scenario]["scenario"] + "\t | \t " + str(result_dict[scenario]["success"]))
            count+=1
        print("----------------------------------------------------")

        self.user_specific_results(result_dict)

    def user_specific_results(self, result_dict:dict):
        indx = list(result_dict.keys())

        print("To get detailed information enter the relevant Scenario number (enter 0 to exit)")

        while True:
            try:
                usr_input = int(input("Scenario Number: "))
            except Exception:
                print("ERROR: Invalid input")
                continue
            if usr_input == 0:
                print("Exiting results...")
                break
            elif usr_input > len(indx):
                print("ERROR: Scenario doesen't exist")
            else:
                print(json.dumps(result_dict[indx[usr_input-1]], indent=2))

def main():
    runner_tool = RunnerTool()
    runner_tool.start_carla()
    runner_tool.runner()

  
if __name__ == "__main__":
    sys.exit(main())