namespace Domain.Entities.Abstract;

public class VoteBase
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public VoteBase() { }

    public VoteBase(Guid pollId, Guid userId)
    {
        Id = Guid.NewGuid();
        PollId = pollId;
        UserId = userId;
        Timestamp = DateTimeOffset.UtcNow;
    }
}
