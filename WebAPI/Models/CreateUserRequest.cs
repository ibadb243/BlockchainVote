using Application.Common.Mappings;
using Application.CQRS.Users.Commands.CreateCommand;
using AutoMapper;

namespace WebAPI.Models;

public class CreateUserRequest : IMapWith<CreateUserCommand>
{
    public string fin { get; set; }
    public string password { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateUserRequest, CreateUserCommand>()
            .ForMember(dest => dest.FIN, opt => opt.MapFrom(src => src.fin))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.password));
    }
}
