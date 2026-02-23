namespace UBS.TradeRisk.Domain.Enums;

/// <summary>
/// Enum que representa as categorias de risco definidas pelas regras de negócio
/// </summary>
public enum RiskCategoryEnum
{
    LowRisk = 1,
    MediumRisk = 2,
    HighRisk = 3
}

/// <summary>
/// Extensões para conversão do enum
/// </summary>
public static class RiskCategoryExtensions
{
    public static string GetRiskCategoryString(this RiskCategoryEnum category) => category switch
    {
        RiskCategoryEnum.LowRisk => "LOWRISK",
        RiskCategoryEnum.MediumRisk => "MEDIUMRISK",
        RiskCategoryEnum.HighRisk => "HIGHRISK",
        _ => throw new ArgumentOutOfRangeException(nameof(category), "Unknown risk category")
    };

    public static RiskCategoryEnum FromString(string category) => category switch
    {
        "LOWRISK" => RiskCategoryEnum.LowRisk,
        "MEDIUMRISK" => RiskCategoryEnum.MediumRisk,
        "HIGHRISK" => RiskCategoryEnum.HighRisk,
        _ => throw new ArgumentException($"Unknown risk category: {category}", nameof(category))
    };
}