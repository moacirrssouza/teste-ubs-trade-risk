namespace UBS.TradeRisk.Domain.Specifications;

/// <summary>
/// Especificação que encapsula as regras de classificação de risco
/// Segue o padrão Specification para facilitar testes e manutenção
/// </summary>
public interface ITradeRiskClassificationSpecification
{
    string ClassifyRisk(decimal value, string clientSector);
}

public class TradeRiskClassificationSpecification : ITradeRiskClassificationSpecification
{
    private const decimal ThresholdAmount = 1_000_000m;

    /// <summary>
    /// Classifica o risco de um trade de acordo com as regras de negócio
    /// Regras:
    /// - LOWRISK: Trades com valor menor que 1.000.000
    /// - MEDIUMRISK: Trades com valor >= 1.000.000 e cliente do setor Privado
    /// - HIGHRISK: Trades com valor >= 1.000.000 e cliente do setor Público
    /// </summary>
    public string ClassifyRisk(decimal value, string clientSector)
    {
        if (value < ThresholdAmount)
            return "LOWRISK";

        return clientSector switch
        {
            "Public" => "HIGHRISK",
            "Private" => "MEDIUMRISK",
            _ => throw new ArgumentException($"Unknown client sector: {clientSector}", nameof(clientSector))
        };
    }
}