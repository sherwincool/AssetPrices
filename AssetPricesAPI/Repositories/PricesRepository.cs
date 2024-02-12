using AssetPricesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetPricesAPI.Repositories
{
    public class PricesRepository : IPricesRepository
    {
        private readonly AssetPricesContext _context;

        public PricesRepository(AssetPricesContext context)
        {
            _context = context;
        }

        public async Task<List<Price>> GetPricesAsync()
        {
            return await _context.Prices.ToListAsync();
        }

        public async Task<Price> GetPriceAsync(int id)
        {
            return await _context.Prices.Include(p => p.Source).Include(p => p.Asset).Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Source> GetSourceAsync(string sourceName)
        {
            return await _context.Sources.FirstOrDefaultAsync(s => s.Name.Contains(sourceName));
        }

        public async Task<List<Asset>> GetAssetsAsync(string[] assetISINs)
        {
            return await _context.Assets.Where(a => assetISINs != null && assetISINs.Contains(a.ISIN)).ToListAsync();
        }

        public async Task<List<Price>> GetPricesAsync(DateTime date, List<Asset> assets, Source source)
        {
            return await _context.Prices
                                        .Where(p => (p.Date > date && p.Date < date.AddDays(1))
                                                && (!assets.Any() || assets.Contains(p.Asset))
                                                && (source == null || p.Source.Equals(source)))
                                        .Include(p => p.Source)
                                        .Include(p => p.Asset)
                                        .ToListAsync();
        }

        public async Task<Price> EditPriceAsync(int id, Price price)
        {
            _context.Entry(price).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PriceExistsAsync(id))
                {
                    throw new ArgumentException($"The Price entry does not exist.");
                }
                else
                {
                    throw;
                }
            }

            return price;
        }

        public async Task<Price> GetPriceAsync(Price price)
        {
            return await _context.Prices.Include(p => p.Asset).Include(p => p.Source)
                                .FirstOrDefaultAsync(p => (p.Date > price.Date.Date && p.Date < price.Date.Date.AddDays(1))
                                                                               && p.Asset.Id.Equals(price.AssetId) && p.Source.Id.Equals(price.SourceId));
        }

        public async Task<Price> AddPriceAsync(Price price)
        {
            _context.Prices.Add(price);
            await _context.SaveChangesAsync();

            return price;
        }

        public async Task DeletePriceAsync(Price price)
        {
            _context.Prices.Remove(price);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> PriceExistsAsync(int id)
        {
            return await _context.Prices.AnyAsync(e => e.Id == id);
        }



    }
}
