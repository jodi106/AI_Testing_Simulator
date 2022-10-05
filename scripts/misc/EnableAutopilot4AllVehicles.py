# Import the CARLA Python API library and some utils
import carla 
import math 
import random 
import time     


# Connect to the client and get the world object
client = carla.Client('localhost', 2000) 
world = client.get_world() 




# Set the all vehicles in motion using the Traffic Manager
for v in world.get_actors().filter('*vehicle*'): 
    v.set_autopilot(True)