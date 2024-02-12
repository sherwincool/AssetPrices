using AssetPricesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetPricesAPI.Repositories
{
    public class SourcesRepository : ISourcesRepository
    {
        private readonly AssetPricesContext context;

        public SourcesRepository(AssetPricesContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Source>> GetSourcesAsync()
        {
            return await context.Sources.ToListAsync();
        }


        public async Task<Source> GetSourceAsync(int id)
        {
            return await context.Sources.FindAsync(id);
        }

        public async Task<Source> GetSourceAsync(string Name)
        {
            return await context.Sources.Where(s => s.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstAsync();
        }


        public async Task<Source> EditSourceAsync(int id, Source source)
        {
            context.Entry(source).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SourceExistsAsync(id))
                {
                    throw new ArgumentException($"The Source entry does not exist.");
                }
                else
                {
                    throw;
                }
            }

            return source;
        }

        public async Task<bool> SourceExistsAsync(int id)
        {
            return await context.Sources.AnyAsync(e => e.Id == id);
        }

        public async Task<Source> AddSourceAsync(Source source)
        {
            context.Sources.Add(source);
            await context.SaveChangesAsync();

            return source;
        }

        public async Task DeleteSourceAsync(Source source)
        {
            context.Sources.Remove(source);
            await context.SaveChangesAsync();
        }

    }
}
