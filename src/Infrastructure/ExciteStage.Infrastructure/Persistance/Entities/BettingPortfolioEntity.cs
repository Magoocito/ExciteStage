namespace ExciteStage.Infrastructure.Persistance.Entities
{
    public class BettingPortfolioEntity
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public MatchEntity Match { get; set; } = null!;
        public double TotalStakePercent { get; set; }
        public double ExpectedReturn { get; set; }

        // Navigation property: One portfolio can have many bets
        public ICollection<PortfolioBetEntity> Bets { get; set; } = new List<PortfolioBetEntity>();
    }
}