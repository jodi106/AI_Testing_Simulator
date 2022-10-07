import sys
import pygame
from pygame.locals import *

# Import additional modules here.
from PyGame.Entities.BackGround import Background

# Feel free to edit these constants to suit your requirements.
FRAME_RATE = 60.0
SCREEN_SIZE = (640, 480)

def pygame_modules_have_loaded():
    success = True

    if not pygame.display.get_init:
        success = False
    if not pygame.font.get_init():
        success = False
    if not pygame.mixer.get_init():
        success = False

    return success

pygame.mixer.pre_init(44100, -16, 2, 512)
pygame.init()
pygame.font.init()

if pygame_modules_have_loaded():
    game_screen = pygame.display.set_mode(SCREEN_SIZE, RESIZABLE)
    infoObject = pygame.display.Info()

    pygame.display.set_caption('Test')
    clock = pygame.time.Clock()


    def declare_globals():
        # The class(es) that will be tested should be declared and initialized
        # here with the global keyword.
        # Yes, globals are evil, but for a confined test script they will make
        # everything much easier. This way, you can access the class(es) from
        # all three of the methods provided below.
        pass


    def prepare_test():
        # Add in any code that needs to be run before the game loop starts.
        pass


    def handle_input(key_name):
        # Add in code for input handling.
        # key_name provides the String name of the key that was pressed.
        pass


    def update(screen, time):
        # Add in code to be run during each update cycle.
        # screen provides the PyGame Surface for the game window.
        # time provides the seconds elapsed since the last update.
        pygame.display.update()


    def CreateWindow(width, height):
        '''Updates the window width and height '''
        pygame.display.set_caption("Press ESC to quit")
        game_screen = pygame.display.set_mode((width, height), RESIZABLE)
        game_screen.fill([255, 255, 255])


    # Add additional methods here.

    def main():
        declare_globals()
        prepare_test()
        backGround = Background('MapImages/Town10HD.png', [0, 0])

        while True:
            game_screen.fill([255, 255, 255])
            game_screen.blit(backGround.image, backGround.rect)

            for event in pygame.event.get():
                if event.type == QUIT:
                    pygame.quit()
                    sys.exit()

                if event.type == KEYDOWN:
                    key_name = pygame.key.name(event.key)
                    handle_input(key_name)

                if event.type == VIDEORESIZE:
                    CreateWindow(event.w, event.h)

            milliseconds = clock.tick(FRAME_RATE)
            seconds = milliseconds / 1000.0
            update(game_screen, seconds)

            sleep_time = (1000.0 / FRAME_RATE) - milliseconds
            if sleep_time > 0.0:
                pygame.time.wait(int(sleep_time))
            else:
                pygame.time.wait(1)


    main()
