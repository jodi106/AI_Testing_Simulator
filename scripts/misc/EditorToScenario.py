# Import the CARLA Python API library and some utils
import carla 
import math 
import random 
import time 

# Connect to the client and get the world object
client = carla.Client('localhost', 2000) 

#world = client.get_world() 
world = client.load_world('Town04')

# Set Spectator view for the 
spectator = world.get_spectator()

# Get Vehicle Lib and Spawn_Points
bp_lib = world.get_blueprint_library() 
spawn_points = world.get_map().get_spawn_points() 

# Get the blueprint for the vehicle you want
vehicle_bp = bp_lib.find('vehicle.lincoln.mkz_2020') 
# Try spawning the vehicle at a randomly chosen spawn point

location = carla.Location(carla.Location(x = 255, y=-173, z=0.3))
rotation = carla.Rotation(pitch=0.000000, yaw=0.000000, roll=0.000000)
transform = carla.Transform(location, rotation)

vehicle = world.try_spawn_actor(vehicle_bp, transform)

#spectator.set_location(carla.Location(transform.location))
spectator.set_location(carla.Location(x = 255, y=-173, z=40))


vehicle.apply_control(carla.VehicleControl(throttle = 0.5))


