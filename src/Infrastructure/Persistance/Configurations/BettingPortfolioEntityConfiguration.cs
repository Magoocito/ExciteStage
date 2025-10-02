using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExciteStage.Infrastructure.Persistance.Entities;

namespace ExciteStage.Infrastructure.Persistance.Configurations
{
    public class BettingPortfolioEntityConfiguration : IEntityTypeConfiguration<BettingPortfolioEntity>
    {
        public void Configure(EntityTypeBuilder<BettingPortfolioEntity> builder)
        {
            builder.ToTable("BettingPortfolios");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.TotalStakePercent)
                .IsRequired();

            builder.Property(p => p.ExpectedReturn)
                .IsRequired();

            // Relación 1:N con PortfolioBetEntity
            builder.HasMany(p => p.Bets)
                .WithOne(b => b.Portfolio)
                .HasForeignKey(b => b.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}