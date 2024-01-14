namespace AssetPricesAPI.Models
{
    public class Price
    {
        public int Id { get; set; }
        
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public DateTime LastUpdated { get; set; }

        public int AssetId { get; set; }
        public virtual Asset? Asset { get; set; }

        public int SourceId { get; set; }
        public virtual Source? Source { get; set; }
    }
}