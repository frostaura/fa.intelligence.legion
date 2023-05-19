'''This module defines valuation model components.'''
from enum import Enum

class VisualizationType(Enum):
    '''A collection of supported visualization types.'''

    LINE = 0
    PIE = 1
    BAR = 2
