using ExciteStage.Infrastructure.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ExciteStage.Infrastructure.Persistance.Configurations
{
    public class MatchEntityConfiguration : IEntityTypeConfiguration<MatchEntity>
    {
        public void Configure(EntityTypeBuilder<MatchEntity> builder)
        {
            builder.ToTable("Matches");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.HomeTeam)
                .IsRequired()
                .HasMaxLength(100); 

            builder.Property(m => m.AwayTeam)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.MatchDate)
                .IsRequired();

            builder.Property(m => m.Altitude)
                .IsRequired();

            builder.Property(m => m.TravelDistance)
                .IsRequired();

            // Relación 1:N con BettingPortfolioEntity
            builder.HasMany(m => m.Portfolios)
                .WithOne(p => p.Match)
                .HasForeignKey(p => p.MatchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}