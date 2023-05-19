'''This module is a bootstrapper for the Zeus program.'''

from zeus.managers import IZeusManager, TelegramZeusManager

zeus: IZeusManager = TelegramZeusManager()

zeus.run()
