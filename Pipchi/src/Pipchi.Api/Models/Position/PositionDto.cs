using Pipchi.Core.Enums;
using Pipchi.Core.ValueObjects;

namespace Pipchi.Api.Models.Position;

public class PositionDto
{
    public Guid OrderId { get; private set; }
    public int SymbolId { get; private set; }
    public Volume Volume { get; private set; }
    public TradeType Type { get; private set; }
    public PositionStatus Status { get; private set; }
    public decimal EntryPrice { get; private set; }
    public decimal? StopLoss { get; private set; }
    public decimal? TakeProfit { get; private set; }
    public decimal? Profit { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
}
