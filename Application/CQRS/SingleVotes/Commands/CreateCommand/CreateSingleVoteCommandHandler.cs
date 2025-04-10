using Application.Common.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Application.CQRS.SingleVotes.Commands.CreateCommand;

public class CreateSingleVoteCommandHandler : IRequestHandler<CreateSingleVoteCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSingleVoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSingleVoteCommand request, CancellationToken cancellationToken)
    {
        var poll = await _context.SingleChoicePolls.FirstOrDefaultAsync(p => p.Id == request.PollId, cancellationToken);
        if (poll == null) throw new NotFoundException<SingleChoicePoll>();

        SingleVote? vote = null;

        if (poll.IsAnonymous)
        {
            vote = await _context.SingleVotes.FirstOrDefaultAsync(v =>
                    v.UserId == hash(request.PollId, request.UserId) &&
                    v.PollId == request.PollId,
                cancellationToken);
        } else
        {
            vote = await _context.SingleVotes.FirstOrDefaultAsync(v => 
                    v.UserId == request.UserId && 
                    v.PollId == request.PollId, 
                cancellationToken);
        }

        if (vote != null) throw new VoteWasLeftException();

        vote = poll.IsAnonymous ?
            new SingleVote(request.PollId, hash(request.PollId, request.UserId), request.OptionId) :
            new SingleVote(request.PollId, request.UserId, request.OptionId);

        await _context.SaveChangesAsync(cancellationToken);

        return vote.Id;
    }

    private Guid hash(Guid pollId, Guid userId) => new Guid(MD5.HashData(userId.ToByteArray()));
}
