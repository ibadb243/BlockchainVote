using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.CQRS.Polls.Commands.CreateCommand.Quick;

public class CreateQuickPollCommandHandler : IRequestHandler<CreateQuickPollCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateQuickPollCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateQuickPollCommand request, CancellationToken cancellationToken)
    {
        var poll = new QuickPoll(
            request.UserId,
            request.Title,
            request.Description,
            request.StartDate,
            request.Duration);

        await _context.QuickPolls.AddAsync(poll, cancellationToken);

        for (int i = 0; i < request.Options.Count; i++)
        {
            var option = new PollOption(
                poll.Id,
                1 << i,
                request.Options[i].Fullname,
                request.Options[i].Description,
                request.Options[i].ImagePath);

            await _context.PollOption.AddAsync(option, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return poll.Id;
    }
}
