using Microsoft.EntityFrameworkCore;
using ExciteStage.Infrastructure.Persistance.Entities;
using ExciteStage.Infrastructure.Persistance.Configurations;

namespace ExciteStage.Infrastructure.Persistance.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<MatchEntity> Matches { get; set; }
        public DbSet<BettingPortfolioEntity> Portfolios { get; set; }
        public DbSet<PortfolioBetEntity> Bets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new MatchEntityConfiguration());
            builder.ApplyConfiguration(new BettingPortfolioEntityConfiguration());
            builder.ApplyConfiguration(new PortfolioBetEntityConfiguration());

            // Si tienes PredictionEntity, también:
            // builder.ApplyConfiguration(new PredictionEntityConfiguration());
        }
    }
}