from xml.dom import minidom
import os
import define_entities
import init_entities
import maneuver_entities
from create_xosc_gui_helper import get_number_of_cars_without_ego, get_vehicle_list


# Needed values (todo get it from scenario_editor)
number_of_simulation_cars = get_number_of_cars_without_ego() # 0 - 99
vehicle_model_ego = "vehicle.volkswagen.t2"
vehicle_model_simulation = "vehicle.audi.tt"
map = "Town04"
x = ['255.7', '290', '255'] # index 0 is ego, index 1+ is simulation vehicle position
y = ['-145.7', '-172', '-190']
z = ['0.3', '0.3', '0.3']
h = ['200', '180', '90']
control_mode = "carla_auto_pilot_control" # var 1: to use rename file carla_autopilot.py in carla_auto_pilot_control.py
# The path is in the folder: \PythonAPI\scenario_runner-0.9.13\srunner\scenariomanager\actorcontrols
#control_mode = "external_control" # var 2: to use with manual_control.py
speed = 3.0
start_after_x_seconds = 1
stop_after_x_seconds = 20


def create_xosc_file():
    root = minidom.Document()
    open_scenario = root.createElement('OpenSCENARIO')  
    
    # Define entities which are used (cars, pedestrians, bicicles, ...)
    define_entities.add_basic_tags(root, open_scenario, map)
    define_entities.add_entities(root, open_scenario, number_of_simulation_cars, vehicle_model_ego, vehicle_model_simulation)

    storyboard = root.createElement('Storyboard')
    open_scenario.appendChild(storyboard)

    # Place the defined entities at specific coordinates on the map
    init_entities.add_init(root, storyboard, number_of_simulation_cars, x, y, z, h, control_mode)
    
    # Start moving the entities: Todo make this more dynamic to be able to handle more different maneuvers.
    maneuver_entities.accelerate_all_simulation_cars(root, storyboard, number_of_simulation_cars, speed, start_after_x_seconds, stop_after_x_seconds)

    xml_str = root.toprettyxml(indent ="\t") 
    save_path_file = "OurScenario.xosc"
    with open(save_path_file, "w") as f:
        f.write(xml_str) 


#####################################################

# Driver Code
if __name__ == "__main__": 
    create_xosc_file()
    print("finished!")