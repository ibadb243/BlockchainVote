using AutoMapper;

namespace Application.Common.Mappings;

public interface IMapWith<T> 
{
    public void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}