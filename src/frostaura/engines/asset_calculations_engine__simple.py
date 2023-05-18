'''This module defines example subordinate components.'''
from logging import debug
from frostaura.engines.subordinate_function import ISubordinateFunction
from frostaura.models import ProfitCalculationResult

class ExampleSubordinateFunction(ISubordinateFunction):
    '''Calculations-related functionality using some maths under-the-hood.'''

    def __init__(self,
                 config: dict = {}):
        self.config = config
