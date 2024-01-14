using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetPricesAPI;
using AssetPricesAPI.Models;
using AssetPricesAPI.Repositories;

namespace AssetPricesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly AssetPricesContext _context;
        private readonly IAssetRepository _assetRepository;

        // Constructor with Dependency Injection
        [ActivatorUtilitiesConstructor]
        public AssetsController(AssetPricesContext context)
        {
            _context = context;
            _assetRepository = new AssetRepository(context);
        }

        // Constructor with Repository Injection
        public AssetsController(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        // GET: api/Assets
        // Retrieve all assets
        [HttpGet]
        public async Task<IActionResult> GetAssets()
        {
            return Ok(await _assetRepository.GetAssetsAsync());
        }

        // GET: api/Assets/5
        // Retrieve a specific asset by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsset(int id)
        {
            var asset = await _assetRepository.GetAssetAsync(id);

            if (asset == null)
            {
                // Return a 400 Bad Request with a custom message if the asset is not found
                return new ObjectResult("Asset Not Found") { StatusCode = StatusCodes.Status400BadRequest };
            }

            return Ok(asset);
        }

        // PUT: api/Assets/5
        // Update an existing asset by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsset(int id, Asset asset)
        {
            try
            {
                // Check for valid ID and match with the provided asset
                if (id != asset.Id)
                {
                    return BadRequest("The asset id is not valid.");
                }

                // Check for duplicate asset ISIN
                if (await _assetRepository.IsExistingAssetISINAsync(asset))
                {
                    return BadRequest($"The Asset ISIN is already exist.");
                }

                // Update the asset in the repository
                await _assetRepository.EditAssetAsync(id, asset);
            }
            catch (Exception ex)
            {
                // Return a 400 Bad Request with the exception message if an error occurs
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        // POST: api/Assets
        // Add a new asset
        [HttpPost]
        public async Task<IActionResult> PostAsset(Asset asset)
        {
            try
            {
                // Check for existing asset with the same ISIN
                if (!await _assetRepository.AssetExistsAsync(asset.ISIN))
                {
                    // Add a new asset if no existing asset is found
                    var newAsset = await _assetRepository.AddAssetAsync(asset);
                    asset = newAsset;
                }
                else
                {
                    // Return a 400 Bad Request with a custom message if the asset ISIN already exists
                    return BadRequest("The Asset ISIN is already existing.");
                }
            }
            catch (Exception ex)
            {
                // Return a 400 Bad Request with the exception message if an error occurs
                return BadRequest(ex.Message);
            }

            // Return a 201 Created status with the created asset
            return CreatedAtAction("GetAsset", new { id = asset.Id }, asset);
        }
    }
}
