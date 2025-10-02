using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExciteStage.Infrastructure.Persistance.Entities;

namespace ExciteStage.Infrastructure.Persistance.Configurations
{
    public class PortfolioBetEntityConfiguration : IEntityTypeConfiguration<PortfolioBetEntity>
    {
        public void Configure(EntityTypeBuilder<PortfolioBetEntity> builder)
        {
            builder.ToTable("PortfolioBets");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.Market)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Odds)
                .IsRequired();

            builder.Property(b => b.StakePercent)
                .IsRequired();

            builder.Property(b => b.Confidence)
                .IsRequired();

            builder.Property(b => b.ExpectedReturn)
                .IsRequired();

            builder.Property(b => b.Reasoning)
                .HasMaxLength(500);
        }
    }
}