from PyGame.Entities.Entity import Pedestrian, Vehicle
from PyGame.Entities.Environment import Environment

class ScenarioInfo:
    def __init__(self, name: str, mapURl: str, env: Environment, pedestrians: list[Pedestrian], vehicles: list[Vehicle],
                 egoVehicle: Vehicle):
        self.egoVehicle: Vehicle = egoVehicle
        self.vehicles: list[Vehicle] = vehicles
        self.pedestrians: list[Pedestrian] = pedestrians
        self.env: Environment = env
        self.name: str = name
        self.mapURL: str = mapURl

