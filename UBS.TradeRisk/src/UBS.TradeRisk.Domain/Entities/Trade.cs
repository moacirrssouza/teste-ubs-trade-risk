namespace UBS.TradeRisk.Domain.Entities;

/// <summary>
/// Entidade de Domínio que representa uma operação financeira (Trade)
/// Segue os princípios de DDD (Domain-Driven Design)
/// </summary>
public class Trade
{
    public Guid Id { get; private set; }
    public decimal Value { get; private set; }
    public string ClientSector { get; private set; }
    public string ClientId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string RiskCategory { get; private set; }

    /// <summary>
    /// Construtor privado para seguir os padrões de DDD
    /// </summary>
    private Trade()
    {
    }

    /// <summary>
    /// Factory Method para criar uma Trade
    /// Garante a criação válida respeitando as regras de negócio
    /// </summary>
    public static Trade Create(decimal value, string clientSector, string clientId)
    {
        ValidateTrade(value, clientSector, clientId);

        return new Trade
        {
            Id = Guid.NewGuid(),
            Value = value,
            ClientSector = clientSector,
            ClientId = clientId,
            CreatedAt = DateTime.UtcNow,
            RiskCategory = string.Empty
        };
    }

    /// <summary>
    /// Classifica o trade de acordo com as regras de negócio
    /// </summary>
    public void ClassifyRisk(string riskCategory)
    {
        if (string.IsNullOrWhiteSpace(riskCategory))
            throw new ArgumentException("Risk category cannot be null or empty", nameof(riskCategory));

        RiskCategory = riskCategory;
    }

    /// <summary>
    /// Valida as regras de negócio para criação de Trade
    /// </summary>
    private static void ValidateTrade(decimal value, string clientSector, string clientId)
    {
        if (value <= 0)
            throw new ArgumentException("Trade value must be greater than zero", nameof(value));

        if (string.IsNullOrWhiteSpace(clientSector))
            throw new ArgumentException("Client sector cannot be null or empty", nameof(clientSector));

        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client ID cannot be null or empty", nameof(clientId));

        var validSectors = new[] { "Public", "Private" };
        if (!validSectors.Contains(clientSector))
            throw new ArgumentException($"Client sector must be 'Public' or 'Private'", nameof(clientSector));
    }
}