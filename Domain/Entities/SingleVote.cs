using Domain.Entities.Abstract;

namespace Domain.Entities;

public class SingleVote : VoteBase
{
    public int OptionId { get; set; }

    public SingleVote() { }

    public SingleVote(Guid pollId, Guid userId, int optionId)
        : base(pollId, userId)
    {
        OptionId = optionId;
    }
}
