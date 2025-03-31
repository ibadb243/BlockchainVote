using MediatR;

namespace Application.CQRS.QuickPolls.Queries.GetDetails;

public class GetQuickPollDetailsQuery : IRequest<QuickPollVm>
{
    public Guid PollId { get; set; }
}
