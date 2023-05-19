'''This module defines example subordinate components.'''
from logging import debug
from zeus.engines.subordinate_function import ISubordinateFunction
from zeus.models import ProfitCalculationResult

class ExampleSubordinateFunction(ISubordinateFunction):
    '''Calculations-related functionality using some maths under-the-hood.'''

    def __init__(self):
        pass
