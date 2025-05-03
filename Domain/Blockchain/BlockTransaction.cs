using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Blockchain
{
    public class BlockTransaction
    {
        public Guid PollId { get; set; }
        public Guid VoteId { get; set; }
        public string UserIdHash { get; set; }
        public List<int> CandidateIds { get; set; } = [];
        public DateTime Timestamp { get; set; }
        public string Hash { get; set; }
    }
}
