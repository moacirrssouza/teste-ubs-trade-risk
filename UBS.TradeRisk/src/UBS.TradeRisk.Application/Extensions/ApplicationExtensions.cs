using UBS.TradeRisk.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace UBS.TradeRisk.Application.Extensions;

/// <summary>
/// Extensão para registrar os serviços da camada Application no container de DI
/// </summary>
public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITradeClassificationService, TradeClassificationService>();
        services.AddScoped<IRiskDistributionAnalysisService, RiskDistributionAnalysisService>();
        return services;
    }
}