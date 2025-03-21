using Domain.Entities.Abstract;

namespace Domain.Entities;

public class MultipleChoicePoll : PollBase
{
    public int MaxSelections { get; set; }

    public MultipleChoicePoll()
        : base() { }

    public MultipleChoicePoll(Guid userId, string title, string desc, int maxSelect, DateTimeOffset startDate, DateTimeOffset? endDate = null)
        : base(userId, title, desc, startDate, endDate)
    {
        MaxSelections = maxSelect;
    }
}
