using MediatR;

namespace ExciteStage.Application.UseCases.CreatePortfolio
{
    public class CreatePortfolioRequest : IRequest<CreatePortfolioResponse>
    {
        public int MatchId { get; set; }
        public double MaxRiskPercent { get; set; } = 0.15;
        public double MinEdgeThreshold { get; set; } = 0.05;
    }
}