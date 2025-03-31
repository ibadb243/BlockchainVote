using MediatR;

namespace Application.CQRS.MultiplePolls.Queries.GetDetails;

public class GetMultiplePollDetailsQuery : IRequest<MultiplePollVm>
{
    public Guid PollId { get; set; }
}
