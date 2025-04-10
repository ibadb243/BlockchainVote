using Application.Common.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.SinglePolls.Commands.DeleteCommand;

public class DeleteSingleChoicePollCommandHandler : IRequestHandler<DeleteSingleChoicePollCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteSingleChoicePollCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteSingleChoicePollCommand request, CancellationToken cancellationToken)
    {
        var poll = await _context.SingleChoicePolls.FirstOrDefaultAsync(p => p.Id == request.PollId, cancellationToken);
        if (poll == null) throw new NotFoundException<SingleChoicePoll>();

        if (poll.UserId != request.UserId) throw new YouAreNotPollCreatorException();

        poll.DeletedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
