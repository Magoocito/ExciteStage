using System;
using System.Collections.Generic;

namespace ExciteStage.Infrastructure.Persistance.Entities
{
    public class MatchEntity
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public int Altitude { get; set; }
        public int TravelDistance { get; set; }

        // Navigation property: One match can have many portfolios
        public ICollection<BettingPortfolioEntity> Portfolios { get; set; } = new List<BettingPortfolioEntity>();
    }
}