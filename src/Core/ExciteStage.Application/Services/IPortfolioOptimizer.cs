using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.Services
{
    public interface IPortfolioOptimizer
    {
        Task<BettingPortfolio> OptimizePortfolioAsync(Match match, MatchPredictions predictions, double maxRiskPercent = 0.15);
    }
}