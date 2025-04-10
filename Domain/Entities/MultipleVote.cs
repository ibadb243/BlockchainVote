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

    public override string GetVoteHash()
    {
        var rawData = $"{Id}{PollId}{UserId}{Timestamp}{Options}";
        return ComputeHash(rawData);
    }

    private string ComputeHash(string input)
    {
        using var sha512 = System.Security.Cryptography.SHA512.Create();
        var bytes = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}
