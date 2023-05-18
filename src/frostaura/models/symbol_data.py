'''This module definessymbol data model components.'''

class SymbolData:
    '''A model with all the detailed information for a symbol.'''

    def __init__(self,
                 symbol: str,
                 company_name: str,
                 current_price: float,
                 eps: float,
                 annual_growth_projected: float,
                 current_yield_of_aaa_corporate_bonds: float = 4.27, # https://fred.stlouisfed.org/series/AAA
                 pe_base_non_growth_company: float = 8.5,
                 average_yield_of_aaa_corporate_bonds: float = 4.4):
        self.symbol = symbol
        self.company_name = company_name
        self.current_price = current_price
        self.eps = eps
        self.annual_growth_projected = annual_growth_projected
        self.current_yield_of_aaa_corporate_bonds = current_yield_of_aaa_corporate_bonds
        self.pe_base_non_growth_company = pe_base_non_growth_company
        self.average_yield_of_aaa_corporate_bonds = average_yield_of_aaa_corporate_bonds
