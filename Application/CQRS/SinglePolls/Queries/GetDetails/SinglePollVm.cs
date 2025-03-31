using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Abstract;

namespace Application.CQRS.SinglePolls.Queries.GetDetails;

public class SinglePollVm : IMapWith<SingleChoicePoll>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public List<OptionDto> Options { get; set; }
    public bool IsAnonymous { get; set; }
    public bool IsClosed { get; set; }
    public Dictionary<int, int> Results { get; set; } // null if IsClosed = false

    public void Mapping(Profile profile)
    {
        profile.CreateMap<SingleChoicePoll, SinglePollVm>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Options, opt => opt.Ignore())
            .ForMember(dest => dest.IsAnonymous, opt => opt.MapFrom(src => src.IsAnonymous))
            .ForMember(dest => dest.IsClosed, opt => opt.MapFrom(src => src.EndDate.HasValue && src.EndDate < DateTimeOffset.UtcNow))
            .ForMember(dest => dest.Results, opt => opt.Ignore());
    }
}
