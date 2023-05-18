'''This module defines data access components related to sensory modalities.'''

class IModalitiesDataAccess:
    '''Component to perform functions related to sensory modalities like imagine, sound and text'''

    def sound_file_to_text(self, file_path: str) -> str:
        '''Extract the text transcript for an audio file.'''

        raise NotImplementedError()

    def image_file_to_text(self, file_path: str) -> str:
        '''Extract the text from an image file.'''

        return NotImplementedError()
