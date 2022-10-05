# Import the CARLA Python API library and some utils
import carla 
import math 
import random 
import time 
import pandas as pd

def setup_world():

    client = carla.Client('localhost', 2000) 
    world = client.get_world() 

    #Check if Town 4 is already loaded, if not load it
    if world.get_map().name != "Carla/Maps/Town04":
        client.set_timeout(15.0) #BC mostly takes longer than default 5 Seconds
        client.load_world("Town04")

        #Setting Spectator Camera Position to the 4-Way Intersection
        spectator = world.get_spectator()
        spectator.set_location(carla.Location(x = 255, y=-173, z=40))


def spawn_entities(world, transform_list, entity_type):
    bp_lib = world.get_blueprint_library() 
  
    entity_list = []
    for transform in transform_list:
        if entity_type == "c":
            entity_bp = random.choice(bp_lib.filter('vehicle'))
        elif entity_type == "p":
            entity_bp = random.choice(bp_lib.filter('passenger'))
        entity =  world.try_spawn_actor(entity_bp, transform)
        entity_list.append(entity) 

    return entity_list

def transform_coordinates(df):
    df = df
    cars = df[df["EntityType"]=="C"][["xCoord","yCoord","zCoord"]]
    passenger = df[df["EntityType"]=="P"][["xCoord","yCoord","zCoord"]]

    coord_cars, coord_pass = [],[]

    for x in range(len(cars)):
        coord_cars.append([list(cars.iloc[x]), "C"])

    for x in range(len(passenger)):
        coord_pass.append([list(passenger.iloc[x]), "P"])


    return transform_list

def read_gui_input(path):
    df = pd.read_csv(path)   

    return df

def control_dummy_car(vehicle_list):
    for car in vehicle_list:
        car.apply_control(carla.VehicleControl(throttle = 0.5))

def control_dummy_passenger(passenger_list):
    for passenger in passenger_list:
        passenger.apply_control(carla.WalkerControl(speed = 0.55))

def start_autopilot(vehicle):
    vehicle.set_autopilot(True)


read_gui_input(scripts\scenario_editor\data\AllEntitiesSet.txt)
setup_world()

