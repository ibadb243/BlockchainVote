using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetPoll
{
    public record GetPollQuery(Guid Id) : IRequest<PollDto?>;
}
