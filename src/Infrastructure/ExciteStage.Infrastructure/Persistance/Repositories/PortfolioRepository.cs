using ExciteStage.Application.Repositories;
using ExciteStage.Domain.Entities;
using ExciteStage.Infrastructure.Persistance.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ExciteStage.Infrastructure.Persistance.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PortfolioRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BettingPortfolio?> GetByIdAsync(int portfolioId, CancellationToken ct)
        {
            var entity = await _context.Portfolios
                .Include(p => p.Bets)
                .FirstOrDefaultAsync(p => p.Id == portfolioId, ct);
            return entity == null ? null : _mapper.Map<BettingPortfolio>(entity);
        }

        public async Task<List<BettingPortfolio>> GetByMatchIdAsync(int matchId, CancellationToken ct)
        {
            var entities = await _context.Portfolios
                .Include(p => p.Bets)
                .Where(p => p.MatchId == matchId)
                .ToListAsync(ct);
            return _mapper.Map<List<BettingPortfolio>>(entities);
        }

        public async Task CreateAsync(BettingPortfolio portfolio, CancellationToken ct)
        {
            var entity = _mapper.Map<Entities.BettingPortfolioEntity>(portfolio);
            await _context.Portfolios.AddAsync(entity, ct);
        }

        public async Task UpdateAsync(BettingPortfolio portfolio, CancellationToken ct)
        {
            var entity = await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == portfolio.Id, ct);
            if (entity != null)
            {
                _mapper.Map(portfolio, entity);
                _context.Portfolios.Update(entity);
            }
        }

        public async Task DeleteAsync(int portfolioId, CancellationToken ct)
        {
            var entity = await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == portfolioId, ct);
            if (entity != null)
            {
                _context.Portfolios.Remove(entity);
            }
        }
    }
}