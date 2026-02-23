using UBS.TradeRisk.Application.DTOs;
using UBS.TradeRisk.Domain.Specifications;
using System.Diagnostics;

namespace UBS.TradeRisk.Application.Services;

/// <summary>
/// Serviço de aplicação para análise de distribuição de risco
/// Implementa o padrão de Service Layer
/// </summary>
public interface IRiskDistributionAnalysisService
{
    RiskDistributionAnalysisResponseDto AnalyzeRiskDistribution(List<TradeInputDto> trades);
}

public class RiskDistributionAnalysisService : IRiskDistributionAnalysisService
{
    private readonly ITradeRiskClassificationSpecification _riskClassificationSpec;

    public RiskDistributionAnalysisService(ITradeRiskClassificationSpecification riskClassificationSpec)
    {
        _riskClassificationSpec = riskClassificationSpec ?? 
            throw new ArgumentNullException(nameof(riskClassificationSpec));
    }

    /// <summary>
    /// Analisa a distribuição de risco de uma carteira de trades
    /// </summary>
    public RiskDistributionAnalysisResponseDto AnalyzeRiskDistribution(List<TradeInputDto> trades)
    {
        if (trades == null || trades.Count == 0)
            throw new ArgumentException("Trades list cannot be null or empty", nameof(trades));

        var stopwatch = Stopwatch.StartNew();
        ValidateTradesInput(trades);

        var categories = new List<string>();
        var summary = new Dictionary<string, RiskCategorySummaryDto>
        {
            { "LOWRISK", new RiskCategorySummaryDto { TopClient = string.Empty } },
            { "MEDIUMRISK", new RiskCategorySummaryDto { TopClient = string.Empty } },
            { "HIGHRISK", new RiskCategorySummaryDto { TopClient = string.Empty } }
        };

        var clientExposure = new Dictionary<string, Dictionary<string, decimal>>();

        foreach (var trade in trades)
        {
            var category = _riskClassificationSpec.ClassifyRisk(trade.Value, trade.ClientSector);
            categories.Add(category);

            // Atualizar resumo
            summary[category].Count++;
            summary[category].TotalValue += trade.Value;

            // Rastrear exposição do cliente por categoria
            if (!clientExposure.ContainsKey(category))
                clientExposure[category] = new Dictionary<string, decimal>();

            if (!clientExposure[category].ContainsKey(trade.ClientId))
                clientExposure[category][trade.ClientId] = 0;

            clientExposure[category][trade.ClientId] += trade.Value;
        }

        // Identificar cliente com maior exposição em cada categoria
        foreach (var category in new[] { "LOWRISK", "MEDIUMRISK", "HIGHRISK" })
        {
            if (clientExposure.ContainsKey(category) && clientExposure[category].Count > 0)
            {
                var topClient = clientExposure[category]
                    .OrderByDescending(x => x.Value)
                    .First();
                summary[category].TopClient = topClient.Key;
            }
        }

        stopwatch.Stop();

        return new RiskDistributionAnalysisResponseDto
        {
            Categories = categories,
            Summary = summary,
            ProcessingTimeMs = stopwatch.ElapsedMilliseconds
        };
    }

    private static void ValidateTradesInput(List<TradeInputDto> trades)
    {
        var errors = new List<string>();

        for (int i = 0; i < trades.Count; i++)
        {
            var trade = trades[i];

            if (trade.Value <= 0)
                errors.Add($"Trade {i}: Value must be greater than zero");

            if (string.IsNullOrWhiteSpace(trade.ClientSector))
                errors.Add($"Trade {i}: Client sector is required");
            else if (trade.ClientSector != "Public" && trade.ClientSector != "Private")
                errors.Add($"Trade {i}: Client sector must be 'Public' or 'Private'");

            if (string.IsNullOrWhiteSpace(trade.ClientId))
                errors.Add($"Trade {i}: Client ID is required");
        }

        if (errors.Count > 0)
            throw new ArgumentException(string.Join("; ", errors), nameof(trades));
    }
}