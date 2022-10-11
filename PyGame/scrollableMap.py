import pygame
from pygame import *

SCREEN_SIZE = pygame.Rect((0, 0, 1920, 1010))
SCREEN_SIZE = pygame.Rect((0, 0, 1000, 640))
scale = 5
MAP_SIZE = [scale * 410.68] * 2
MAP_RECT = pygame.Rect((0, 0, scale * 410.68, scale * 410.68))


class CameraAwareLayeredUpdates(pygame.sprite.LayeredUpdates):
    def __init__(self, target):
        super().__init__()
        self.target = target
        self.cam = pygame.Vector2(0, 0)
        if self.target:
            self.add(target)

    def update(self, surface, image):
        super().update()
        if self.target:
            x = -self.target.rect.center[0] + SCREEN_SIZE.width / 2
            y = -self.target.rect.center[1] + SCREEN_SIZE.height / 2
            self.cam += (pygame.Vector2((x, y)) - self.cam) * 0.05
            self.cam.x = max(-(MAP_RECT.width - SCREEN_SIZE.width), min(0, self.cam.x))
            self.cam.y = max(-(MAP_RECT.height - SCREEN_SIZE.height), min(0, self.cam.y))
        for spr in self.sprites(): # only sprite is CameraSprite
            surface.blit(image, spr.rect.move(self.cam))


def main():
    pygame.init()

    map = pygame.transform.scale(pygame.image.load("../img/Town10HD.png"), MAP_SIZE)

    screen = pygame.display.set_mode(SCREEN_SIZE.size)

    pygame.display.set_caption("Use arrows to move!")

    camera = CameraSprite((0,0))
    entities = CameraAwareLayeredUpdates(camera)

    while 1:
        for e in pygame.event.get():
            if e.type == QUIT:
                return
            if e.type == KEYDOWN and e.key == K_ESCAPE:
                return

        entities.update(screen, map)

        pygame.display.update()

        print(entities.cam, camera.rect)


class CameraSprite(pygame.sprite.Sprite):
    def __init__(self, pos, *groups):
        super().__init__(*groups)
        self.vel = pygame.Vector2((0, 0))
        self.speed = 2
        self.image = Surface((2, 2))
        self.image.fill(Color("#ff0015"))
        self.rect = self.image.get_rect(topleft=pos)

    def update(self):
        self.image = Surface((0, 0))
        self.image.fill(Color("#ff0015"))
        pressed = pygame.key.get_pressed()
        up = pressed[K_UP]
        left = pressed[K_LEFT]
        right = pressed[K_RIGHT]
        down = pressed[K_DOWN]
        running = pressed[K_SPACE]

        if up:
            self.vel.y = +self.speed
        if left:
            self.vel.x = +self.speed
        if right:
            self.vel.x = -self.speed
        if down:
            self.vel.y = -self.speed
        if running:
            self.vel.x *= 5
            self.vel.y *= 5
        if not (left or right):
            self.vel.x = 0
        if not (up or down):
            self.vel.y = 0

        # increment in x direction
        self.rect.left += self.vel.x
        # increment in y direction
        self.rect.top += self.vel.y

        # do x-axis collisions
        # self.collide(self.vel.x, 0, self.platforms)

        # do y-axis collisions
        # self.collide(0, self.vel.y, self.platforms)

    # def collide(self, xvel, yvel, platforms):
    #     for p in platforms:
    #         if pygame.sprite.collide_rect(self, p):
    #             if isinstance(p, ExitBlock):
    #                 pygame.event.post(pygame.event.Event(QUIT))
    #             if xvel > 0:
    #                 self.rect.right = p.rect.left
    #             if xvel < 0:
    #                 self.rect.left = p.rect.right
    #             if yvel > 0:
    #                 self.rect.bottom = p.rect.top
    #                 self.onGround = True
    #                 self.vel.y = 0
    #             if yvel < 0:
    #                 self.rect.top = p.rect.bottom


if __name__ == "__main__":
    main()