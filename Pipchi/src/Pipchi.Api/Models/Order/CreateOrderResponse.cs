using Pipchi.Api.Models.DTOs;

namespace Pipchi.Api.Models.Order;

public class CreateOrderResponse : BaseResponse
{
    public CreateOrderResponse(Guid correlationId) : base(correlationId)
    {
        
    }

    public OrderDto OrderDto { get; set; } = new();
}
