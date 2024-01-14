Feature: AssetsController - PutAsset

    Scenario: Successfully update an existing asset
    Given there are existing assets in the system
    When the user submits an update for the asset with ID 10
    Then the system should update the asset details

    Scenario: Attempt to update with an existing ISIN
    Given there are existing assets in the system
    When the user attempts to update the asset ID 789 with the conflicting ISIN
    Then the system should return a "The Asset ISIN is already exist." error

    Scenario: Attempt to update a non-existing asset
    Given there is no asset with ID 999 in the system
    When the user attempts to update the asset with ID 999
    Then the system should return a "Asset Not Found" error
