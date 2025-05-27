using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VoteCandidate
    {
        public Guid VoteId { get; set; }
        public Guid PollId { get; set; }
        public int CandidateId { get; set; }

        [JsonIgnore]
        public Vote Vote { get; set; } = null!;

        [JsonIgnore]
        public Candidate Candidate { get; set; } = null!;
    }
}
