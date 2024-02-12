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
    public class SourcesController : ControllerBase
    {
        private readonly ISourcesRepository sourcesRepository;

        public SourcesController(ISourcesRepository sourcesRepository)
        {
            this.sourcesRepository = sourcesRepository;
        }

        // GET: api/Sources
        [HttpGet]
        public async Task<IActionResult> GetSources()
        {
            return Ok(sourcesRepository.GetSourcesAsync());
        }

        // GET: api/Sources/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSource(int id)
        {
            var source = await sourcesRepository.GetSourceAsync(id);

            if (source == null)
            {
                return NotFound();
            }

            return Ok(source);
        }

        // PUT: api/Sources/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSource(int id, Source source)
        {
            if (id != source.Id)
            {
                return BadRequest();
            }

            try
            {
                await sourcesRepository.EditSourceAsync(id, source);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        // POST: api/Sources
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostSource(Source source)
        {
            var currentSource = await sourcesRepository.GetSourceAsync(source.Name);

            if (currentSource != null)
            {
                return BadRequest("The Source name is already exist.");
            }
            await sourcesRepository.AddSourceAsync(source);

            return CreatedAtAction("GetSource", new { id = source.Id }, source);
        }

        // DELETE: api/Sources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSource(int id)
        {
            var source = await sourcesRepository.GetSourceAsync(id);
            if (source == null)
            {
                return NotFound();
            }

            await sourcesRepository.DeleteSourceAsync(source);

            return NoContent();
        }

    }
}
