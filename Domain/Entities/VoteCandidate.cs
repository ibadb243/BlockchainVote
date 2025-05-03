using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VoteCandidate
    {
        public Guid VoteId { get; set; }
        public int CandidateId { get; set; }
        public Vote Vote { get; set; } = null!;
        public Candidate Candidate { get; set; } = null!;
    }
}
