import subprocess
import carla 
import time
from datetime import datetime
import os
import sys
import json

from colorama import init, Fore, Back, Style
init()
#------------------------- SET INPUTS --------------------------------------------------------------------------------#

# SET PATHS IN CONFIG.JSON FILE # 
# 
# TODOS: 
# Set spectator to ego vehicle position (scenario_runner.py)
# - check manual_control.py for spectator updates
# How to add Ego Vehicle -> try agent attribute
# Add command line parser for runnerTool
# python C:\CARLA\WindowsNoEditor\PythonAPI\util\config.py --fps 20 -> set fixed frame rate 50.00 milliseconds (20 FPS)




# vehicle_bp = bp_lib.find('vehicle.lincoln.mkz_2020') 
# vehicle = world.try_spawn_actor(vehicle_bp, spawn_points[79])

#spectator = world.get_spectator() 
#transform = carla.Transform(vehicle.get_transform().transform(carla.Location(x=-4,z=2.5)),vehicle.get_transform().rotation) 
#spectator.set_transform(transform)

class RunnerTool(object):

    def __init__(self, detailed_log = False, checks = True, save_overview = False, speed = 100):

        self.log = Log()
        self.config = self.get_config()
        if checks:
            self.check_paths(self.config)
        self.create_result_dir()
        self.detailed_log = detailed_log
        self.save_overview = save_overview
        self.speed = speed


    def create_scenario_runner_params(self, fps=40):
        params = {}
        params["fps"] = fps

        temp_path = "{runner_root}/temp".format(runner_root=self.config["PATH_TO_SCENARIO_RUNNER_ROOT"])

        if not os.path.exists(temp_path):
            os.makedirs(temp_path)
            self.log.create_entry("INFO: Created temp directory.")
        else:
            self.log.create_entry("INFO: Results temp found.")


        with open(temp_path+ '/temp.json', 'w', encoding='utf-8') as fp:
            json.dump(params, fp, sort_keys=False, indent=4)

    def get_config(self):
        ''' Loads required user specified paths from config.json file.'''
        try:         
            with open(os.path.join(sys.path[0],'config.json'), 'r') as f:
                    conf = json.load(f)
            return conf

        except Exception: 
            self.log.create_entry("ERROR: Couldn't import config.json from path: {path}".format(path=os.path.join(sys.path[0],'config.json')))
            raise

    def check_paths(self, conf:dict):
        '''
        Checks whether paths were specified by user and are valid.

        Parameters
        ----------
        conf : dict
            Dictonary containing user specified paths

        Returns
        -------
        None
        '''
        k = 0
        for key in conf.keys():
            if "INSERT_PATH" in conf[key]:
                self.log.create_entry('Path for {path} not specified in config.json'.format(path=key))
                raise ValueError(self.log.get_top(print_out=False))
            if not os.path.exists(conf[key]) and k < 4:
                self.log.create_entry('Path {path} specified in config.json does not exist'.format(path=key))
                raise ValueError(self.log.get_top(print_out=False))
            k+=1

    def start_carla(self, host = 'localhost', port = 2000, low_quality = False):
        ''' Starts Carla.exe if not already running.'''
        quality = "Epic"
        if low_quality:
            quality = "Low"

        try:
            client = carla.Client(host, port) 
            world = client.get_world()
            self.log.create_entry("INFO: Carla is already running, starting scenario..")
        except Exception:
            self.log.create_entry("INFO: Carla not Running, Starting exe..")
            self.carla_exe = subprocess.Popen(self.config["PATH_TO_CARLA_ROOT"]+"/CarlaUE4.exe -quality-level={quality}".format(quality=quality))
            time.sleep(10)
            client = carla.Client(host, port) 
            world = client.get_world()

    def adjust_speed(self):
        ''' Adjust play speed of scenario'''

        speed_cmd = " --speed %i" %self.speed
        self.log.create_entry("INFO: Setting Scenario display Speed to %.2fX" %(self.speed/100))
        return speed_cmd

    def runner(self):
        ''' Runs all n .xosc files in specified dir by executing scenario_runner.py in n subprocesses'''
        conf = self.config

        folder_addr = conf["PATH_TO_XOSC_FILES"]
        file_list = os.listdir(folder_addr)
        self.log.create_entry("INFO: Found scenarios in directory:\n" + '\n'.join([x for x in file_list if ".xosc" in x]))
        for file in file_list:
            if ".xosc" in file:
                try:
                    self.log.create_entry("INFO: Running scenario -> {scenario}...".format(scenario=file))
                    openscenario = folder_addr + "/" + file
                    # Building subprocess command. (Subprocess is executed in new cmd terminal, thus python env root is required.)
                    cmd = """cd \"{runner_root}\"\
                                &{python_root}/python scenario_runner.py --openscenario \"{file}\" --json --outputDir \"{result_path}\"{speed}
                            """.format(runner_root= conf["PATH_TO_SCENARIO_RUNNER_ROOT"],
                                       python_root=conf["PATH_TO_PYTHON_ENV"],
                                       file=openscenario, 
                                       result_path=self.results_path, 
                                       speed=self.adjust_speed())
                    result = subprocess.run(cmd, shell=True, capture_output=True)
                    self.print_subprocess_output(result)
                except Exception as e:
                    self.log.create_entry("ERROR: An error occured while running scenario {s_name}.".format(s_name=file))
                    show = input("Show error message?(y/n): ")
                    if show == "y":
                        self.log.create_entry(e)
        
        self.create_results_overview()

    def create_linux_cmd(self):
        # activate python env source path/to/virtual/environment/bin/activate
        pass


    def print_subprocess_output(self, result):
        '''
        Prints individual scenario results directly after execution.

        Parameters
        ----------
        results : subprocess object
            A subprocess object containing return values from executed subprocess.run() method

        Returns
        -------
        None
        '''
        if "Not all scenario tests were successful" in result.stdout.decode("utf-8"):
            self.log.create_entry("INFO: Not all scenario tests were successful")
        elif "All scenario tests were passed successfully" in result.stdout.decode("utf-8"):
            self.log.create_entry("INFO: All scenario tests were passed successfully")

        if "exception" in result.stderr.decode("utf-8"):
            self.log.create_entry(result.stderr.decode("utf-8"))

    def create_result_dir(self):
        ''' Creates new dir in RUNNER_ROOT to store scenario results, if none exists.'''
        self.results_path = "{runner_root}/results".format(runner_root=self.config["PATH_TO_SCENARIO_RUNNER_ROOT"])

        if not os.path.exists(self.results_path):
            os.makedirs(self.results_path)
            self.log.create_entry("INFO: Created results directory.")
        else:
            self.log.create_entry("INFO: Results directory found.")

    def load_results(self):
        '''
        Loads scenario results .json files form result dir.

        Parameters
        ----------
        None

        Returns
        -------
        results_dict : dict
            Dictonary containing all scenario results stored in results dir
        '''
        file_list = os.listdir(self.results_path)

        results_dict = {}

        for file in file_list:
            if ".json" in file:
                scenario_results = self.results_path + "/" + file
                with open(scenario_results, 'r') as f:
                    results_dict[file] = json.load(f)
                
        return results_dict

    def create_results_overview(self, call = True):
        ''' Creates scenario success overview of all scenario results .json files in results dir'''
        
        result_dict = self.load_results()
        overview = ["SCNEARIO NAMES | SUCCESS"]
        count = 1

        self.log.create_entry("----------------- RESULTS OVERVIEW -----------------")
        print("\tSCNEARIO NAMES\t | \tSUCCESS")
        self.log.create_entry("SCNEARIO NAMES | SUCCESS", print_result=False)

        for scenario in result_dict.keys():
            if result_dict[scenario]["success"]:
                print("\t" + str(count) + ". " + result_dict[scenario]["scenario"] + "\t | \t " + str(result_dict[scenario]["success"]))
                self.log.create_entry(str(count) + ". " + result_dict[scenario]["scenario"] + " | " + str(result_dict[scenario]["success"]), print_result=False)
                overview.append(self.log.get_top(print_out=False))
            else:
                print("\t" + str(count) + ". " + Fore.RED + result_dict[scenario]["scenario"] + Style.RESET_ALL +"\t | \t " + Fore.RED + str(result_dict[scenario]["success"]) + Style.RESET_ALL)
                self.log.create_entry(str(count) + ". " + result_dict[scenario]["scenario"]  +" | "  + str(result_dict[scenario]["success"]), print_result =False)
                overview.append(self.log.get_top(print_out=False))
            count+=1
        self.log.create_entry("----------------------------------------------------")
        if call:
            self.user_specific_results(result_dict)
            self.log.append_dict("All_Results",result_dict)

        if self.detailed_log:
            self.log.store(self.config["PATH_TO_SCENARIO_RUNNER_ROOT"] + "/")
        if self.save_overview:
            file = open("{path}/Scenario_Overview_{date}.txt".format(path = self.config["PATH_TO_SCENARIO_RUNNER_ROOT"],
                                                                    date = datetime.now().strftime('%Y-%m-%d-%H-%M-%S')),
                                                                    'w')
            for item in overview:
                file.write(item+"\n")
            file.close()

    def get_failed(self, result_dict:dict, print_out = True):
        failed = []
        if print_out:
            self.log.create_entry("----------------- FAILED SCENARIOS -----------------")
        for key in result_dict.keys():
            if not result_dict[key]["success"]:
                criteria = []
                s = result_dict[key]["scenario"] + " (Failed Tests: "
                for criteria_dict in result_dict[key]["criteria"]:
                    if not criteria_dict["success"]:
                        criteria.append(criteria_dict["name"])
                s = s + ','.join(criteria) + ")"
                failed.append(s)
            if print_out:
                self.log.create_entry(s)
            
        return failed


    def user_specific_results(self, result_dict:dict):
        '''
        Loads scenario results .json files form result dir.

        Parameters
        ----------
        result_dict : dict
            Dictonary containing all scenario results stored in results dir

        Returns
        -------
        None
        '''
        indx = list(result_dict.keys())

        self.log.create_entry("Browse Scenario Results (enter 0 to exit, h for help)")
        
        while True:
            try:
                usr_input = input("Enter Command(0 to exit, h for help): ")
                try:
                   usr_input = int(usr_input)
                except Exception:
                    pass
            except Exception:
                self.log.create_entry("ERROR: Invalid input")
                continue
            if usr_input == "f":
                self.get_failed(result_dict)
            elif usr_input == 0:
                self.log.create_entry("Exiting results...")
                break
            elif type(usr_input) == int and usr_input > len(indx):
                self.log.create_entry("ERROR: Scenario doesen't exist")
            elif type(usr_input) == int:
                print(json.dumps(result_dict[indx[usr_input-1]], indent=2))
                self.log.create_entry("-- LOG_INFO: Detailed Scenario Info omitted for Logfile. See All Results. LOG_INFO --", print_result=False)
            elif usr_input == "o":
                self.create_results_overview(call=False)
            else:
                print("""\n----------------- HELP AVAILABLE COMMANDS -----------------:
                Exit: 0
                Scenario Details: [Scenario_Number]
                List of failed scenarios: \"f\"
                Scenario Overview: \"o\"\n""")

class Log:
    
    def __init__(self):
        self.logfile = {}
        self.count = 1

    def create_entry(self, entry: str, print_result = True):
        self.logfile[str(self.count) + ": " + str(datetime.now())[10:19]] = entry
        if print_result:
            self.get_top(print_result)
        self.count+=1

    def get_log(self):
        return self.logfile
    
    def get_top(self, print_out):
        keys = list(self.logfile.keys())
        if print_out:
            print(self.logfile[keys[-1]])

        return self.logfile[keys[-1]] 
    
    def append_dict(self, name, dict):
        self.logfile[name] = dict
    
    def store(self, path:str=""):
        with open(path+ 'runnerTool_Log_{date}.json'
                  .format(date = datetime.now().strftime('%Y-%m-%d-%H-%M-%S')), 
                  'w', encoding='utf-8') as fp:
            json.dump(self.logfile, fp, sort_keys=False, indent=4)


def main():

    ##### TO DO: implement command line parser #####

    runner_tool = RunnerTool(checks = True, detailed_log=True, save_overview=True, speed=100)
    runner_tool.start_carla(low_quality=True)
    runner_tool.runner()
    #runner_tool.create_results_overview()
  
if __name__ == "__main__":
    sys.exit(main())