using Domain.Entities.Abstract;
using System.Text;

namespace Domain.Entities;

public class QuickVote : VoteBase
{
    public int OptionId { get; set; }

    public QuickVote() { }

    public QuickVote(Guid pollId, Guid userId, int optionId)
        : base(pollId, userId)
    {
        OptionId = optionId;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append(Id)
            .Append(PollId)
            .Append(UserId)
            .Append(OptionId)
            .Append(Timestamp);

        return builder.ToString();
    }
}
