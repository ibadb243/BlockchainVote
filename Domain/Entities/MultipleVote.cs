using Domain.Entities.Abstract;

namespace Domain.Entities;

public class MultipleVote : VoteBase
{
    public int Options { get; set; }

    public MultipleVote() { }

    public MultipleVote(Guid pollId, Guid userId, int options)
        : base(pollId, userId)
    {
        Options = options;
    }
}
