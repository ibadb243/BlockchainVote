namespace Domain.Entities.Abstract;

public abstract class PollBase
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }


    public PollBase() { }

    protected PollBase(Guid userId, string title, string desc, DateTimeOffset startDate, DateTimeOffset? endDate = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title;
        Description = desc;
        StartDate = startDate;
        EndDate = endDate;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
