using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetVote
{
    public record GetVoteQuery(Guid Id) : IRequest<VoteVerificationDto?>;
}
