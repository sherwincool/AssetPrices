Feature: PricesController - Retrieving Prices

    Scenario: Successfully retrieve all prices
    Given there are existing prices in the system
    When the user requests to retrieve all prices
    Then the system should return a list of all prices

    
    Scenario: No prices available to retrieve
    Given there are no existing prices in the system
    When the user requests to retrieve all prices
    Then the system should return an empty price list

