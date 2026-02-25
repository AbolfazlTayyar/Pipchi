using AutoMapper;
using Pipchi.Api.Models.Order;
using Pipchi.Core.AccountAggregate;

namespace Pipchi.Api.MappingProfiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
    }
}
