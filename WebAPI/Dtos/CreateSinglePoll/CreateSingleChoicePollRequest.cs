using Application.Common.Mappings;
using AutoMapper;

namespace WebAPI.Dtos.CreateSinglePoll;

public class CreateSingleChoicePollRequest : IMapWith<Application.CQRS.SinglePolls.Commands.CreateCommand.CreateSingleChoicePollCommand>
{
    public string title { get; set; }
    public string description { get; set; } = string.Empty;
    public bool is_anonymous { get; set; } = false;
    public DateTimeOffset start_date { get; set; }
    public DateTimeOffset? end_date { get; set; }
    public OptionDto[] options { get; set; } = new OptionDto[0];

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateSingleChoicePollRequest, Application.CQRS.SinglePolls.Commands.CreateCommand.CreateSingleChoicePollCommand>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
            .ForMember(dest => dest.IsAnonymous, opt => opt.MapFrom(src => src.is_anonymous))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.start_date))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.end_date))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.options.ToList()));
    }
}
