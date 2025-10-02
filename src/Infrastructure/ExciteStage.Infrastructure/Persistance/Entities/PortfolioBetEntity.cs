namespace ExciteStage.Infrastructure.Persistance.Entities
{
    public class PortfolioBetEntity
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public BettingPortfolioEntity Portfolio { get; set; } = null!;
        public string Type { get; set; } = string.Empty; // "Core", "Safety", "Insurance"
        public string Market { get; set; } = string.Empty; // e.g., "Over 2.5"
        public double Odds { get; set; }
        public double StakePercent { get; set; }
        public double Confidence { get; set; }
        public double ExpectedReturn { get; set; }
        public string Reasoning { get; set; } = string.Empty;
    }
}