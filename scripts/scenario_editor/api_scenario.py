# Import the CARLA Python API library and some utils
import carla 
import math 
import random 
import time 
import pandas as pd

from send_to_carla import run

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
    
    spectator.set_transform(carla.Transform(carla.Location(x=267.260406, y=-182.479721, z=17.611162), carla.Rotation(pitch=-47.555298, yaw=133.529037, roll=-0.000673)))
    
    #spectator.set_transform(carla.Transform(carla.Location(x = 255, y=-173, z=40),
    #carla.Rotation(pitch=-88.918983, yaw=-89.502739, roll=-0.001539)))
    

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

    print(entity_list)
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

    cppmax_x_up, cpmax_x_down, cpmax_y_up, cpmax_y_down = 380, 420, 480, 320


    # Größer 0
    df.loc[df["xCoord"]>=0, ["xCoord"]] = df.loc[df["xCoord"]>=0, ["xCoord"]] /cppmax_x_up
    df.loc[df["yCoord"]>=0, ["yCoord"]] = df.loc[df["yCoord"]>=0, ["yCoord"]] /cpmax_y_up
    # kleiner 0
    df.loc[df["xCoord"]<0, ["xCoord"]] = df.loc[df["xCoord"]<0, ["xCoord"]] /cpmax_x_down
    df.loc[df["yCoord"]<0, ["yCoord"]] = df.loc[df["yCoord"]<0, ["yCoord"]] /cpmax_y_down

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


def start_recording():
    # start a recording for t= 30+5sec. Save recording in file "recording01.log", spawn n=0 cars
    #path = "C:\\Users\\Natalie\\Desktop\\Carla\\WindowsNoEditor"
    path = "C:\\Users\\stefan.stingl\\CARLA_0.9.13\\WindowsNoEditor"
    s1 = "cd " + path + "\\PythonAPI\\examples" + " && "
    s1 = s1 + "python start_recording.py -f " + path + "\\recording01.log -n 0 -t 30" 
    print(s1)
    run(s1)

def replay_recording():
    client.stop_recorder()
    
    #print(client.replay_file("C:\\Users\\Natalie\\Desktop\\recording01.log", 0.0, 30.0, 0, 0))
    #path = "C:\\Users\\Natalie\\Desktop\\Carla\\WindowsNoEditor\\recording01.log"
    path = "C:\\Users\stefan.stingl\\CARLA_0.9.13\\WindowsNoEditor\\recording01.log"
    print(client.replay_file(path, 0.0, 0, 0)) # replay the session

    # show collisions
    print(client.show_recorder_collisions(path, "a", "a"))
    #print(client.show_recorder_actors_blocked(path, 0, 0))

client = setup_world()
df = read_gui_input("data\\AllEntitiesSet.txt")
df = scale_coords(df)
vehicle_list, passenger_list = transform_coordinates(df) 
cars = spawn_entities(client, vehicle_list, "C")
peds = spawn_entities(client, passenger_list, "P")

control_cars(cars)
control_dummy_passenger(peds)
