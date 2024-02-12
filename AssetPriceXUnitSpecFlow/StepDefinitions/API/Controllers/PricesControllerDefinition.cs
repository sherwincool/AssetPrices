using AssetPricesAPI.Controllers;
using AssetPricesAPI.Models;
using AssetPricesAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TechTalk.SpecFlow;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AssetPriceXUnitSpecFlow.StepDefinitions.API.Controllers
{
    [Binding]
    public sealed class PricesControllerDefinition
    {
        private readonly ScenarioContext scenarioContext;

        public PricesControllerDefinition(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        private List<Price> _price = new List<Price>
            {
                new Price() { Id = 1, Date = DateTime.Parse("2021-01-01"), Value = 1.0M, LastUpdated = DateTime.Parse("2021-01-01"), AssetId = 1, SourceId = 1 },
                new Price() { Id = 2, Date = DateTime.Parse("2021-01-02"), Value = 2.0M, LastUpdated = DateTime.Parse("2021-01-01"), AssetId = 1, SourceId = 1 },
                new Price() { Id = 3, Date = DateTime.Parse("2021-01-01"), Value = 3.0M, LastUpdated = DateTime.Parse("2021-01-01"), AssetId = 3, SourceId = 1 },
                new Price() { Id = 4, Date = DateTime.Parse("2021-01-01"), Value = 4.0M, LastUpdated = DateTime.Parse("2021-01-01"), AssetId = 4, SourceId = 1 },
                new Price() { Id = 5, Date = DateTime.Parse("2021-01-01"), Value = 5.0M, LastUpdated = DateTime.Parse("2021-01-01"), AssetId = 5, SourceId = 1 },
                new Price() { Id = 6, Date = DateTime.Parse("2021-01-01"), Value = 6.0M, LastUpdated = DateTime.Parse("2021-01-01"), AssetId = 6, SourceId = 1 },
                new Price() { Id = 7, Date = DateTime.Parse("2021-01-01"), Value = 7.0M, LastUpdated = DateTime.Parse("2021-01-01"), AssetId = 7, SourceId = 1 },
            };

        [Given(@"there are existing prices in the system")]
        public void GivenThereAreExistingPricesInTheSystem()
        {
            scenarioContext["prices"] = _price;
        }

        [When(@"the user requests to retrieve all prices")]
        public async Task WhenTheUserRequestsToRetrieveAllPrices()
        {
            scenarioContext.TryGetValue("prices", out List<Price> prices);

            var mockPricesRepository = new Mock<IPricesRepository>();
            var mockAssetRepository = new Mock<IAssetRepository>();
            var mockSourcesRepository = new Mock<ISourcesRepository>();

            mockPricesRepository.Setup(m => m.GetPricesAsync()).ReturnsAsync(prices);

            var pricesController = new PricesController(mockPricesRepository.Object, mockAssetRepository.Object, mockSourcesRepository.Object);

            scenarioContext["ActionResult"] = await pricesController.GetPrices();
        }

        [Then(@"the system should return a list of all prices")]
        public void ThenTheSystemShouldReturnAListOfAllPrices()
        {
            //Assert
            var result = scenarioContext["ActionResult"] as ObjectResult;

            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK
                && result.Value != null && result.Value is List<Price> && (result.Value as List<Price>).Count() > 0);
        }


        [Given(@"there are no existing prices in the system")]
        public void GivenThereAreNoExistingPricesInTheSystem()
        {
            scenarioContext["prices"] = new List<Price>();
        }


        [Then(@"the system should return an empty price list")]
        public void ThenTheSystemShouldReturnAnEmptyPriceList()
        {
            //Assert
            var result = scenarioContext["ActionResult"] as ObjectResult;

            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK
                && result.Value != null && result.Value is List<Price> && (result.Value as List<Price>).Count() == 0);
        }

        [When(@"the user requests to retrieve the price with ID (.*)")]
        public async Task WhenTheUserRequestsToRetrieveThePriceWithID(int priceId)
        {
            scenarioContext.TryGetValue("prices", out List<Price> prices);

            var price = prices.FirstOrDefault(p => p.Id == priceId);

            var mockPricesRepository = new Mock<IPricesRepository>();
            var mockAssetRepository = new Mock<IAssetRepository>();
            var mockSourcesRepository = new Mock<ISourcesRepository>();

            mockPricesRepository.Setup(m => m.GetPriceAsync(priceId)).ReturnsAsync(price);

            var pricesController = new PricesController(mockPricesRepository.Object, mockAssetRepository.Object, mockSourcesRepository.Object);

            scenarioContext["ActionResult"] = await pricesController.GetPrice(priceId);
        }

        [Then(@"the system should return the details of the price")]
        public void ThenTheSystemShouldReturnTheDetailsOfThePrice()
        {
            //Assert
            var result = scenarioContext["ActionResult"] as ObjectResult;

            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK
                && result.Value != null && result.Value is Price);
        }


        [When(@"the user requests to retrieve prices by date ""([^""]*)""")]
        public async Task WhenTheUserRequestsToRetrievePricesByDate(string dateTime)
        {
            DateTime date = DateTime.Parse(dateTime);
            scenarioContext.TryGetValue("prices", out List<Price> prices);

            var pricesDate = prices.Where(p => p.Date == date).ToList();

            scenarioContext.TryGetValue("assetISINs", out string[] assetISINs);
            scenarioContext.TryGetValue("sourceName", out string sourceName);
            
            var mockPricesRepository = new Mock<IPricesRepository>();
            var mockAssetRepository = new Mock<IAssetRepository>();
            var mockSourcesRepository = new Mock<ISourcesRepository>();

            mockPricesRepository.Setup(m => m.GetSourceAsync(sourceName)).ReturnsAsync(It.IsAny<Source>());
            mockPricesRepository.Setup(m => m.GetAssetsAsync(assetISINs)).ReturnsAsync(It.IsAny<List<Asset>>());
            mockPricesRepository.Setup(m => m.GetPricesAsync(date, It.IsAny<List<Asset>>(), It.IsAny<Source>())).ReturnsAsync(pricesDate);


            var pricesController = new PricesController(mockPricesRepository.Object, mockAssetRepository.Object, mockSourcesRepository.Object);

            scenarioContext["ActionResult"] = await pricesController.GetPriceByDate(date, assetISINs, sourceName);
        }
        
        [Given(@"the user wants to retrieve prices from source ""([^""]*)""")]
        public void GivenTheUserWantsToRetrievePricesFromSource(string sourceName)
        {
            scenarioContext["sourceName"] = sourceName;
        }

        [Given(@"the user wants to retrieve prices from assets with ISIN ""([^""]*)""")]
        public void GivenTheUserWantsToRetrievePricesFromAssetsWithISIN(string ISIN)
        {
            scenarioContext["assetISINs"] = new string[] { ISIN };
        }



        [Then(@"the system should return a list of prices for that date")]
        public void ThenTheSystemShouldReturnAListOfPricesForThatDate()
        {
            //Assert
            var result = scenarioContext["ActionResult"] as ObjectResult;

            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK
                && result.Value != null && (result.Value as List<Price>).Count() > 0);
        }

        [When(@"the user submits an update for the price with ID (.*)")]
        public async Task WhenTheUserSubmitsAnUpdateForThePriceWithID(int priceId)
        {
            scenarioContext.TryGetValue("prices", out List<Price> prices);

            Price price = prices.Find(p => p.Id == priceId);

            price = price ?? new Price { Id = priceId };

            scenarioContext.TryGetValue("existingPrice", out Price existingPrice);
            var mockPricesRepository = new Mock<IPricesRepository>();
            var mockAssetRepository = new Mock<IAssetRepository>();
            var mockSourcesRepository = new Mock<ISourcesRepository>();

            mockPricesRepository.Setup(m => m.GetPriceAsync(price)).ReturnsAsync(existingPrice);
            mockPricesRepository.Setup(m => m.EditPriceAsync(priceId, price)).ReturnsAsync(It.IsAny<Price>());
            
            var pricesController = new PricesController(mockPricesRepository.Object, mockAssetRepository.Object, mockSourcesRepository.Object);

            scenarioContext["ActionResult"] = await pricesController.PutPrice(priceId, price);
        }

        [Given(@"there is another price with a conflicting value")]
        public void GivenThereIsAnotherPriceWithAConflictingValue()
        {
            scenarioContext["existingPrice"] = _price.Find(p => p.Id == 2);
        }

        [Given(@"there is a price with ID (.*)")]
        public void GivenThereIsAPriceWithID(int priceId)
        {
            scenarioContext["existingPrice"] = _price.Find(p => p.Id == priceId);
        }


        [Then(@"the system should update the price details")]
        public void ThenTheSystemShouldUpdateThePriceDetails()
        {
            //Assert
            var result = scenarioContext["ActionResult"] as ObjectResult;

            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK);
        }


    }
}