using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Abstract;

namespace Application.CQRS.QuickPolls.Queries.GetDetails;

public class QuickPollVm : IMapWith<QuickPoll>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public List<OptionDto> Options { get; set; }
    public bool IsClosed { get; set; }
    public Dictionary<int, int> Results { get; set; } // null if IsClosed = false

    public void Mapping(Profile profile)
    {
        profile.CreateMap<QuickPoll, QuickPollVm>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.GetType().Name))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Options, opt => opt.Ignore())
            .ForMember(dest => dest.IsClosed, opt => opt.MapFrom(src => src.EndDate.HasValue && src.EndDate < DateTimeOffset.UtcNow))
            .ForMember(dest => dest.Results, opt => opt.Ignore());
    }
}
