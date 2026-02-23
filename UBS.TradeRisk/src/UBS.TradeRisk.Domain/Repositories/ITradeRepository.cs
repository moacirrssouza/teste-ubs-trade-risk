using UBS.TradeRisk.Domain.Entities;

namespace UBS.TradeRisk.Domain.Repositories;

/// <summary>
/// Interface do repositório para a entidade Trade
/// Segue o padrão Repository e princípios SOLID
/// </summary>
public interface ITradeRepository
{
    Task<Trade> AddAsync(Trade trade, CancellationToken cancellationToken = default);
    Task<Trade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Trade>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Trade>> AddRangeAsync(IEnumerable<Trade> trades, CancellationToken cancellationToken = default);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}