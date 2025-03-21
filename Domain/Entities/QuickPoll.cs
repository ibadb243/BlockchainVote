using Domain.Entities.Abstract;

namespace Domain.Entities;

public class QuickPoll : PollBase
{
    public QuickPoll()
        : base() { }

    public QuickPoll(Guid userId, string title, string desc, DateTimeOffset startDate)
        : base(userId, title, desc, startDate, startDate.AddMinutes(30)) { }

    public QuickPoll(Guid userId, string title, string desc, DateTimeOffset startDate, TimeSpan duration)
        : base(userId, title, desc, startDate, startDate + duration) { }
}