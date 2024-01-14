Feature: PricesController - GetPriceByDate

    Scenario: Successfully retrieve prices by date
    Given there are existing prices in the system
    And the user wants to retrieve prices from source "xxx"
    And the user wants to retrieve prices from assets with ISIN "US0378331005"
    When the user requests to retrieve prices by date "2021-01-01"
    Then the system should return a list of prices for that date

    
    Scenario: No prices available for the specified date
    Given there are existing prices in the system
    When the user requests to retrieve prices by date "2021-01-05"
    Then the system should return an empty price list
