'''This module is a bootstrapper for the Zeus program.'''

from frostaura.managers import IZeusManager, TelegramZeusManager

zeus: IZeusManager = TelegramZeusManager()

zeus.run()
