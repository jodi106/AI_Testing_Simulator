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
x = ['-13', '363', '26'] # index 0 is ego, index 1+ is simulation vehicle position
y = ['357', '16', '-217']
z = ['0.3', '0.3', '0.3']
h = ['1,57', '1.57', '1.57']
control_mode = "carla_auto_pilot_control" # var 1: to use rename file carla_autopilot.py in carla_auto_pilot_control.py
control_mode = "external_control" # var 2


def create_xosc_file():
    root = minidom.Document()
    open_scenario = root.createElement('OpenSCENARIO')  
  
    define_entities.add_basic_tags(root, open_scenario, map)
    define_entities.add_entities(root, open_scenario, number_of_simulation_cars, vehicle_model_ego, vehicle_model_simulation)
    init_entities.add_storyboard(root, open_scenario, number_of_simulation_cars, x, y, z, h, control_mode)
    
    xml_str = root.toprettyxml(indent ="\t") 
    save_path_file = "OurScenario.xosc"
    with open(save_path_file, "w") as f:
        f.write(xml_str) 


#####################################################

# Driver Code
if __name__ == "__main__": 
    create_xosc_file()
    print("finished!")