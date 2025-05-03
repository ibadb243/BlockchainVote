using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Commands.CreatePoll
{
    public record CreateCandidateDto : IMapWith<Candidate>
    {
        public string? Name { get; set; } = null;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateCandidateDto, Candidate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
