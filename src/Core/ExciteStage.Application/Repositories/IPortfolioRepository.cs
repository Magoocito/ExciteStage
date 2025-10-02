using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.Repositories
{
    public interface IPortfolioRepository
    {
        Task<BettingPortfolio?> GetByIdAsync(int portfolioId, CancellationToken ct);
        Task<List<BettingPortfolio>> GetByMatchIdAsync(int matchId, CancellationToken ct);
        Task CreateAsync(BettingPortfolio portfolio, CancellationToken ct);
        // Puedes agregar más métodos según la lógica de negocio
    }
}