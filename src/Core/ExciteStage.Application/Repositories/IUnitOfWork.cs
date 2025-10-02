namespace ExciteStage.Application.Repositories
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync(CancellationToken ct);
    }
}