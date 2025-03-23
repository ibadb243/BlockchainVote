using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.CQRS.Polls.Commands.CreateCommand.Multiple;

public class CreateMultipleChoicePollCommandHandler : IRequestHandler<CreateMultipleChoicePollCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateMultipleChoicePollCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateMultipleChoicePollCommand request, CancellationToken cancellationToken)
    {
        var poll = new MultipleChoicePoll(
            request.UserId,
            request.Title,
            request.Description,
            request.MaxSelections,
            request.StartDate,
            request.EndDate);

        await _context.MultipleChoicePolls.AddAsync(poll, cancellationToken);

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
