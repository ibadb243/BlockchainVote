using Application.Common.Exceptions;
using Application.Interfaces;
using Domain.Entities.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Polls.Commands.CloseCommand;

public class ClosePollCommandHandler : IRequestHandler<ClosePollCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public ClosePollCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(ClosePollCommand request, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == request.PollId && p.UserId == request.UserId, cancellationToken);
        if (poll == null) throw new NotFoundException<PollBase>();

        if (poll.EndDate < DateTimeOffset.UtcNow) throw new PollClosedException();
        poll.EndDate = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
