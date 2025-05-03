using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Candidate
    {
        public int Id { get; set; }
        public Guid PollId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Poll Poll { get; set; } = null!;
        public List<Vote> Votes { get; set; } = [];
        public List<VoteCandidate> VoteCandidates { get; set; } = [];
    }
}
