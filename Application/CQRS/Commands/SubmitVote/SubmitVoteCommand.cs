using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Commands.SubmitVote
{
    public record SubmitVoteCommand(Guid UserId, Guid PollId, List<int> CandidateIds) : IRequest<Guid>;
}
