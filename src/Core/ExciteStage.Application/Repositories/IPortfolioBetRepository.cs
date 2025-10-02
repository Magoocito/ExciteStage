using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.Repositories
{
    public interface IPortfolioBetRepository
    {
        Task<PortfolioBet?> GetByIdAsync(int betId, CancellationToken ct);
        Task<List<PortfolioBet>> GetByPortfolioIdAsync(int portfolioId, CancellationToken ct);
        Task CreateAsync(PortfolioBet bet, CancellationToken ct);
    }
}