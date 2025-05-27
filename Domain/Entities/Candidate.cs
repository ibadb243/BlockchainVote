using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Candidate
    {
        public int Id { get; set; }
        public Guid PollId { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public Poll Poll { get; set; } = null!;

        [JsonIgnore]
        public List<VoteCandidate> VoteCandidates { get; set; } = [];
    }
}
