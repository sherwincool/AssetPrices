Feature: AssetsController - RetrieveAllAssets

    Scenario: Successfully retrieve all assets
    Given there are existing assets in the system
    When the user requests to retrieve all assets
    Then the system should return a list of all assets


    Scenario: No assets available to retrieve
    Given there are no existing assets in the system
    When the user requests to retrieve all assets
    Then the system should return an empty list