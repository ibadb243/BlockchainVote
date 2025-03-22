using Domain.Entities.Abstract;

namespace Domain.Entities;

public class SingleChoicePoll : PollBase
{
    public bool IsAnonymous { get; set; }

    public SingleChoicePoll()
        : base() { }

    public SingleChoicePoll(Guid userId, string title, string desc, DateTimeOffset startDate, DateTimeOffset? endDate = null, bool isAnonymous = false)
        : base(userId, title, desc, startDate, endDate)
    {
        IsAnonymous = isAnonymous;
    }
}
