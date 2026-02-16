namespace Pipchi.Api.Models;

public abstract class BaseResponse
{
    protected Guid _correlationId;
    public Guid CorrelationId => _correlationId;

    public BaseResponse(Guid correlationId)
    {
        _correlationId = correlationId;
    }
}
