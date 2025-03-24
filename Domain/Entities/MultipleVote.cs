using Domain.Entities.Abstract;
using System.Text;

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

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append(Id)
            .Append(PollId)
            .Append(UserId)
            .Append(Options)
            .Append(Timestamp);

        return builder.ToString();
    }
}
