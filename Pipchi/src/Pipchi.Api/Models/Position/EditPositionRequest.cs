namespace Pipchi.Api.Models.Position;

public class EditPositionRequest : BaseRequest
{
    public const string Route = "api/positions";

    public Guid AccountId { get; set; }
    public Guid PositionId { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
}
