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

        

def spawn_vehicle():
    pass

def transform_coordinates():
    pass

def control_dummy():
    pass

def start_autopilot():
    pass


setup_world()

