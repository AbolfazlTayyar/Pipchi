using AutoMapper;
using Pipchi.Api.Models.Position;
using Pipchi.Core.AccountAggregate;

namespace Pipchi.Api.MappingProfiles
{
    public class PositionProfile : Profile
    {
        public PositionProfile()
        {
            CreateMap<Position, PositionDto>()
                .ForMember(dest => dest.Volume,
                    opt => opt.MapFrom(src => src.Volume.Value));
        }
    }
}
