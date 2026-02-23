using UBS.TradeRisk.Domain.Entities;
using UBS.TradeRisk.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UBS.TradeRisk.Infra.Repositories;

/// <summary>
/// Implementação do repositório para Trade usando Entity Framework Core
/// Segue o padrão Repository Pattern
/// </summary>
public class TradeRepository : ITradeRepository
{
    private readonly Data.TradeRiskDbContext _context;

    public TradeRepository(Data.TradeRiskDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Trade> AddAsync(Trade trade, CancellationToken cancellationToken = default)
    {
        if (trade == null)
            throw new ArgumentNullException(nameof(trade));

        await _context.Trades.AddAsync(trade, cancellationToken);
        return trade;
    }

    public async Task<Trade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Trades.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Trade>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Trades.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Trade>> AddRangeAsync(IEnumerable<Trade> trades, CancellationToken cancellationToken = default)
    {
        if (trades == null)
            throw new ArgumentNullException(nameof(trades));

        var tradeList = trades.ToList();
        await _context.Trades.AddRangeAsync(tradeList, cancellationToken);
        return tradeList;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return changes > 0;
    }
}