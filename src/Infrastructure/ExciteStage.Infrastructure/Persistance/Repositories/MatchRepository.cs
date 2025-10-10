using ExciteStage.Application.Repositories;
using ExciteStage.Domain.Entities;
using ExciteStage.Infrastructure.Persistance.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ExciteStage.Infrastructure.Persistance.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MatchRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Match?> GetByIdAsync(int matchId, CancellationToken ct)
        {
            var entity = await _context.Matches
                .Include(m => m.Portfolios)
                .FirstOrDefaultAsync(m => m.Id == matchId, ct);
            return entity == null ? null : _mapper.Map<Match>(entity);
        }

        public async Task<List<Match>> GetUpcomingMatchesAsync(CancellationToken ct)
        {
            var entities = await _context.Matches
                .Where(m => m.MatchDate > DateTime.UtcNow)
                .OrderBy(m => m.MatchDate)
                .Take(10)
                .ToListAsync(ct);
            return _mapper.Map<List<Match>>(entities);
        }

        public async Task<List<Match>> GetAllAsync(CancellationToken ct)
        {
            var entities = await _context.Matches.ToListAsync(ct);
            return _mapper.Map<List<Match>>(entities);
        }
    }
}