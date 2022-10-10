from enum import Enum

from PyGame.Entities.Coord import Coord

class Entity:
    def __init__(self, id: int, path: list[Coord], spawnPos: Coord):
        self.id: int = id
        self.path: list[Coord] = path
        self.spawnPos: Coord = spawnPos

    def __str__(self):
        return f"Entity with id: {self.id} and spawnPos: {self.spawnPos}\nhas path: {self.path}"

    __repr__ = __str__()


class VehicleCategories(Enum):
    Car = 1
    Bike = 2


class Vehicle(Entity):
    def __init__(self, id: int, path: list[Coord], spawnPos: Coord, model: str,
                 category: VehicleCategories):
        super(Vehicle, self).__init__(id, path, spawnPos)
        self.model: str = model
        self.category: VehicleCategories = category

    def __str__(self):
        return f"Vehicle with id: {self.id}, model: {self.model}, category: {self.category} and spawnPos: {self.spawnPos}\nhas path: {self.path}"

    __repr__ = __str__()


class Pedestrian(Entity):
    def __init__(self, id: int, path, spawnPos, model: str):
        super(Pedestrian, self).__init__(id, path, spawnPos)
        self.model: str = model

    def __str__(self):
        return f"Pedestrian with id: {self.id}, model: {self.model}, and spawnPos: {self.spawnPos}\nhas path: {self.path}"

    __repr__ = __str__()
