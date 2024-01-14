using AssetPricesAPI.Models;

namespace AssetPricesAPI.Repositories
{
    public interface IPricesRepository
    {
        Task<List<Price>> GetPricesAsync();
        Task<Price> GetPriceAsync(int id);
        Task<Price> GetPriceAsync(Price price);
        Task<Source> GetSourceAsync(string sourceName);
        Task<List<Asset>> GetAssetsAsync(string[] assetISINs);
        Task<List<Price>> GetPricesAsync(DateTime date, List<Asset> assets, Source source);
        Task<Price> EditPriceAsync(int id, Price price);
        Task<Price> AddPriceAsync(Price price);
        Task<bool> PriceExistsAsync(int id);
        Task DeletePriceAsync(Price price);
    }
}