using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.CQRS.Polls.Queries.GetDetails;

public class OptionDto : IMapWith<PollOption>
{
    public int Id { get; set; }
    public string Fullname { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PollOption, OptionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Fullname))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath));
    }
}
