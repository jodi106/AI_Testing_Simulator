import pygame
from pygame import *

SCREEN_SIZE = (1080,720) #(1920, 1080)
scale = 1
PAN_SPEED = 2

def main():
    pygame.init()

    surface = pygame.image.load("../img/Town10HD.png")

    screen = pygame.display.set_mode(SCREEN_SIZE)

    offset = [0, 0]
    mouseDown = False

    while True:
        for e in pygame.event.get():
            if e.type == QUIT:
                return
            if e.type == KEYDOWN and e.key == K_ESCAPE:
                return

            if e.type == pygame.MOUSEBUTTONDOWN:
                mouseDown = True
                x,y = e.pos
                print(x + offset[0], y + offset[1])
            elif e.type == pygame.MOUSEMOTION:
                if mouseDown:  # dragging
                    xa, ya = e.rel
                    offset[0] -= xa
                    offset[1] -= ya
            elif e.type == pygame.MOUSEBUTTONUP:
                mouseDown = False

        pressed = pygame.key.get_pressed()
        if pressed[K_LEFT]:
            offset[0] -= PAN_SPEED
        if pressed[K_RIGHT]:
            offset[0] += PAN_SPEED
        if pressed[K_UP]:
            offset[1] -= PAN_SPEED
        if pressed[K_DOWN]:
            offset[1] += PAN_SPEED

        screen.blit(surface, (0, 0), (offset[0], offset[1], SCREEN_SIZE[0], SCREEN_SIZE[1]))

        pygame.display.flip()


if __name__ == "__main__":
    main()
