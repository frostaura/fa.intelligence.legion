'''This module defines subordinate functions components.'''
from frostaura.models import ProfitCalculationResult

class ISubordinateFunction:
    '''The base to all subordinate functions.'''

    def calculate_holdings_profits(self, holdings: pd.DataFrame) -> pd.DataFrame:
        '''Determine individual asset profit ratio & profit USD and interpolate them into a copy of the given holdings.'''

        raise NotImplementedError()
