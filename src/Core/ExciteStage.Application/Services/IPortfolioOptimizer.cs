using ExciteStage.Application.Services.HFT;
using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.Services
{
    public interface IPortfolioOptimizer
    {
        Task<BettingPortfolio> OptimizePortfolioAsync(Match match, MatchPredictions predictions,List<MarketOdds> availableOdds, decimal maxRiskPercent = 0.15m);
    }
}