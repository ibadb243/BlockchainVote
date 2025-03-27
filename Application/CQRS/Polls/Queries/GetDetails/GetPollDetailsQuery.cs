using MediatR;

namespace Application.CQRS.Polls.Queries.GetDetails;

public class GetPollDetailsQuery : IRequest<PollVm>
{
    public Guid PollId { get; set; }
}
