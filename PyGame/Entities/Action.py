from PyGame.Entities.Coord import Coord
from PyGame.Entities.Entity import Entity


class Action:
    def __init__(self, id: int, pos: Coord, involvedEntities: list[Entity], type: str, trigger, field):
        self.id = id
        self.pos = pos
        self.involvedEntities = involvedEntities
        self.type = type
        self.trigger = trigger
        self.field = field
