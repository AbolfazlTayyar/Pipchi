using AutoMapper;
using Pipchi.Api.Models.Account;
using Pipchi.Core.AccountAggregate;

namespace Pipchi.Api.MappingProfiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Account, AccountDto>()
            .ForMember(dest => dest.Balance,
                opt => opt.MapFrom(src => src.Balance.Amount))
            .ForMember(dest => dest.Currency,
                opt => opt.MapFrom(src => src.Balance.Currency));
    }
}
