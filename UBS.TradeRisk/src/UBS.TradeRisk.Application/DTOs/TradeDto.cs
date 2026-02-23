namespace UBS.TradeRisk.Application.DTOs;

/// <summary>
/// DTO para representar uma Trade na entrada da API
/// </summary>
public class TradeInputDto
{
    public decimal Value { get; set; }
    public string ClientSector { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
}

/// <summary>
/// DTO para representar uma Trade na saída da API
/// </summary>
public class TradeOutputDto
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public string ClientSector { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string RiskCategory { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO para o resumo de risco por categoria
/// </summary>
public class RiskCategorySummaryDto
{
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public string TopClient { get; set; } = string.Empty;
}

/// <summary>
/// DTO para a resposta da classificação de trades
/// </summary>
public class TradeClassificationResponseDto
{
    public List<string> Categories { get; set; } = new();
}

/// <summary>
/// DTO para a resposta da análise de distribuição de risco
/// </summary>
public class RiskDistributionAnalysisResponseDto
{
    public List<string> Categories { get; set; } = new();
    public Dictionary<string, RiskCategorySummaryDto> Summary { get; set; } = new();
    public long ProcessingTimeMs { get; set; }
}