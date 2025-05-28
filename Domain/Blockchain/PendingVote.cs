using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Blockchain
{
    public class PendingVote
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PollId { get; set; }
        public DateTime Timestamp { get; set; }
        public List<int> CandidateIds { get; set; } = [];
    }
}
