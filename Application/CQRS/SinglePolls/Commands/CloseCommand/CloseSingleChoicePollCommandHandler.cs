using Application.Common.Exceptions;
using Application.Interfaces;
using Domain.Entities.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.SinglePolls.Commands.CloseCommand;

public class CloseSingleChoicePollCommandHandler : IRequestHandler<CloseSingleChoicePollCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public CloseSingleChoicePollCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CloseSingleChoicePollCommand request, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == request.PollId && p.UserId == request.UserId, cancellationToken);
        if (poll == null) throw new NotFoundException<PollBase>();

        if (poll.EndDate < DateTimeOffset.UtcNow) throw new PollClosedException();
        poll.EndDate = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
