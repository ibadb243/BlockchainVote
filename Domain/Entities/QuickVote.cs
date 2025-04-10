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

    public override string GetVoteHash()
    {
        var rawData = $"{Id}{PollId}{UserId}{Timestamp}{OptionId}";
        return ComputeHash(rawData);
    }

    private string ComputeHash(string input)
    {
        using var sha512 = System.Security.Cryptography.SHA512.Create();
        var bytes = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}
