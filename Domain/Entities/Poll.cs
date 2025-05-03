using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Poll
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSurvey { get; set; }
        public bool AllowRevote { get; set; }
        public int? MaxSelections { get; set; }
        public bool IsAnonymous { get; set; }
        public List<Candidate> Candidates { get; set; } = [];
        public List<Vote> Votes { get; set; } = [];
    }
}
