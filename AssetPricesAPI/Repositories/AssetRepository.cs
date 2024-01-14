using AssetPricesAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace AssetPricesAPI.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AssetPricesContext context;

        public AssetRepository() { }

        public AssetRepository(AssetPricesContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync()
        {
            return await context.Assets.ToListAsync();
        }

        public async Task<Asset> GetAssetAsync(int id)
        {
            return await context.Assets.FindAsync(id);
        }

        public async Task<bool> IsExistingAssetISINAsync(Asset asset)
        {
            return await context.Assets.AnyAsync(a => a.ISIN == asset.ISIN && a.Id != asset.Id);
        }


        public async Task EditAssetAsync(int id, Asset asset)
        {
            context.Entry(asset).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await AssetExistsAsync(id))
                {
                    throw new ArgumentException($"The Asset with id {id} does not exist.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Asset> AddAssetAsync(Asset asset)
        {
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            return asset;
        }

        public async Task<bool> AssetExistsAsync(int id)
        {
            return await context.Assets.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> AssetExistsAsync(string ISIN)
        {
            return await context.Assets.AnyAsync(e => e.ISIN == ISIN);
        }

    }
}
