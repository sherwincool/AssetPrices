Feature: AssetsController - RetrieveSpecificAsset

    Scenario: Successfully retrieve a specific asset
    Given there are existing assets in the system
    When the user requests to retrieve the asset with ID 10
    Then the system should return the details of the asset

    Scenario: Attempt to retrieve a non-existing asset
    Given there is no asset with ID 456 in the system
    When the user requests to retrieve the asset with ID 456
    Then the system should return a "Asset Not Found" error
