'''This module defines valuation model components.'''

class ValuationResult:
    '''A model with all the post-valuation data for an asset.'''

    def __init__(self,
                 symbol: str,
                 company_name: str,
                 current_price: float,
                 valuation_method: str,
                 valuation_price: float,
                 margin_of_safety: float):
        assert margin_of_safety > 0 and margin_of_safety < 1

        self.symbol = symbol
        self.company_name = company_name
        self.current_price = current_price
        self.valuation_method = valuation_method
        self.valuation_price = valuation_price
        self.fair_price = valuation_price * (1 - margin_of_safety)
        self.absolute_current_v_valuation_delta = 1 - (min(self.fair_price, current_price) / max(self.fair_price, current_price))
        self.is_overvalued = self.fair_price < current_price
        self.margin_of_safety = margin_of_safety
        self.divident_payout_frequency_in_months = 0 # TODO: Fetch this data.
        self.annual_dividend_percentage = 0 # TODO: Fetch this data.
