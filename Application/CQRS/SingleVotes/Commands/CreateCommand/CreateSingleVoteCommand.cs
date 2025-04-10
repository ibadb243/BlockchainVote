using MediatR;

namespace Application.CQRS.SingleVotes.Commands.CreateCommand;

public class CreateSingleVoteCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid PollId { get; set; }
    public int OptionId { get; set; }
}
