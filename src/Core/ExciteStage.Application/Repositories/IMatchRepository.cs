using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.Repositories
{
    public interface IMatchRepository
    {
        Task<Match?> GetByIdAsync(int matchId, CancellationToken ct);
        Task<List<Match>> GetUpcomingMatchesAsync(CancellationToken ct);
        Task<List<Match>> GetAllAsync(CancellationToken ct);
    }
}