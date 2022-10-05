# Import the CARLA Python API library and some utils
import carla 
import math 
import random 
import time 

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

    return client


def spawn_entities(client, transform_list, entity_type):
    world = client.get_world()
    bp_lib = world.get_blueprint_library() 
  
    entity_list = []
    for transform in transform_list:
        if entity_type == "C":
            entity_bp = random.choice(bp_lib.filter('vehicle'))
        elif entity_type == "P":
            entity_bp = random.choice(bp_lib.filter('passenger'))
        entity =  world.try_spawn_actor(entity_bp, transform)
        entity_list.append(entity) 

    return entity_list

def transform_coordinates(df):



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

def control_cars(vehicles):
    for car in vehicles[1]:
        car.apply_control(carla.VehicleControl(throttle = 0.5))

    start_autopilot(vehicles[0])

def control_dummy_passenger(passengers):
    for passenger in passengers:
        passenger.apply_control(carla.WalkerControl(speed = 0.55))

def start_autopilot(vehicle):
    vehicle.set_autopilot(True)


client = setup_world()
df = read_gui_input("data\\AllEntitiesSet.txt")
vehicle_list, passenger_list = transform_coordinates(df) 
control_cars(spawn_entities(client, vehicle_list, "C"))
control_dummy_passenger(spawn_entities(client, passenger_list, "P"))

def recorder():
    # execute python file
    # check output
    # szenario was succesfull or not