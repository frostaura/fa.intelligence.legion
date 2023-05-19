'''This module defines Zeus management for Telegram components.'''
import os
from telegram import Update
from frostaura.data_access import INotificationsDataAccess
from frostaura.data_access import TelegramNotificationsDataAccess
from zeus.managers.zeus_manager import IZeusManager
from zeus.data_access import IModalitiesDataAccess
from zeus.data_access import TelegramModalitiesDataAccess

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
