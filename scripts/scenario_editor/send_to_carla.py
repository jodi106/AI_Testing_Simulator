import subprocess


  

def run(cmd, get_output: bool = False):
    '''Runs subprocess in command line'''
    output = subprocess.run(cmd, shell = True, capture_output=True)
    
    if output.returncode == 0:
        print("Command executed succesfully")
    else:
        raise Exception("Error occured while executing subprocess command:\n" + str(output.stderr))

    if get_output is True:
        return output.stdout

def build_string(path_to_scenario_runner: str, path_to_file: str):
    '''Builds string to execute carla OPENscenario file '''
    cmd = "python {psc}\\scenario_runner.py --openscenario {ptf}".format(psc=path_to_scenario_runner, ptf = path_to_file )
    return cmd




