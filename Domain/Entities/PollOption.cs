namespace Domain.Entities;

public class PollOption
{
    public Guid PollId { get; set; }
    public int Id { get; set; }
    public string Fullname { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }

    public PollOption() { }

    public PollOption(Guid pollId, int id, string fullname, string? desc = null, string? imagePath = null)
    {
        Id = id;
        PollId = pollId;
        Fullname = fullname;
        Description = desc ?? string.Empty;
        ImagePath = imagePath ?? string.Empty;
    }
}
