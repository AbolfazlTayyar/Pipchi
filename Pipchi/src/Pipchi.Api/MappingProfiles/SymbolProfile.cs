using AutoMapper;
using Pipchi.Contracts.DTOs;
using Pipchi.Core.SyncedAggregates;

namespace Pipchi.Api.MappingProfiles;

public class SymbolProfile : Profile
{
    public SymbolProfile()
    {
        CreateMap<Symbol, SymbolDto>();
    }
}
