using UBS.TradeRisk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace UBS.TradeRisk.Infra.Data;

/// <summary>
/// DbContext para o Trade Risk Management
/// Configura as entidades e relacionamentos com o banco de dados
/// </summary>
public class TradeRiskDbContext : DbContext
{
    public TradeRiskDbContext(DbContextOptions<TradeRiskDbContext> options) : base(options)
    {
    }

    public DbSet<Trade> Trades { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Trade
        modelBuilder.Entity<Trade>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                .ValueGeneratedNever();

            entity.Property(t => t.Value)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(t => t.ClientSector)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(t => t.ClientId)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(t => t.RiskCategory)
                .HasMaxLength(50);

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            // Índices para performance
            entity.HasIndex(t => t.ClientSector);
            entity.HasIndex(t => t.RiskCategory);
            entity.HasIndex(t => t.ClientId);
            entity.HasIndex(t => t.CreatedAt);
        });
    }
}