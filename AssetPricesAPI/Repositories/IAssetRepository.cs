using AssetPricesAPI.Models;

namespace AssetPricesAPI.Repositories
{
    public interface IAssetRepository
    {
        public Task<IEnumerable<Asset>> GetAssetsAsync();
        public Task<Asset> GetAssetAsync(int id);
        public Task<bool> IsExistingAssetISINAsync(Asset asset);
        public Task EditAssetAsync(int id, Asset asset);
        public Task<Asset> AddAssetAsync(Asset asset);
        public Task<bool> AssetExistsAsync(int id);
        public Task<bool> AssetExistsAsync(string ISIN);
    }
}
