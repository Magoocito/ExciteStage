using ExciteStage.Application.Repositories;
using ExciteStage.Infrastructure.Persistance.Context;

namespace ExciteStage.Infrastructure.Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CommitAsync(CancellationToken ct)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var success = await _context.SaveChangesAsync(ct) > 0;
                await transaction.CommitAsync(ct);
                return success;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}