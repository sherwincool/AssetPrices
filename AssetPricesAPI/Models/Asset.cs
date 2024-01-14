namespace AssetPricesAPI.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string ISIN { get; set; } = string.Empty;

        public virtual ICollection<Price>? Prices { get; set; } = null;
    }
}