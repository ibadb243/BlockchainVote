using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.CQRS.Users.Queries.GetDetailsQuery;

public class UserVm : IMapWith<User>
{
    public DateTimeOffset CreatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, UserVm>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}