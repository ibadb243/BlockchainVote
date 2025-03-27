using MediatR;

namespace Application.CQRS.QuickPolls.Commands.CreateCommand;

public class CreateQuickPollCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);
    public List<OptionDto> Options { get; set; } = new List<OptionDto>();
}
