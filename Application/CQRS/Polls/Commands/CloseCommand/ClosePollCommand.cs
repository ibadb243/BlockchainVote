using MediatR;

namespace Application.CQRS.Polls.Commands.CloseCommand;

public class ClosePollCommand : IRequest
{
    public Guid UserId { get; set; }
    public Guid PollId { get; set; }
}
