using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExciteStage.Domain.Entities
{
    public sealed class BettingPortfolio
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public List<PortfolioBet> Bets { get; private set; } = new();
        public double TotalStakePercent => Bets.Sum(b => b.StakePercent);
        public DateTime CreatedAt { get; set; }

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
