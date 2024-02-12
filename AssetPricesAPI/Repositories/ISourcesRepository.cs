using AssetPricesAPI.Models;

namespace AssetPricesAPI.Repositories
{
    public interface ISourcesRepository
    {
        Task<Source> AddSourceAsync(Source source);
        Task DeleteSourceAsync(Source source);
        Task<Source> EditSourceAsync(int id, Source source);
        Task<Source> GetSourceAsync(int id);
        Task<Source> GetSourceAsync(string Name);
        Task<IEnumerable<Source>> GetSourcesAsync();
        Task<bool> SourceExistsAsync(int id);
    }
}