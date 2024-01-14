﻿using System;
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
    public class PricesController : ControllerBase
    {
        private readonly AssetPricesContext _context;
        private readonly IPricesRepository _pricesRepository;

        [ActivatorUtilitiesConstructor]
        public PricesController(AssetPricesContext context)
        {
            _context = context;
            _pricesRepository = new PricesRepository(context);
        }

        public PricesController(IPricesRepository pricesRepository)
        {
            _pricesRepository = pricesRepository;
        }

        // GET: api/Prices
        // Retrieve all prices
        [HttpGet]
        public async Task<IActionResult> GetPrices()
        {
            return Ok(await _pricesRepository.GetPricesAsync());
        }

        // GET: api/Prices/5
        // Retrieve a specific price by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrice(int id)
        {
            var price = await _pricesRepository.GetPriceAsync(id);

            if (price == null)
            {
                return new ObjectResult("Price Not Found") { StatusCode = StatusCodes.Status400BadRequest };
            }

            return Ok(price);
        }

        // GET: api/Prices/date
        // Retrieve prices for a specific date, assets, and source
        [HttpGet("date")]
        public async Task<IActionResult> GetPriceByDate(DateTime date, [FromQuery]string[]? assetISINs = null, string sourceName = null)
        {
            Source source = await _pricesRepository.GetSourceAsync(sourceName);

            List<Asset> assets = await _pricesRepository.GetAssetsAsync(assetISINs);

            return Ok(await _pricesRepository.GetPricesAsync(date, assets, source));
        }


        // PUT: api/Prices/5
        // Update an existing price by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrice(int id, Price price)
        {
            // Check for valid ID and match with the provided price
            if (id <= 0 || id != price.Id)
            {
                return BadRequest();
            }

            // Retrieve the current price for comparison
            var currentPrice = await _pricesRepository.GetPriceAsync(price);

            // Handle scenarios for existing and duplicate prices
            if (currentPrice == null)
            {
                return new ObjectResult("Price Not Found") { StatusCode = StatusCodes.Status400BadRequest };
            }
            else if (currentPrice.Id != id)
            {
                return BadRequest("The update will introduce a duplicate price in a different entry.");
            }

            // Set date and last updated information before updating the price
            price.Date = currentPrice.Date;
            price.LastUpdated = DateTime.Now;

            try
            {
                // Update the price in the repository
                await _pricesRepository.EditPriceAsync(id, price);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(price);
        }

        // POST: api/Prices
        // Add a new price or update an existing price
        [HttpPost]
        public async Task<IActionResult> PostPrice(Price price)
        {
            // Set last updated timestamp
            price.LastUpdated = DateTime.Now;

            // Check for existing price with the same asset and source
            var existingPrice = await _pricesRepository.GetPriceAsync(price);

            if (existingPrice != null)
            {
                // Update the existing price if found
                price.Id = existingPrice.Id;
                existingPrice.Value = price.Value;
                existingPrice.LastUpdated = price.LastUpdated = DateTime.Now;

                price.Asset = existingPrice.Asset;
                price.Source = existingPrice.Source;

                await _pricesRepository.EditPriceAsync(price.Id, existingPrice);
            }
            else
            {
                // Add a new price if no existing price is found
                price = await _pricesRepository.AddPriceAsync(price);
                price.Asset = await _context.Assets.FindAsync(price.AssetId);
                price.Source = await _context.Sources.FindAsync(price.SourceId);
            }

            return Ok(CreatedAtAction("GetPrice", new { id = price.Id }, price));
        }

        // DELETE: api/Prices/5
        // Delete an existing price by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrice(int id)
        {
            // Retrieve the price for deletion
            var price = await _pricesRepository.GetPriceAsync(id);

            if (price == null)
            {
                // Return a 400 Bad Request with a custom message if the price is not found
                return new ObjectResult("Price Not Found") { StatusCode = StatusCodes.Status400BadRequest };
            }

            // Delete the price from the repository
            await _pricesRepository.DeletePriceAsync(price);

            return Ok();
        }

    }
}
