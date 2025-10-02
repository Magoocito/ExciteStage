using ExciteStage.Application.Repositories;
using ExciteStage.Domain.Entities;
using ExciteStage.Infrastructure.Persistance.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ExciteStage.Infrastructure.Repositories
{
    public class PortfolioBetRepository : IPortfolioBetRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PortfolioBetRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PortfolioBet?> GetByIdAsync(int betId, CancellationToken ct)
        {
            var entity = await _context.Bets.FirstOrDefaultAsync(b => b.Id == betId, ct);
            return entity == null ? null : _mapper.Map<PortfolioBet>(entity);
        }

        public async Task<List<PortfolioBet>> GetByPortfolioIdAsync(int portfolioId, CancellationToken ct)
        {
            var entities = await _context.Bets
                .Where(b => b.PortfolioId == portfolioId)
                .ToListAsync(ct);
            return _mapper.Map<List<PortfolioBet>>(entities);
        }

        public async Task CreateAsync(PortfolioBet bet, CancellationToken ct)
        {
            var entity = _mapper.Map<Infrastructure.Persistance.Entities.PortfolioBetEntity>(bet);
            await _context.Bets.AddAsync(entity, ct);
        }

        public async Task UpdateAsync(PortfolioBet bet, CancellationToken ct)
        {
            var entity = await _context.Bets.FirstOrDefaultAsync(b => b.Id == bet.Id, ct);
            if (entity != null)
            {
                _mapper.Map(bet, entity);
                _context.Bets.Update(entity);
            }
        }

        public async Task DeleteAsync(int betId, CancellationToken ct)
        {
            var entity = await _context.Bets.FirstOrDefaultAsync(b => b.Id == betId, ct);
            if (entity != null)
            {
                _context.Bets.Remove(entity);
            }
        }
    }
}