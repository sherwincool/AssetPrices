using AssetPricesAPI.Models;
using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

public class AssetPricesContext : DbContext
{
    public AssetPricesContext(DbContextOptions<AssetPricesContext> options) : base(options)
    {
    }

    public AssetPricesContext()
    {
    }

    public virtual DbSet<Asset> Assets { get; set; }
    public virtual DbSet<Price> Prices { get; set; }
    public virtual DbSet<Source> Sources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        modelBuilder.Entity<Asset>().ToTable("Asset");
        modelBuilder.Entity<Price>().ToTable("Price");
        modelBuilder.Entity<Source>().ToTable("Source");
    }
}