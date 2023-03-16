import subprocess
import carla 
import time
from datetime import datetime
import os
import sys
import json
import argparse
from argparse import RawTextHelpFormatter

from colorama import init, Fore, Back, Style
init()

RUNNER_TOOL_VERSION = "1.0"

#------------------------- SET INPUTS --------------------------------------------------------------------------------#
# SET PATHS IN CONFIG.JSON FILE # 
#---------------------------------------------------------------------------------------------------------------------#

class RunnerTool(object):
    '''
    Inititates runnerTool object. Checks config paths, creates result and log dir if not exist.

    Attributes
    ----------
    log : Log
        Log object to create logfile
    config : dict
        Contains required dir paths set in config.json
    host : 
        IP of the host server (default: localhost)
    port : int
        TCP port to listen to (default: 2000)
    checks : bool
        Disable checks for config.json
    detailed_log : bool  
        Saves detailed json log to root dir
    save_overview : bool
        Saves scenario success overview txt to root dir
    failed_scenarios : bool
        Saves failed scenarios overview txt to root dir
    low_quality : bool
        Set Carla renderquality to low
    speed : int
        Play speed of scenario in percent(Default=100). Doesn't effect (time)metrics. Max stable value 1000 (10X Speed)
    camera : str{bird, ego} 
        Set camera perspectiv (bird, ego) fixed to ego vehicle (default: None)

    Methods
    -------
    get_config()
        Loads required user specified paths from config.json file.
    check_paths(conf)
        Checks whether paths were specified by user and are valid.
    set_agent(file)
        Opens specified file and replaces HeroAgent value with self.agent string.
    start_carla()
        Starts Carla.exe if not already running.
    adjust_speed()
        Create string to adjust play speed of scenario.
    set_camera_perspective()
        Create string tp adjust camera perspective.
    runner()
        Runs all n .xosc files in specified dir by executing scenario_runner.py in n subprocesses.
    print_subprocess_output(result)
        Prints subprocess output.
    create_result_dir()
        Creates new dir in RUNNER_ROOT to store scenario results, if none exists.
    create_logs_dir()
        Creates new dir in RUNNER_ROOT to store logs and overview files, if none exists.'
    load_results()
        Loads scenario results .json files form result dir.
    create_results_overview(call=True)
        Creates scenario success overview of all scenario results .json files in results dir.
    get_failed(result_dict, print_out = True)
        Creates list of failed scenarios including the respective failed tests.
    user_specific_results(result_dict)
        Takes user input to browse results.
    '''

    def __init__(self, args:argparse.Namespace):
        '''
        Parameters
        ----------
        args : Namespace
            Namespace containing command line arguments
        '''
        self.log = Log()
        self.config = self.get_config()
        self.checks = args.noChecks
        if self.checks:
            self.check_paths(self.config)
        self.create_result_dir()
        self.create_logs_dir()
        self.detailed_log = args.log
        self.save_overview = args.overview
        self.failed_scenarios = args.failed
        self.speed = args.speed
        self.camera = args.camera
        self.agent = args.agent

        # Args for start_carla
        self.host = args.host
        self.port = args.port
        self.low_quality = args.lowQuality

    def get_config(self):
        ''' 
        Loads required user specified paths from config.json file.
        
        Returns
        -------
        conf: dict
            Dict containing config file with required paths
        '''
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

    def set_agent(self, file:str):
        '''
        Opens specified file and replaces HeroAgent value with self.agent string.
        
        Parameters
        ----------
        file : str
            full path to .xosc file

        '''
        try:
            with open(file, 'r') as f:
                xosc = f.read() 

            heroindex = xosc.index("HeroAgent")
            valueindex = heroindex + xosc[heroindex:].index("value")
            quot1 = valueindex + xosc[valueindex:].index("\"")
            quot2 = quot1 + xosc[quot1+1:].index("\"")
            controller = xosc[quot1+1:quot2+1]
            xosc = xosc.replace(controller, self.agent)

            with open(file, "w") as f:
                f.write(xosc)
            self.log.create_entry("INFO: Agent name changed from \"{old}\" to \"{new}\"".format(old=controller, new=self.agent))
        except Exception as e:
            self.log.create_entry("ERROR: Could not change Agent name due to exception: {error}".format(error=e))

    def start_carla(self):
        ''' Starts Carla.exe if not already running.'''
        
        quality = "Epic"
        if self.low_quality:
            quality = "Low"

        try:
            client = carla.Client(self.host, self.port) 
            world = client.get_world()
            self.log.create_entry("INFO: Carla is already running, starting scenario..")
            if self.low_quality:
                self.log.create_entry("INFO: Changing renderquality when carla is already running is not possible! Keeping old settings.")
        except Exception:
            self.log.create_entry("INFO: Carla not Running, Starting exe..")
            self.carla_exe = subprocess.Popen(self.config["PATH_TO_CARLA_ROOT"]+"/CarlaUE4.exe -quality-level={quality}".format(quality=quality))
            time.sleep(10)
            client = carla.Client(self.host, self.port) 
            world = client.get_world()

    def adjust_speed(self):
        ''' 
        Create string to adjust play speed of scenario.

        Returns
        -------
        speed_cmd: str
            String in command line argument format containing user defined or default value for scenario display speed.
        '''
        speed_cmd = " --speed %i" %self.speed
        self.log.create_entry("INFO: Setting Scenario display Speed to %.2fX" %(self.speed/100))
        return speed_cmd
    
    def set_camera_perspective(self):
        ''' 
        Create string tp adjust camera perspective.

        Returns
        -------
        camera_cmd: str
            String in command line argument format containing user defined or default value for camera perspective.
        '''
        if self.camera:
            camera_cmd = " --camera %s" %self.camera
        else:
            camera_cmd=""
        return camera_cmd

    def runner(self):
        ''' Runs all n .xosc files in specified dir by executing scenario_runner.py in n subprocesses.'''
        conf = self.config

        folder_addr = conf["PATH_TO_XOSC_FILES"]
        file_list = os.listdir(folder_addr)
        self.log.create_entry("INFO: Found scenarios in directory:\n" + '\t\n'.join([x for x in file_list if ".xosc" in x]))
        for file in file_list:
            if ".xosc" in file:
                try:
                    openscenario = folder_addr + "/" + file
                    self.log.create_entry("INFO: Running scenario -> {scenario}...".format(scenario=file)) 
                    if self.agent:
                        self.set_agent(openscenario)          
                    # Building subprocess command. (Subprocess is executed in new cmd terminal, thus python env root is required.)
                    cmd = """cd \"{runner_root}\"\
                                &{python_root}/python scenario_runner.py --openscenario \"{file}\" --json --outputDir \"{result_path}\"{speed} {camera}
                            """.format(runner_root= conf["PATH_TO_SCENARIO_RUNNER_ROOT"],
                                       python_root=conf["PATH_TO_PYTHON_ENV"],
                                       file=openscenario, 
                                       result_path=self.results_path, 
                                       speed=self.adjust_speed(),
                                       camera=self.set_camera_perspective())
                    result = subprocess.run(cmd, shell=True, capture_output=True)
                    self.print_subprocess_output(result)

                except Exception as e:
                    self.log.create_entry("ERROR: An error occured while running scenario {s_name}.".format(s_name=file))
                    show = input("Show error message?(y/n): ")
                    if show == "y":
                        self.log.create_entry(e)
        
        self.create_results_overview()

    def print_subprocess_output(self, result: subprocess):
        '''
        Prints subprocess output.

        Parameters
        ----------
        results : subprocess object
            A subprocess object containing return values from executed subprocess.run() method

        Returns
        -------
        None
        '''
        self.log.create_entry("SCENARIO RUNNER STDOUT: {error}".format(error=result.stdout))
        self.log.create_entry("SCENARIO RUNNER STDERR: {error}".format(error=result.stderr))

        if "Not all scenario tests were successful" in result.stdout.decode("utf-8"):
            self.log.create_entry("INFO: Not all scenario tests were successful\n")
        elif "All scenario tests were passed successfully" in result.stdout.decode("utf-8"):
            self.log.create_entry("INFO: All scenario tests were passed successfully\n")

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

    def create_logs_dir(self):
        ''' Creates new dir in RUNNER_ROOT to store logs and overview files, if none exists.'''
        self.logs_path = "{runner_root}/logfiles".format(runner_root=self.config["PATH_TO_SCENARIO_RUNNER_ROOT"])

        if not os.path.exists(self.logs_path):
            os.makedirs(self.logs_path)
            self.log.create_entry("INFO: Created logs directory.")
        else:
            self.log.create_entry("INFO: logs directory found.")

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
        ''' 
        Creates scenario success overview of all scenario results .json files in results dir.
        
        Parameters
        ----------
        call: bool
            Boolean variable to enable/disable calling parts of the function to prevent redundant actions if function is called multiple times. This includes:
            - calling user_specific_results()
            - appending result dict to log
            - storing log.json
            - storing overview.txt
            - storing failed scenarios

        Returns
        -------
        None
        '''        
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

            date = datetime.now().strftime('%Y-%m-%d-%H-%M-%S')

            if self.detailed_log:
                self.log.store(self.logs_path+ "/")

            if self.save_overview:
                file = open("{path}/Scenario_Overview_{date}.txt".format(path = self.logs_path,
                                                                        date = date),'w')
                for item in overview:
                    file.write(item+"\n")
                file.close()
                
            if self.failed_scenarios:
                with open("{path}/Failed_Scenarios_{date}.json".format(path = self.logs_path,
                                                                        date = date),'w', encoding='utf-8') as fp:
                    json.dump(self.get_failed(result_dict, False), fp, sort_keys=False, indent=4)

    def get_failed(self, result_dict:dict, print_out = True):
        '''
        Creates list of failed scenarios including the respective failed tests.

        Parameters
        ----------
        result_dict: dict
            Dictonary containing all scenario results stored in results dir
        print_out: bool
            Enable/Disable printing to console

        Returns
        -------
        failed: list
            List of the names of failed scenarios including the respective failed tests
        '''
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
        Takes user input to browse results.

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
    '''
    Inititates runnerTool object. Checks config paths, creates result and log dir if not exist.

    Attributes
    ----------
    logfile : dict
        Dict for logfile entries
    count : int
        Iterable to create unique log entries

    Methods
    -------
    create_entry(entry, print_result = True)
        Creates new count numbered log entry with timestamp
    get_log()
        Returns logfile
    get_top(print_out)
        Returns most recent log entry
    append_dict(name, dict)
        Appends a dict with name to logfile
    store(path="")
        Stores the logfile at path
    '''
    def __init__(self):
        self.logfile = {}
        self.count = 1

    def create_entry(self, entry: str, print_result = True):
        '''
        Creates new count numbered log entry with timestamp.

        Attributes
        ----------
        entry : str
            Content of log entry as a string
        print_result: bool
            Enables/Disables calling get_top() to print entry to console
        
        Returns
        -------
        None
        '''
        self.logfile[str(self.count) + ": " + str(datetime.now())[10:19]] = entry
        if print_result:
            self.get_top(print_result)
        self.count+=1

    def get_log(self):
        '''
        Returns Logfile.

        Returns
        -------
        logfile : dict
            logfile as dict
        '''
        return self.logfile
    
    def get_top(self, print_out):
        '''
        Returns most recent log entry.

        Attributes
        ----------
        print_out: bool
            Enables/Disables printing entry to console
        
        Returns
        -------
        str
            Most recent logfile element as string
        '''
        keys = list(self.logfile.keys())
        if print_out:
            print(self.logfile[keys[-1]])

        return self.logfile[keys[-1]] 
    
    def append_dict(self, name, dict):
        '''Appends a dict with name to logfile.'''
        self.logfile[name] = dict
    
    def store(self, path:str=""):
        '''Stores the logfile at path'''
        with open(path+ 'runnerTool_Log_{date}.json'
                  .format(date = datetime.now().strftime('%Y-%m-%d-%H-%M-%S')), 
                  'w', encoding='utf-8') as fp:
            json.dump(self.logfile, fp, sort_keys=False, indent=4)


def main():
    description = ("OpenSCENARIO runnerTool for CARLA Simulator: Run and Evaluate multiple OpenScenario scenarios using CARLA\n"
                   "Current version: " + RUNNER_TOOL_VERSION)
   
    parser = argparse.ArgumentParser(description=description,
                                     formatter_class=RawTextHelpFormatter)
    parser.add_argument('-v', '--version', action='version', version='%(prog)s ' + RUNNER_TOOL_VERSION)
    parser.add_argument('--host', default='localhost',
                        help='IP of the host server (default: localhost)')
    parser.add_argument('--port', default=2000, type=int,
                        help='TCP port to listen to (default: 2000)')
    parser.add_argument('--noChecks', default=True, action="store_false", help='Disable checks for config.json')
    parser.add_argument('--log', action="store_true", help='Saves detailed json log to root dir')
    parser.add_argument('--overview', action="store_true", help='Saves scenario success overview txt to root dir')
    parser.add_argument('--failed', action="store_true", help='Saves failed scenarios overview txt to root dir')
    parser.add_argument('--lowQuality', action="store_true", help='Set Carla renderquality to low')   
    parser.add_argument('--speed', default=100, type=int, help='Play speed of scenario in percent(Default=100). Doesn\'t effect (time)metrics.\nValues >500 might lead to physics bugs')
    parser.add_argument('--camera', default=None, type=str, help='Set camera perspectiv (bird, ego) fixed to ego vehicle. Might cause carla crash if bird view is combined with high speed.')
    parser.add_argument('--agent', default=None, type=str, help='Specify agent name to run all scenarios in dir')

    arguments = parser.parse_args()

    runner_tool = RunnerTool(arguments)
    runner_tool.start_carla()
    runner_tool.runner()
    
    #uncomment line below and comment the two lines above to skip directly to results overview without running scenarios
    #runner_tool.create_results_overview()
  
if __name__ == "__main__":
    sys.exit(main())