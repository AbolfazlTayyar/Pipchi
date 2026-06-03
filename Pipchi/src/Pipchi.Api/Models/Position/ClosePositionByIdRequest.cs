namespace Pipchi.Api.Models.Position;

public class ClosePositionByIdRequest : BaseRequest
{
    public const string Route = "api/accounts/{accountId}/positions/{positionId}/close";
}
