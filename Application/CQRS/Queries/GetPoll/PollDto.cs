using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetPoll
{
    public class PollDto : IMapWith<Poll>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<CandidateDto> Candidates { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSurvey { get; set; }
        public bool AllowRevote { get; set; }
        public int? MaxSelections { get; set; }
        public bool IsAnonymous { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Poll, PollDto>();
        }
    }
}
