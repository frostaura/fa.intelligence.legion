'''This module defines Zeus management for Telegram components.'''

import os
from frostaura.managers.zeus_manager import IZeusManager

class TelegramZeusManager(IZeusManager):
    '''Component to perform functions related to managing Zeus for Telegram.'''

    def __init__(self):
        self.telegram_bot_token = os.environ['TELEGRAM_BOT_TOKEN']

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc_value, t_b):
        pass

    def run(self) -> None:
        '''Initialize the Zeus program.'''

        raise NotImplementedError()
