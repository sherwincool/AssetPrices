Feature: AssetsController - PostAsset

    #ISIN "US1234567890" is a new asset
    Scenario: Successfully add a new asset
    Given there are existing assets in the system
    When the user submits a new asset with the ISIN "US1234567890"
    Then the system should add the new asset to the database

    #ISIN "US5949181045" is an existing asset
    Scenario: Attempt to add a duplicate asset
    Given there are existing assets in the system
    When the user submits a new asset with the ISIN "US5949181045"
    Then the system should return a "The Asset ISIN is already existing." error
