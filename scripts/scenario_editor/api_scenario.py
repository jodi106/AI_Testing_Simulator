# Import the CARLA Python API library and some utils
import carla 
import math 
import random 
import time 
import pandas as pd

def setup_world():
    
    
    client = carla.Client('localhost', 2000) 
    client.set_timeout(30.0)
    

    world = client.get_world() 

    #Check if Town 4 is already loaded, if not load it
    if world.get_map().name != "Carla/Maps/Town04":
        #client.set_timeout(15.0) #BC mostly takes longer than default 5 Seconds
        client.load_world("Town04")
        world = client.get_world() 
        #Setting Spectator Camera Position to the 4-Way Intersection
        spectator = world.get_spectator()
        spectator.set_transform(carla.Transform(carla.Location(x = 255, y=-173, z=40),
        carla.Rotation(pitch=-88.918983, yaw=-89.502739, roll=-0.001539)))
        

    return client


def spawn_entities(client, transform_list, entity_type):
    world = client.get_world()
    bp_lib = world.get_blueprint_library() 
  
    entity_list = []
    for transform in transform_list:
        if entity_type == "C":
            entity_bp = random.choice(bp_lib.filter('vehicle'))
        elif entity_type == "P":
            entity_bp = random.choice(bp_lib.filter('pedestrian'))
        entity =  world.try_spawn_actor(entity_bp, transform)
        entity_list.append(entity) 

    return entity_list

def transform_coordinates(df):
    cars = df[df["EntityType"]=="C"][["xCoord","yCoord","zCoord", "rotationDeg"]]
    passenger = df[df["EntityType"]=="P"][["xCoord","yCoord","zCoord","rotationDeg"]]

    coord_cars, coord_pass = [],[]

    for x in range(len(cars)):
        coord_cars.append(list(cars.iloc[x]))

    for x in range(len(passenger)):
        coord_pass.append(list(passenger.iloc[x]))


    transform_cars = []
    
    for element in  coord_cars:

        Location = carla.Location(x = element[0], y = element[1], z = element[2])
        Rotation = carla.Rotation(0, element[3], 0)
        transform = carla.Transform(Location, Rotation)

        transform_cars.append(transform)


    transform_pass = []
    
    for element in  coord_pass:

        Location = carla.Location(x = element[0], y = element[1], z = element[2])
        Rotation = carla.Rotation(0, element[3], 0)
        transform = carla.Transform(Location, Rotation)

        transform_pass.append(transform)

    return transform_cars, transform_pass

def scale_coords(df):
    carla_urx, carla_ury = 256.63, -170.97
    carla_difx, carla_dify = 28.28, 28.28
    
    df[["xCoord", "yCoord"]] = df[["xCoord", "yCoord"]] / 400

    df["xCoord"] = df["xCoord"] * carla_difx + carla_urx
    df["yCoord"] = df["yCoord"] * carla_dify + carla_ury

    return df

def read_gui_input(path):
    df = pd.read_csv(path)   

    return df

def control_cars(vehicles):
    for car in vehicles[1:]:
        car.apply_control(carla.VehicleControl(throttle = 0.5))

    start_autopilot(vehicles[0])

def control_dummy_passenger(passengers):
    for passenger in passengers:
        passenger.apply_control(carla.WalkerControl(speed = 0.55))

def start_autopilot(vehicle):
    vehicle.set_autopilot(True)



client = setup_world()
df = read_gui_input("data\\AlleRechts.txt")
df = scale_coords(df)
vehicle_list, passenger_list = transform_coordinates(df) 
cars = spawn_entities(client, vehicle_list, "C")
peds = spawn_entities(client, passenger_list, "P")

control_cars(cars)
control_dummy_passenger(peds)

