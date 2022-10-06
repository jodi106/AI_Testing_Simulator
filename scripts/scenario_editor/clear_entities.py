import carla 

client = carla.Client('localhost', 2000) 
client.set_timeout(30.0)


world = client.get_world() 

for i in world.get_actors():
    i.destroy()