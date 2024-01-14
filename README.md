# AssetPrices
- Created BDDs Unit test in Specflow for for PricesController and AssetsController
- for Creating and updating an Asset (POST and PUT), validate if there is already asset with the same ISIN to proceed with the process.
- Retrieve prices for a specific date, assets, and source
-   assets is searched by ISINs
-   source is searched by Name
-   specific date is allowed to search, date range should be considered
-   minimized the If/Else by simplifying the LINQ query
- for updating a prices (PUT: api/Prices/5), check first if there is an entry with the same date, source and asset. If there is already an entry with different Id, throw a "duplicate price" error.
- for creating a price entry (POST: api/Prices), check first if there is an entry with the same date, source and asset. If there is already an entry, update that entry else create a new one.
- Source was treated as new entity for data normalization.
- created a repositories to wrap the DbContext methods since I experieced an error in mocking the extension methods
Unsupported expression: ... => ....AnyAsync<Asset>(It.IsAny<CancellationToken>()) Extension methods (here: EntityFrameworkQueryableExtensions.AnyAsync) may not be used in setup / verification expressions.
