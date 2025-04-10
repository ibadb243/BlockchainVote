using MediatR;

namespace Application.CQRS.SinglePolls.Commands.DeleteCommand;

public class DeleteSingleChoicePollCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid PollId { get; set; }
}
