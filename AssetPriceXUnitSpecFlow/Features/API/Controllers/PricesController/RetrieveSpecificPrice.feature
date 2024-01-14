Feature: PricesController - RetrieveSpecificPrice

    Scenario: Successfully retrieve a specific price
    Given there are existing prices in the system
    When the user requests to retrieve the price with ID 1
    Then the system should return the details of the price

    
    Scenario: Attempt to retrieve a non-existing price
    Given there are existing prices in the system
    When the user requests to retrieve the price with ID 456
    Then the system should return a "Price Not Found" error

