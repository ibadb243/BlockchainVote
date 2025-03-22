namespace Application.CQRS.Polls.Commands.CreateCommand.Single;

public class OptionDto
{
    public string Fullname { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
}
