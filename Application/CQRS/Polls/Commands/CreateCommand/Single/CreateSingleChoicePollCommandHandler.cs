using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.CQRS.Polls.Commands.CreateCommand.Single;

public class CreateSingleChoicePollCommandHandler : IRequestHandler<CreateSingleChoicePollCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSingleChoicePollCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSingleChoicePollCommand request, CancellationToken cancellationToken)
    {
        var poll = new SingleChoicePoll(
            request.UserId,
            request.Title,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.IsAnonymous);

        await _context.SingleChoicePolls.AddAsync(poll, cancellationToken);

        for (int i = 0; i < request.Options.Count; i++)
        {
            var option = new PollOption(
                poll.Id,
                1 << i,
                request.Options[i].Fullname,
                request.Options[i].Description,
                request.Options[i].ImagePath);

            await _context.PollOption.AddAsync(option,cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return poll.Id;
    }
}
