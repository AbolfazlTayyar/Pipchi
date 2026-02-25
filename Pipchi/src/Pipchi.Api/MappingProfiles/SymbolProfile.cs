using AutoMapper;
using Pipchi.Api.Models.Symbol;
using Pipchi.Core.SyncedAggregates;

namespace Pipchi.Api.MappingProfiles;

public class SymbolProfile : Profile
{
    public SymbolProfile()
    {
        CreateMap<Symbol, SymbolDto>();
    }
}
