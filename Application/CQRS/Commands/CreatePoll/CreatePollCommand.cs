using Ardalis.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Commands.CreatePoll
{
    public record CreatePollCommand(
        string Title, 
        List<CreateCandidateDto> Candidates, 
        DateTime StartTime, 
        DateTime EndTime, 
        bool IsSurvey, 
        bool AllowRevote, 
        int? MaxSelections, 
        bool IsAnonymous) 
        : IRequest<Result<Guid>>;
}
