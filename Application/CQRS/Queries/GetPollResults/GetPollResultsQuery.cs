using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetPollResults
{
    public record GetPollResultsQuery(Guid PollId) : IRequest<Dictionary<int, int>>;
}
