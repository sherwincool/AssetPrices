Feature: PricesController - PutPrice

    Scenario: Successfully update an existing price
    Given there are existing prices in the system
    And there is a price with ID 1
    When the user submits an update for the price with ID 1
    Then the system should update the price details

    Scenario: Attempt to update with a duplicate price
    Given there are existing prices in the system
    And there is another price with a conflicting value
    When the user submits an update for the price with ID 1
    Then the system should return a "The update will introduce a duplicate price in a different entry." error
    
    Scenario: Attempt to update a non-existing price
    Given there are existing prices in the system
    When the user submits an update for the price with ID 999
    Then the system should return a "Price Not Found" error

