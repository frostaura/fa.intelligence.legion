'''This module defines profit calculation model components.'''

class ProfitCalculationResult:
    '''A model with all the post-profit calculation data for an asset.'''

    def __init__(self, percentage: float, value: float):
        self.percentage = percentage
        self.value = value
