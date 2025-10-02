using System.Collections.Generic;

namespace ExciteStage.Application.UseCases.CreatePortfolio
{
    public class CreatePortfolioResponse
    {
        public int PortfolioId { get; set; }
        public int MatchId { get; set; }
        public string MatchName { get; set; }
        public double TotalRiskPercent { get; set; }
        public double ExpectedReturn { get; set; }
        public List<BetDto> Bets { get; set; } = new();
    }

    public class BetDto
    {
        public string Type { get; set; }
        public string Market { get; set; }
        public double Odds { get; set; }
        public double StakePercent { get; set; }
        public double Confidence { get; set; }
        public string Reasoning { get; set; }
    }
}