using AssetPricesAPI.Controllers;
using AssetPricesAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using TechTalk.SpecFlow;
using AutoMapper;
using NuGet.ContentModel;
using Asset = AssetPricesAPI.Models.Asset;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using AssetPricesAPI.Repositories;


namespace AssetPriceXUnitSpecFlow.StepDefinitions.API.Controllers
{
    [Binding]
    public sealed class AssetsControllerDefinition
    {
        private List<Asset> _assets = new List<Asset> {
                new() { Id = 1, ISIN = "US0378331005", Name = "Apple Inc.", Symbol = "APLI" },
                new() { Id = 2, ISIN = "US5949181045", Name = "Microsoft Corporation", Symbol = "MSC" },
                new() { Id = 3, ISIN = "US0231351067", Name = "Amazon.com, Inc.", Symbol = "AWS" },
                new() { Id = 4, ISIN = "US02079K3059", Name = "Alphabet Inc.", Symbol = "GGL" },
                new() { Id = 5, ISIN = "US38259P5089", Name = "Goldman Sachs Group, Inc.", Symbol = "GSGI" },
                new() { Id = 6, ISIN = "US0378331005A", Name = "Apple Inc.", Symbol = "APLI" },
                new() { Id = 7, ISIN = "US5949181045A", Name = "Microsoft Corporation", Symbol = "MSC" },
                new() { Id = 8, ISIN = "US0231351067A", Name = "Amazon.com, Inc.", Symbol = "AWS" },
                new() { Id = 9, ISIN = "US02079K3059A", Name = "Alphabet Inc.", Symbol = "GGL" },
                new() { Id = 10, ISIN = "US38259P5089A", Name = "Goldman Sachs Group, Inc.", Symbol = "GSGI" }
            };

        private readonly ScenarioContext scenarioContext;

        public AssetsControllerDefinition(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given(@"there are existing assets in the system")]
        public void GivenThereAreExistingAssetsInTheSystem()
        {
            scenarioContext["assets"] = _assets;
        }

        [Given(@"there are no existing assets in the system")]
        public void GivenThereAreNoExistingAssetsInTheSystem()
        {
            scenarioContext["assets"] = new List<Asset>();
        }


        [When(@"the user requests to retrieve all assets")]
        public async Task WhenTheUserRequestsToRetrieveAllAssets()
        {
            scenarioContext.TryGetValue("assets", out List<Asset> assets);

            var mockAssetRepository = new Mock<IAssetRepository>();

            mockAssetRepository.Setup(m => m.GetAssetsAsync()).ReturnsAsync(assets);
            
            var assetsController = new AssetsController(mockAssetRepository.Object);

            scenarioContext["ActionResult"] = await assetsController.GetAssets();
        }


        [Then(@"the system should return a list of all assets")]
        public void ThenTheSystemShouldReturnAListOfAllAssets()
        {
            //Assert
            var result = scenarioContext["ActionResult"] as ObjectResult;

            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK
                && result.Value != null && result.Value is List<Asset>);
        }


        //the system should return an empty list
        [Then(@"the system should return an empty list")]
        public void ThenTheSystemShouldReturnAnEmptyList()
        {
            //Assert
            var result = scenarioContext["ActionResult"] as ObjectResult;

            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK
                && result.Value.As<List<Asset>>().Count == 0 && result.Value is List<Asset>);
        }


        [Then(@"the system should return the details of the asset")]
        public void ThenTheSystemShouldReturnDetailedAsset()
        {
            //Assert
            var result = scenarioContext["ActionResult"] as ObjectResult;

            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK
                && result.Value != null && result.Value is Asset);
        }


        [When(@"the user requests to retrieve the asset with ID (.*)")]
        public async Task WhenTheUserRequestsToRetrieveTheAssetWithID(int AssetID)
        {
            scenarioContext.TryGetValue("assets", out List<Asset> assets);

            var mockAssetrepository = new Mock<IAssetRepository>();
            mockAssetrepository.Setup(m => m.GetAssetAsync(AssetID)).ReturnsAsync(assets.Find(a => a.Id == AssetID));
            var assetsController = new AssetsController(mockAssetrepository.Object);

            scenarioContext["ActionResult"] = await assetsController.GetAsset(AssetID);
        }


        [Given(@"there is no asset with ID (.*) in the system")]
        public void GivenThereIsNoAssetWithIDInTheSystem(int AssetID)
        {
            scenarioContext["assets"] = _assets;
        }


        [Then(@"the system should return a ""([^""]*)"" error")]
        public void ThenTheSystemShouldReturnAError(string message)
        {
            //Assert
            var result = scenarioContext["ActionResult"] as ObjectResult;

            Assert.True(result != null && result.StatusCode == StatusCodes.Status400BadRequest
                && Convert.ToString(result.Value) == message);
        }

        [When(@"the user submits an update for the asset with ID (.*)")]
        public async Task WhenTheUserSubmitsAnUpdateForTheAssetWithID(int AssetId)
        {
            scenarioContext.TryGetValue("assets", out List<Asset> assets);

            var asset = assets.Find(a => a.Id == AssetId);

            var mockAssetRepository = new Mock<IAssetRepository>();
            mockAssetRepository.Setup(m => m.IsExistingAssetISINAsync(asset)).ReturnsAsync(false);
            mockAssetRepository.Setup(m => m.EditAssetAsync(AssetId, asset));
            
            var assetsController = new AssetsController(mockAssetRepository.Object);

            scenarioContext["ActionResult"] = await assetsController.PutAsset(AssetId, asset);
        }

        [Then(@"the system should update the asset details")]
        public void ThenTheSystemShouldUpdateTheAssetDetails()
        {
            //Assert
            var result = scenarioContext["ActionResult"] as NoContentResult;

            Assert.True(result != null && result.StatusCode == StatusCodes.Status204NoContent);
        }

        [When(@"the user attempts to update the asset ID (.*) with the conflicting ISIN")]
        public async Task WhenTheUserAttemptsToUpdateTheAssetIDWithTheConflictingISIN(int AssetId)
        {
            scenarioContext.TryGetValue("assets", out List<Asset> assets);

            var asset = new Asset { Id = AssetId };

            var mockAssetRepository = new Mock<IAssetRepository>();
            mockAssetRepository.Setup(m => m.IsExistingAssetISINAsync(asset)).ReturnsAsync(true);
            mockAssetRepository.Setup(m => m.EditAssetAsync(AssetId, asset));

            var assetsController = new AssetsController(mockAssetRepository.Object);

            scenarioContext["ActionResult"] = await assetsController.PutAsset(AssetId, asset);
        }


        [When(@"the user attempts to update the asset with ID (.*)")]
        public async Task WhenTheUserAttemptsToUpdateTheAssetWithID(int AssetId)
        {
            scenarioContext.TryGetValue("assets", out List<Asset> assets);

            var asset = new Asset { Id = AssetId };

            var mockAssetRepository = new Mock<IAssetRepository>();
            mockAssetRepository.Setup(m => m.IsExistingAssetISINAsync(asset)).ReturnsAsync(false);
            mockAssetRepository.Setup(m => m.EditAssetAsync(AssetId, asset)).Throws(new Exception("Asset Not Found"));

            var assetsController = new AssetsController(mockAssetRepository.Object);

            scenarioContext["ActionResult"] = await assetsController.PutAsset(AssetId, asset);
        }

        [When(@"the user submits a new asset with the ISIN ""([^""]*)""")]
        public async Task WhenTheUserSubmitsANewAssetWithTheISIN(string ISIN)
        {
            scenarioContext.TryGetValue("assets", out List<Asset> assets);

            var asset = new Asset { ISIN = ISIN, Id = 11 };
            
            
            var mockAssetRepository = new Mock<IAssetRepository>();

            mockAssetRepository.Setup(m => m.AssetExistsAsync(asset.ISIN)).ReturnsAsync(assets.Any(a => a.ISIN == ISIN));
            mockAssetRepository.Setup(m => m.AddAssetAsync(asset)).ReturnsAsync(asset);

            var assetsController = new AssetsController(mockAssetRepository.Object);

            scenarioContext["ActionResult"] = await assetsController.PostAsset(asset);
        }

        [Then(@"the system should add the new asset to the database")]
        public void ThenTheSystemShouldAddTheNewAssetToTheDatabase()
        {
            //Assert
            var result = scenarioContext["ActionResult"] as ObjectResult;

            Assert.True(result != null && (result.StatusCode >= StatusCodes.Status200OK && result.StatusCode < StatusCodes.Status300MultipleChoices)
                        && (result.Value as Asset).Id == 11);
        }








    }
}