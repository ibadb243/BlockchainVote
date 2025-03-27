using MediatR;

namespace Application.CQRS.MultiplePolls.Commands.CreateCommand;

public class CreateMultipleChoicePollCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public int MaxSelections { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public List<OptionDto> Options { get; set; } = new List<OptionDto>();
}
