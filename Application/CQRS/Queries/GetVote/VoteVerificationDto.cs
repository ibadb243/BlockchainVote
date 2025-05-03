using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetVote
{
    public class VoteVerificationDto : IMapWith<Vote>
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid PollId { get; set; }
        public List<int> CandidateIds { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsAnonymous { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Vote, VoteVerificationDto>()
                .ForMember(dest => dest.CandidateIds, opt => opt.MapFrom(src => src.Candidates.Select(c => c.CandidateId)))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Poll.IsAnonymous ? (Guid?)null : src.UserId))
                .ForMember(dest => dest.IsAnonymous, opt => opt.MapFrom(src => src.Poll.IsAnonymous));
        }
    }
}
