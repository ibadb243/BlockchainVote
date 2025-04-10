using Application.Common.Mappings;
using AutoMapper;

namespace WebAPI.Dtos.CreateSinglePoll;

public class OptionDto : IMapWith<Application.CQRS.SinglePolls.Commands.CreateCommand.OptionDto>
{
    public string fullname { get; set; }
    public string image { get; set; }
    public string desc { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<OptionDto, Application.CQRS.SinglePolls.Commands.CreateCommand.OptionDto>()
            .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.fullname))
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.image))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.desc));
    }
}
