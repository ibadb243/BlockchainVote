using Domain.Entities.Abstract;

namespace Domain.Entities;

public class SingleChoicePoll : PollBase
{
    public SingleChoicePoll()
        : base() { }

    public SingleChoicePoll(Guid userId, string title, string desc, DateTimeOffset startDate, DateTimeOffset? endDate = null)
        : base(userId, title, desc, startDate, endDate) { }
}
