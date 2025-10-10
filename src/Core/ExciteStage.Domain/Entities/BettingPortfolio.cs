namespace ExciteStage.Domain.Entities
{
    public sealed class BettingPortfolio
    {
        public int Id { get; set; }
        public int MatchId { get; set; }

        // Relación con el partido
        public Match Match { get; set; }

        public List<PortfolioBet> Bets { get; private set; } = new();
        public double TotalStakePercent => Bets.Sum(b => b.StakePercent);
        public double ExpectedReturn => Bets.Sum(b => b.ExpectedReturn); // NUEVO

        public DateTime CreatedAt { get; set; }

        // Constructor privado para EF Core
        private BettingPortfolio() { }

        // Constructor público para uso normal
        public BettingPortfolio(int matchId)
        {
            MatchId = matchId;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddBet(PortfolioBet bet)
        {
            if (TotalStakePercent + bet.StakePercent > 0.15)
                throw new InvalidOperationException("Portfolio exceeds 15% bankroll limit");
            Bets.Add(bet);
        }

        public double CalculateWorstCaseScenario()
        {
            return Bets.Min(b => b.ExpectedReturn);
        }
    }
}
