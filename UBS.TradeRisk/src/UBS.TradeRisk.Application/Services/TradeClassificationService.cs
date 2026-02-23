using UBS.TradeRisk.Application.DTOs;
using UBS.TradeRisk.Domain.Specifications;

namespace UBS.TradeRisk.Application.Services;

/// <summary>
/// Serviço de aplicação para classificação de trades
/// Implementa o padrão de Service Layer
/// </summary>
public interface ITradeClassificationService
{
    TradeClassificationResponseDto ClassifyTrades(List<TradeInputDto> trades);
}

public class TradeClassificationService : ITradeClassificationService
{
    private readonly ITradeRiskClassificationSpecification _riskClassificationSpec;

    public TradeClassificationService(ITradeRiskClassificationSpecification riskClassificationSpec)
    {
        _riskClassificationSpec = riskClassificationSpec ?? 
            throw new ArgumentNullException(nameof(riskClassificationSpec));
    }

    /// <summary>
    /// Classifica uma lista de trades de acordo com as regras de negócio
    /// </summary>
    public TradeClassificationResponseDto ClassifyTrades(List<TradeInputDto> trades)
    {
        if (trades == null || trades.Count == 0)
            throw new ArgumentException("Trades list cannot be null or empty", nameof(trades));

        ValidateTradesInput(trades);

        var categories = new List<string>();

        foreach (var trade in trades)
        {
            var category = _riskClassificationSpec.ClassifyRisk(trade.Value, trade.ClientSector);
            categories.Add(category);
        }

        return new TradeClassificationResponseDto
        {
            Categories = categories
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