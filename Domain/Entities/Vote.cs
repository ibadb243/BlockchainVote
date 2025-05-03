using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Vote
    {
        public Guid Id { get; set; }
        public Guid PollId { get; set; }
        public Guid UserId { get; set; }
        public int CandidateId { get; set; }
        public DateTime Timestamp { get; set; }
        public Poll Poll { get; set; } = null!;
        public User User { get; set; } = null!;
        public List<VoteCandidate> Candidates { get; set; } = [];
    }
}
