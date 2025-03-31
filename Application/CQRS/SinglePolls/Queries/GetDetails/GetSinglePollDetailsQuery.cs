using MediatR;

namespace Application.CQRS.SinglePolls.Queries.GetDetails;

public class GetSinglePollDetailsQuery : IRequest<SinglePollVm>
{
    public Guid PollId { get; set; }
}
