'''This module defines Telegram Modalities data access components.'''
from frostaura.data_access.modalities_data_access import IModalitiesDataAccess
import whisper
from transformers import pipeline

class TelegramModalitiesDataAccess(IModalitiesDataAccess):
    '''Telegram modalities-related functionality.'''

    def __init__(self, config: dict = {}):
        self.config = config
        self.sound_model = whisper.load_model('small')
        self.image_model = pipeline('image-to-text', model='ydshieh/vit-gpt2-coco-en')

    def sound_file_to_text(self, file_path: str) -> str:
        '''Extract the text transcript for an audio file.'''

        return self.sound_model.transcribe(file_path)['text']

    def image_file_to_text(self, file_path: str) -> str:
        '''Extract the text from an image file.'''

        return self.image_model(file_path)
