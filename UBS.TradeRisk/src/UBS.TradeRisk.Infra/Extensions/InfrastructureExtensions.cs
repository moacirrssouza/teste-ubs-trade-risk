using UBS.TradeRisk.Domain.Repositories;
using UBS.TradeRisk.Domain.Specifications;
using UBS.TradeRisk.Infra.Data;
using UBS.TradeRisk.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UBS.TradeRisk.Infra.Extensions;

/// <summary>
/// Extensão para registrar os serviços da camada Infra no container de DI
/// </summary>
public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<TradeRiskDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
                sqlOptions.CommandTimeout(60)
            )
        );

        // Repositórios
        services.AddScoped<ITradeRepository, TradeRepository>();

        // Especificações de Domínio
        services.AddScoped<ITradeRiskClassificationSpecification, TradeRiskClassificationSpecification>();

        return services;
    }
}