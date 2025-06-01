using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Vote
    {
        public Guid PollId { get; set; }
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; }

        [JsonIgnore]
        public Poll Poll { get; set; } = null!;

        [JsonIgnore]
        public User User { get; set; } = null!;
        public List<VoteCandidate> Candidates { get; set; } = [];
    }
}
